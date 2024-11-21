namespace Communicator.Models;

public class SmsMessage
{
   public List<string> Recipients { get; set; } = null!;
   public string Message { get; set; } = null!;
   public string Channel { get; set; } = null!;
}