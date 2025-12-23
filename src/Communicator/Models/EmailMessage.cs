namespace Communicator.Models;

public class EmailMessage
{
   public required List<string> Recipients { get; set; }
   public required string Subject { get; set; }
   public required string Body { get; set; }
   public List<string> Cc { get; set; } = [];
   public List<string> Bcc { get; set; } = [];
   public List<EmailAttachment> Attachments { get; set; } = [];
   public bool IsBodyHtml { get; set; } = false;
   public required string Channel { get; set; }
}