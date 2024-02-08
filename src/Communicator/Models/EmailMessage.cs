using Communicator.Enums;

namespace Communicator.Models;

public class EmailMessage
{
    public List<string> Recipients { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
    public List<string> Cc { get; set; } = [];
    public List<string> Bcc { get; set; } = [];
    public List<EmailAttachment> Attachments { get; set; } = [];
    public bool IsBodyHtml { get; set; } = false;
    public string Channel { get; set; } = null!;
}