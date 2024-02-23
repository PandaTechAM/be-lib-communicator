using Communicator.Enums;
using Communicator.Options;

namespace Communicator.Helpers;

internal static class OptionsValidator
{
    internal static void Validate(this CommunicatorOptions options)
    {
        switch (options)
        {
            case { EmailFake: false, EmailConfigurations: null }:
                throw new InvalidOperationException("Email Configuration Option is required.");
            case { SmsFake: false, SmsConfigurations: null }:
                throw new InvalidOperationException("SMS Configuration Option is required.");
        }

        var emailConfigKeys = options.EmailConfigurations?
            .Select(x => x.Key)
            .Where(x => !Channels.EmailChannels.Contains(x))
            .ToList();

        var smsConfigKeys = options.SmsConfigurations?
            .Select(x => x.Key)
            .Where(x => !Channels.SmsChannels.Contains(x))
            .ToList();

        var notSupportedChannels = emailConfigKeys?.Concat(smsConfigKeys ?? []).ToList();

        if (notSupportedChannels?.Count != 0)
        {
            throw new InvalidOperationException(
                $"There are unsupported Channels provided {string.Join(",", notSupportedChannels!)}.");
        }
    }
}