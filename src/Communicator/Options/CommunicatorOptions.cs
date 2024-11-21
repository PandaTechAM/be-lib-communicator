namespace Communicator.Options;

public class CommunicatorOptions
{
   public bool SmsFake { get; set; }
   public Dictionary<string, SmsConfiguration>? SmsConfigurations { get; set; }
   public bool EmailFake { get; set; }
   public Dictionary<string, EmailConfiguration>? EmailConfigurations { get; set; }
}