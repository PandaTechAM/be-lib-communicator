using Communicator.Models;
using Communicator.Services.Interfaces;

namespace Communicator.Services.Implementations;

internal class SmsService : ISmsService
{
    public Task SendAsync(SmsMessage smsMessage, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SendBulkAsync(List<SmsMessage> smsList, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

}