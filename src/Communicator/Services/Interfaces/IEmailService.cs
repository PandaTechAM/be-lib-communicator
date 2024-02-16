using Communicator.Models;
using Communicator.Models.GeneralResponses;

namespace Communicator.Services.Interfaces;

public interface IEmailService
{
    Task<GeneralEmailResponse> SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
    Task<List<GeneralEmailResponse>> SendBulkAsync(List<EmailMessage> emailMessages, CancellationToken cancellationToken = default);
}
