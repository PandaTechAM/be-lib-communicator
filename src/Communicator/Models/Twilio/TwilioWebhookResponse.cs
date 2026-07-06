namespace Communicator.Models.Twilio;

/// <summary>Payload delivered by a Twilio status-callback webhook.</summary>
public class TwilioWebhookResponse
{
    /// <summary>Message text.</summary>
    public string Body { get; set; } = null!;

    /// <summary>Twilio message SID.</summary>
    public string MessageSid { get; set; } = null!;

    /// <summary>Legacy SMS SID (mirrors <see cref="MessageSid" />).</summary>
    public string SmsSid { get; set; } = null!;

    /// <summary>Twilio account SID.</summary>
    public string AccountSid { get; set; } = null!;

    /// <summary>Messaging service SID, when used.</summary>
    public string MessagingServiceSid { get; set; } = null!;

    /// <summary>Sender identifier.</summary>
    public string From { get; set; } = null!;

    /// <summary>Recipient phone number.</summary>
    public string To { get; set; } = null!;

    /// <summary>Number of media items attached.</summary>
    public int NumMedia { get; set; }
}
