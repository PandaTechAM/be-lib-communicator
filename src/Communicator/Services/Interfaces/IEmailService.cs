using Communicator.Models;

namespace Communicator.Services.Interfaces;

public interface IEmailService
{
    Task SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
    Task SendBulkAsync(List<EmailMessage> emailMessages, CancellationToken cancellationToken = default);
}
