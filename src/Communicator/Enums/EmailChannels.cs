namespace Communicator.Enums;

/// <summary>Built-in named email channel keys used to select a configured SMTP sender.</summary>
public static class EmailChannels
{
    /// <summary>General-purpose email sender.</summary>
    public const string GeneralSender = "GeneralSender";

    /// <summary>Transactional email sender (receipts, confirmations, password resets).</summary>
    public const string TransactionalSender = "TransactionalSender";

    /// <summary>Notification email sender.</summary>
    public const string NotificationSender = "NotificationSender";

    /// <summary>Marketing email sender.</summary>
    public const string MarketingSender = "MarketingSender";

    /// <summary>Support email sender.</summary>
    public const string SupportSender = "SupportSender";
}
