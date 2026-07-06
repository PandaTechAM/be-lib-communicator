namespace Communicator.Models.Dexatel;

/// <summary>Per-message result returned by the Dexatel SMS send API.</summary>
public class DexatelSmsSendResponseData
{
    /// <summary>Dexatel message id.</summary>
    public required string Id { get; set; }

    /// <summary>Dexatel account id, when returned.</summary>
    public string? AccountId { get; set; }

    /// <summary>Message text.</summary>
    public required string Text { get; set; }

    /// <summary>Sender identifier.</summary>
    public required string From { get; set; }

    /// <summary>Recipient phone number.</summary>
    public required string To { get; set; }

    /// <summary>Dexatel channel identifier.</summary>
    public required string Channel { get; set; }

    /// <summary>Delivery status.</summary>
    public required string Status { get; set; }

    /// <summary>Timestamp the message was created.</summary>
    public DateTime CreateDate { get; set; }

    /// <summary>Timestamp the message was last updated.</summary>
    public DateTime UpdateDate { get; set; }

    /// <summary>Message encoding reported by Dexatel.</summary>
    public required string Encoding { get; set; }
}
