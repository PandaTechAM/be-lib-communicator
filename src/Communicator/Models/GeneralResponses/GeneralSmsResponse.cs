namespace Communicator.Models.GeneralResponses;

public class GeneralSmsResponse
{
    public string From { get; set; } = null!;
    public string To { get; set; } = null!;
    public string OuterSmsId { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public string Body { get; set; } = null!;
}