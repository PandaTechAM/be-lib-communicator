using Communicator.Models;

namespace Communicator.Services.Interfaces;

public interface ISmsService
{
    Task SendAsync(SmsMessage smsMessage, CancellationToken cancellationToken = default);
    Task SendBulkAsync(List<SmsMessage> smsMessageList, CancellationToken cancellationToken = default);
}