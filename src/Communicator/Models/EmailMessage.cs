namespace Communicator.Models;

/// <summary>An email to send: recipients, subject, body, optional cc/bcc/attachments, and target channel.</summary>
public class EmailMessage
{
    /// <summary>Primary (To) recipient addresses.</summary>
    public required List<string> Recipients { get; set; }

    /// <summary>Email subject line.</summary>
    public required string Subject { get; set; }

    /// <summary>Email body; plain text or HTML depending on <see cref="IsBodyHtml" />.</summary>
    public required string Body { get; set; }

    /// <summary>Carbon-copy recipient addresses.</summary>
    public List<string> Cc { get; set; } = [];

    /// <summary>Blind carbon-copy recipient addresses.</summary>
    public List<string> Bcc { get; set; } = [];

    /// <summary>File attachments to include.</summary>
    public List<EmailAttachment> Attachments { get; set; } = [];

    /// <summary>Whether <see cref="Body" /> is HTML. Defaults to false (plain text).</summary>
    public bool IsBodyHtml { get; set; } = false;

    /// <summary>Named email channel selecting the configured sender (see <see cref="Enums.EmailChannels" />).</summary>
    public required string Channel { get; set; }
}
