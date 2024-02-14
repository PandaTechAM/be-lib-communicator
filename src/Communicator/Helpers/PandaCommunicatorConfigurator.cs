using Communicator.Options;
using Microsoft.Extensions.Configuration;

namespace Communicator.Helpers;

public static class PandaCommunicatorConfigurator
{
    public static PandaCommunicatorOptions? ReadConfigurationOptions(IConfiguration configuration)
    {
        // Bind PandaCommunicatorOptions configuration sections
        try
        {
            var pandaCommunicatorSection = configuration.GetSection("PandaCommunicator");
            if (pandaCommunicatorSection.Exists())
            {
                var pandaCommunicatorOptions = new PandaCommunicatorOptions();
                
                configuration.GetSection("PandaCommunicator").Bind(pandaCommunicatorOptions);
                pandaCommunicatorOptions.Validate();
            
                return pandaCommunicatorOptions;
            }

            return null;
        }
        catch (Exception exception)
        {
            throw new Exception(exception.Message);
        }
    }
}