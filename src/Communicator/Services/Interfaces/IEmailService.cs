using Communicator.Models;
using Communicator.Models.GeneralResponses;

namespace Communicator.Services.Interfaces;

public interface IEmailService
{
    Task<string> SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
    Task<List<string>> SendBulkAsync(List<EmailMessage> emailMessages, CancellationToken cancellationToken = default);
}
