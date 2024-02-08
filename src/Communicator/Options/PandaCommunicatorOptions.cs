namespace Communicator.Options;

public class PandaCommunicatorOptions
{
    public bool SmsFake { get; set; } = false;
    public SmsConfiguration? SmsConfiguration { get; set; }
    public bool EmailFake { get; set; } = false;
    public EmailConfiguration? EmailConfiguration { get; set; }
}