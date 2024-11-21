namespace Communicator.Options;

public class EmailConfiguration
{
   public string SmtpServer { get; set; } = null!;
   public int SmtpPort { get; set; }
   public string SmtpUsername { get; set; } = null!;
   public string SmtpPassword { get; set; } = null!;
   public bool UseSsl { get; set; }
   public string SenderEmail { get; set; } = null!;
   public int TimeoutMs { get; set; } = 10000;
}