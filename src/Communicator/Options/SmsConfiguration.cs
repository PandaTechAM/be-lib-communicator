namespace Communicator.Options;

public class SmsConfiguration
{
   public string Provider { get; set; } = null!;
   public string From { get; set; } = null!;
   public Dictionary<string, string> Properties { get; set; } = new();
   public int TimeoutMs { get; set; } = 10000;
}