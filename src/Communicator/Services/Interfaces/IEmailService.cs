using Communicator.Models;

namespace Communicator.Services.Interfaces;

public interface IEmailService
{
   Task<string> SendAsync(EmailMessage emailMessage, CancellationToken ct = default);
   Task<List<string>> SendBulkAsync(List<EmailMessage> emailMessages, CancellationToken ct = default);
}