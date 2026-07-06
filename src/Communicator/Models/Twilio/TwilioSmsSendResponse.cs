namespace Communicator.Models.Twilio;

/// <summary>Response returned by the Twilio message-create API.</summary>
public class TwilioSmsSendResponse
{
    /// <summary>Message text.</summary>
    public string Body { get; set; } = null!;

    /// <summary>Number of SMS segments the message was split into.</summary>
    public int NumSegments { get; set; }

    /// <summary>Message direction (e.g. outbound-api).</summary>
    public string Direction { get; set; } = null!;

    /// <summary>Sender identifier.</summary>
    public string From { get; set; } = null!;

    /// <summary>Timestamp the message was last updated.</summary>
    public DateTime DateUpdated { get; set; }

    /// <summary>Message price, when available.</summary>
    public int? Price { get; set; } // is this string?

    /// <summary>Error message, when the send failed.</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>Relative REST URI of this message resource.</summary>
    public string Uri { get; set; } = null!;

    /// <summary>Twilio account SID.</summary>
    public string AccountSid { get; set; } = null!;

    /// <summary>Recipient phone number.</summary>
    public string To { get; set; } = null!;

    /// <summary>Timestamp the message was created.</summary>
    public DateTime DateCreated { get; set; }

    /// <summary>Delivery status.</summary>
    public string Status { get; set; } = null!;

    /// <summary>Twilio message SID.</summary>
    public string Sid { get; set; } = null!;

    /// <summary>Timestamp the message was sent, when available.</summary>
    public DateTime? DateSent { get; set; }

    /// <summary>Messaging service SID, when used.</summary>
    public string? MessagingServiceSid { get; set; }

    /// <summary>Error code, when the send failed.</summary>
    public int? ErrorCode { get; set; }

    /// <summary>Currency of <see cref="Price" />.</summary>
    public string PriceUnit { get; set; } = null!;

    /// <summary>Twilio API version that handled the request.</summary>
    public string ApiVersion { get; set; } = null!;

    /// <summary>Related subresource URIs.</summary>
    public SubresourceUris SubresourceUris { get; set; } = null!;
}
