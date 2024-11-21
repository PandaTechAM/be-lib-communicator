using Communicator.Helpers;
using Communicator.Models;
using Communicator.Models.GeneralResponses;
using Communicator.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Communicator.Services.Implementations;

internal class FakeSmsService(ILogger<FakeSmsService> logger) : ISmsService
{
   public Task<List<GeneralSmsResponse>> SendAsync(SmsMessage smsMessage, CancellationToken cancellationToken = default)
   {
      smsMessage = SmsMessageValidator.ValidateAndTransform(smsMessage);

      foreach (var recipient in smsMessage.Recipients)
      {
         recipient.Transform();

         logger.LogCritical("Sms sent to {Recipient}\n Sms message is {Message}", recipient, smsMessage.Message);
      }

      return Task.FromResult(new List<GeneralSmsResponse>());
   }

   public Task<List<GeneralSmsResponse>> SendBulkAsync(List<SmsMessage> smsMessageList,
      CancellationToken cancellationToken = default)
   {
      foreach (var smsMessage in smsMessageList)
      {
         SmsMessageValidator.ValidateAndTransform(smsMessage);

         foreach (var recipient in smsMessage.Recipients)
         {
            recipient.Transform();

            logger.LogCritical("Sms sent to {Recipient} \n Sms message is {Message}", recipient, smsMessage.Message);
         }
      }

      return Task.FromResult(new List<GeneralSmsResponse>());
   }
}