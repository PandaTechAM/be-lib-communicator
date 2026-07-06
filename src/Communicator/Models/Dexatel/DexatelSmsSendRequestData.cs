namespace Communicator.Models.Dexatel;

/// <summary>Message payload for a Dexatel SMS send request.</summary>
public class DexatelSmsSendRequestData
{
    /// <summary>Sender identifier.</summary>
    public required string From { get; set; }

    /// <summary>Recipient phone numbers.</summary>
    public required List<string> To { get; set; }

    /// <summary>Message text.</summary>
    public required string Text { get; set; }

    /// <summary>Dexatel channel identifier.</summary>
    public required string Channel { get; set; }
}
