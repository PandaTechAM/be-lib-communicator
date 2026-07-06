using Communicator.Models;

namespace Communicator.Services.Interfaces;

/// <summary>Sends emails through the configured SMTP channels.</summary>
public interface IEmailService
{
    /// <summary>Send a single email and return the raw SMTP server response.</summary>
    Task<string> SendAsync(EmailMessage emailMessage, CancellationToken ct = default);

    /// <summary>Send multiple emails and return the raw SMTP server response for each.</summary>
    Task<List<string>> SendBulkAsync(List<EmailMessage> emailMessages, CancellationToken ct = default);
}
