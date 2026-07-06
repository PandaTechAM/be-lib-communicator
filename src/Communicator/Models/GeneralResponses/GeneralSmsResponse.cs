namespace Communicator.Models.GeneralResponses;

/// <summary>Normalized SMS send result, unified across providers.</summary>
public class GeneralSmsResponse
{
    /// <summary>Sender identifier the message was sent from.</summary>
    public required string From { get; set; }

    /// <summary>Recipient phone number.</summary>
    public required string To { get; set; }

    /// <summary>Provider-assigned message id.</summary>
    public required string OuterSmsId { get; set; }

    /// <summary>Delivery status reported by the provider.</summary>
    public required string Status { get; set; }

    /// <summary>Timestamp the message was created at the provider.</summary>
    public DateTime CreateDate { get; set; }

    /// <summary>Timestamp the message status was last updated.</summary>
    public DateTime UpdateDate { get; set; }

    /// <summary>Message text.</summary>
    public required string Body { get; set; }
}
