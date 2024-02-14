namespace Communicator.Models;

public class SendSmsResponse
{
    public string To { get; set; } = null!;
    public string OuterSmsId { get; set; } = null!;
    public DateTime ProviderSentDate { get; set; }
    public string Status { get; set; } = null!;
}