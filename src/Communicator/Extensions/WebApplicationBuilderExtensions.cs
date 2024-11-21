using Communicator.Enums;
using Communicator.Helpers;
using Communicator.Options;
using Communicator.Services.Implementations;
using Communicator.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Communicator.Extensions;

public static class WebApplicationBuilderExtensions
{
   public static WebApplicationBuilder AddCommunicator(this WebApplicationBuilder builder,
      Action<CommunicatorOptions>? setupAction = null)
   {
      var setupActionOptions = GetCommunicatorSetupOptions(setupAction);

      var configurationOptions = GetCommunicatorConfigurationOptions(builder);

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

      RegisterSmsHttpClientsFromConfig(builder, options);

      RegisterServices(builder, options);

      return builder;
   }

   private static CommunicatorOptions? GetCommunicatorConfigurationOptions(IHostApplicationBuilder builder)
   {
      var configuration = builder.Configuration;

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

   private static void RegisterSmsHttpClientsFromConfig(IHostApplicationBuilder builder,
      CommunicatorOptions communicatorOptions)
   {
      if (communicatorOptions.SmsFake)
      {
         return;
      }

      foreach (var (key, configValue) in communicatorOptions.SmsConfigurations!)
      {
         builder.Services.AddHttpClient(key,
            client =>
            {
               client.BaseAddress = new Uri(SmsProviderIntegrations.BaseUrls[configValue.Provider]);
               client.Timeout = TimeSpan.FromMilliseconds(configValue.TimeoutMs);
            });
      }
   }

   private static void RegisterServices(IHostApplicationBuilder builder,
      CommunicatorOptions communicatorOptions)
   {
      if (communicatorOptions.EmailFake)
      {
         builder.Services.AddScoped<IEmailService, FakeEmailService>();
      }
      else
      {
         builder.Services.AddScoped<IEmailService, EmailService>();
      }

      if (communicatorOptions.SmsFake)
      {
         builder.Services.AddScoped<ISmsService, FakeSmsService>();
      }
      else
      {
         builder.Services.AddScoped<ISmsService, SmsService>();
      }

      builder.Services.AddSingleton(communicatorOptions);
   }
}