using Communicator.Options;
using Microsoft.Extensions.Configuration;

namespace Communicator.Helpers;

internal static class CommunicatorConfigurator
{
    internal static CommunicatorOptions? ReadConfigurationOptions(IConfiguration configuration)
    {
        var communicatorSection = configuration.GetSection("Communicator");

        if (!communicatorSection.Exists()) return null;

        var communicatorOptions = new CommunicatorOptions();

        communicatorSection.Bind(communicatorOptions);

        return communicatorOptions;
    }
}