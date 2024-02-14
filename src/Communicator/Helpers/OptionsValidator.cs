using Communicator.Options;

namespace Communicator.Helpers;

public static class OptionsValidator
{
    public static void Validate(this PandaCommunicatorOptions options)
    {
        if (options is { EmailFake: false, EmailConfigurations: null })
        {
            throw new InvalidOperationException("Email Configuration Option is required.");
        }

        if (options is { SmsFake: false, SmsConfigurations: null })
        {
            throw new InvalidOperationException("SMS Configuration Option is required.");
        }
    }
}