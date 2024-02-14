namespace Communicator.Options;

public class PandaCommunicatorOptions
{
    public bool SmsFake { get; set; }
    public List<SmsConfiguration>? SmsConfigurations { get; set; }
    public bool EmailFake { get; set; }
    public List<EmailConfiguration>? EmailConfigurations { get; set; }
}