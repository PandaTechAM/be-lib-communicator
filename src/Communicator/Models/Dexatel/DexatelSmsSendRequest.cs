namespace Communicator.Models.Dexatel;

/// <summary>Request envelope for the Dexatel SMS send API.</summary>
public class DexatelSmsSendRequest
{
    /// <summary>Message payload.</summary>
    public required DexatelSmsSendRequestData Data { get; set; }
}
