namespace Communicator.Options;

/// <summary>SMTP settings for a single named email channel.</summary>
public class EmailConfiguration
{
    /// <summary>SMTP server host name.</summary>
    public required string SmtpServer { get; set; }

    /// <summary>SMTP server port.</summary>
    public int SmtpPort { get; set; }

    /// <summary>SMTP user name; leave null for servers that accept anonymous/unauthenticated sending.</summary>
    public string? SmtpUsername { get; set; }

    /// <summary>SMTP password; leave null when no authentication is required.</summary>
    public string? SmtpPassword { get; set; }

    /// <summary>Sender email address used in the From header.</summary>
    public required string SenderEmail { get; set; }

    /// <summary>Optional display name shown alongside the sender address.</summary>
    public string? SenderName { get; set; }

    /// <summary>SMTP operation timeout in milliseconds. Defaults to 10000.</summary>
    public int TimeoutMs { get; set; } = 10000;
}
