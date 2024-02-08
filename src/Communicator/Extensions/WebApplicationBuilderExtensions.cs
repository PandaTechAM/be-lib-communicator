using Communicator.Services.Implementations;
using Communicator.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Communicator.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder RegisterPandaCommunicator(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEmailService, FakeEmailService>();
        builder.Services.AddScoped<ISmsService, FakeSmsService>();
        return builder;
    }
}