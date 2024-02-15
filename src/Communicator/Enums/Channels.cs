namespace Communicator.Enums;

internal static class Channels
{
    internal static readonly List<string> EmailChannels = new()
    {
        "GeneralSender",
        "TransactionalSender",
        "NotificationSender",
        "MarketingSender",
        "SupportSender"
    };
    
    internal static readonly List<string> SmsChannels = new()
    {
        "GeneralSender",
        "TransactionalSender",
        "NotificationSender",
        "MarketingSender",
        "SupportSender"
    };
}