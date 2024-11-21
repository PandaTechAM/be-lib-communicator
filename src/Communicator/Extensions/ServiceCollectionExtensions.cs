using Communicator.Enums;
using Communicator.Helpers;
using Communicator.Options;
using Communicator.Services.Implementations;
using Communicator.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Communicator.Extensions;

public static class ServiceCollectionExtensions
{
   public static IServiceCollection AddCommunicator(this IServiceCollection services,
      IConfiguration configuration,
      Action<CommunicatorOptions>? setupAction = null)
   {
      var setupActionOptions = GetCommunicatorSetupOptions(setupAction);

      var configurationOptions = GetCommunicatorConfigurationOptions(configuration);

      CommunicatorOptions options;

      if (setupAction is not null)
      {
         options = setupActionOptions!;
      }
      else if (configurationOptions is not null)
      {
         options = configurationOptions;
      }
      else
      {
         throw new Exception("No any Configuration Option setup.");
      }

      RegisterSmsHttpClientsFromConfig(services, options);

      RegisterServices(services, options);

      return services;
   }

   private static CommunicatorOptions? GetCommunicatorConfigurationOptions(IConfiguration configuration)
   {
      var configurationOptions = CommunicatorConfigurator.ReadConfigurationOptions(configuration);

      if (configurationOptions is null)
      {
         return null;
      }

      configurationOptions.Validate();

      return configurationOptions;
   }

   private static CommunicatorOptions? GetCommunicatorSetupOptions(Action<CommunicatorOptions>? setupAction = null)
   {
      var setupOptions = new CommunicatorOptions();

      if (setupAction is null)
      {
         return null;
      }

      setupAction.Invoke(setupOptions);
      setupOptions.Validate();

      return setupOptions;
   }

   private static void RegisterSmsHttpClientsFromConfig(IServiceCollection services,
      CommunicatorOptions communicatorOptions)
   {
      if (communicatorOptions.SmsFake)
      {
         return;
      }

      foreach (var (key, configValue) in communicatorOptions.SmsConfigurations!)
      {
         services.AddHttpClient(key,
            client =>
            {
               client.BaseAddress = new Uri(SmsProviderIntegrations.BaseUrls[configValue.Provider]);
               client.Timeout = TimeSpan.FromMilliseconds(configValue.TimeoutMs);
            });
      }
   }

   private static void RegisterServices(IServiceCollection services,
      CommunicatorOptions communicatorOptions)
   {
      if (communicatorOptions.EmailFake)
      {
         services.AddScoped<IEmailService, FakeEmailService>();
      }
      else
      {
         services.AddScoped<IEmailService, EmailService>();
      }

      if (communicatorOptions.SmsFake)
      {
         services.AddScoped<ISmsService, FakeSmsService>();
      }
      else
      {
         services.AddScoped<ISmsService, SmsService>();
      }

      services.AddSingleton(communicatorOptions);
   }
}