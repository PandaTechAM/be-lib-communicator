namespace Communicator.Options;

public class EmailConfiguration
{
   public required string SmtpServer { get; set; }
   public int SmtpPort { get; set; }
   public string? SmtpUsername { get; set; }
   public string? SmtpPassword { get; set; }
   public required string SenderEmail { get; set; }
   public string? SenderName { get; set; }
   public int TimeoutMs { get; set; } = 10000;
}