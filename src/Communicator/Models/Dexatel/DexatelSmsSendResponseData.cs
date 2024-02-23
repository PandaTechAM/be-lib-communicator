namespace Communicator.Models.Dexatel;

public class DexatelSmsSendResponseData
{
    public string Id { get; set; } = null!;
    public string AccountId { get; set; } = null!;
    public string Text { get; set; } = null!;
    public string From { get; set; } = null!;
    public string To { get; set; } = null!;
    public string Channel { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public string Encoding { get; set; } = null!;
    public int SegmentsCount { get; set; }
}