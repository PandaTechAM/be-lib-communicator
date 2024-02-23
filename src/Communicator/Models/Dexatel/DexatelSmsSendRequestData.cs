namespace Communicator.Models.Dexatel;

public class DexatelSmsSendRequestData
{
    public string From { get; set; } = null!;
    public List<string> To { get; set; } = null!;
    public string Text { get; set; } = null!;
    public string Channel { get; set; } = null!;
}