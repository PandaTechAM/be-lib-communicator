using Communicator.Helpers;
using Communicator.Models;
using Communicator.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Communicator.Services.Implementations;

internal class FakeSmsService(ILogger<FakeSmsService> logger) : ISmsService
{
    public Task SendAsync(SmsMessage smsMessage, CancellationToken cancellationToken = default)
    {
        foreach (var recipient in smsMessage.Recipients)
        {
            recipient.ValidateAndTransform();
            logger.LogCritical("Sms sent to {Recipient}\n Sms message is {Message}", recipient, smsMessage.Message);
        }
        
        return Task.CompletedTask;
    }

    public Task SendBulkAsync(List<SmsMessage> smsMessages, CancellationToken cancellationToken = default)
    {
        foreach (var sms in smsMessages)
        {
            foreach (var recipient in sms.Recipients)
            {
                recipient.ValidateAndTransform();
                logger.LogCritical("Sms sent to {Recipient} \n Sms message is {Message}", recipient, sms.Message);
            }
        }

        return Task.CompletedTask;
    }
}