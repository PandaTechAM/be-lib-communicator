namespace Communicator.Models;

public class SmsMessage
{
   public required List<string> Recipients { get; set; }
   public required string Message { get; set; }
   public required string Channel { get; set; }
}