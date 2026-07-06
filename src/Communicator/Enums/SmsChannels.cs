namespace Communicator.Enums;

/// <summary>Built-in named SMS channel keys used to select a configured SMS sender.</summary>
public static class SmsChannels
{
    /// <summary>General-purpose SMS sender.</summary>
    public const string GeneralSender = "GeneralSender";

    /// <summary>Transactional SMS sender (OTP codes, confirmations).</summary>
    public const string TransactionalSender = "TransactionalSender";

    /// <summary>Notification SMS sender.</summary>
    public const string NotificationSender = "NotificationSender";

    /// <summary>Marketing SMS sender.</summary>
    public const string MarketingSender = "MarketingSender";

    /// <summary>Support SMS sender.</summary>
    public const string SupportSender = "SupportSender";
}
