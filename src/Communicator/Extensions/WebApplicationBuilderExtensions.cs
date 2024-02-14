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
    public static WebApplicationBuilder AddPandaCommunicator(this WebApplicationBuilder builder,
        Action<PandaCommunicatorOptions>? setupAction = null)
    {
        var setupActionOptions = GetPandaCommunicatorSetupOptions(setupAction);

        var configurationOptions = GetPandaCommunicatorConfigurationOptions(builder);

        PandaCommunicatorOptions options;

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

        RegisterServices(builder, options);

        return builder;
    }

    private static PandaCommunicatorOptions? GetPandaCommunicatorConfigurationOptions(IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        var configurationOptions = PandaCommunicatorConfigurator.ReadConfigurationOptions(configuration);

        if (configurationOptions is null)
        {
            return null;
        }

        configurationOptions.Validate();

        return configurationOptions;
    }

    private static PandaCommunicatorOptions? GetPandaCommunicatorSetupOptions(
        Action<PandaCommunicatorOptions>? setupAction = null)
    {
        var setupOptions = new PandaCommunicatorOptions();

        if (setupAction is null)
        {
            return null;
        }

        setupAction.Invoke(setupOptions);
        setupOptions.Validate();

        return setupOptions;
    }

    private static void RegisterServices(IHostApplicationBuilder builder,
        PandaCommunicatorOptions pandaCommunicatorOptions)
    {
        if (pandaCommunicatorOptions.EmailFake)
        {
            builder.Services.AddScoped<IEmailService, FakeEmailService>();
        }
        else
        {
            builder.Services.AddScoped<IEmailService, EmailService>();
        }

        if (pandaCommunicatorOptions.SmsFake)
        {
            builder.Services.AddScoped<ISmsService, FakeSmsService>();
        }
        else
        {
            builder.Services.AddScoped<ISmsService, SmsService>();
        }

        builder.Services.AddSingleton(pandaCommunicatorOptions);
    }
}