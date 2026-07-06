namespace Communicator.Models;

/// <summary>An SMS to send: recipients, message text, and target channel.</summary>
public class SmsMessage
{
    /// <summary>Recipient phone numbers in E.164 format.</summary>
    public required List<string> Recipients { get; set; }

    /// <summary>SMS message text.</summary>
    public required string Message { get; set; }

    /// <summary>Named SMS channel selecting the configured sender (see <see cref="Enums.SmsChannels" />).</summary>
    public required string Channel { get; set; }
}
