namespace Communicator.Models.Dexatel;

/// <summary>Response envelope for the Dexatel SMS send API.</summary>
public class DexatelSmsSendResponse
{
    /// <summary>Per-message send results.</summary>
    public List<DexatelSmsSendResponseData> Data { get; set; } = [];
}
