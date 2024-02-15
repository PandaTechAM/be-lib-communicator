namespace Communicator.Enums;

internal static class SmsProviderIntegrations
{
    internal static readonly Dictionary<string, string> BaseUrls = new()
    {
        { "Dexatel", "https://api.dexatel.com" },
        { "Twilio", "https://api.twilio.com" }
    };
}