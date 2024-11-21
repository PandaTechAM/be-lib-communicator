using Communicator.Models;
using Communicator.Models.GeneralResponses;

namespace Communicator.Services.Interfaces;

public interface ISmsService
{
   Task<List<GeneralSmsResponse>> SendAsync(SmsMessage smsMessage, CancellationToken cancellationToken = default);

   Task<List<GeneralSmsResponse>> SendBulkAsync(List<SmsMessage> smsMessageList,
      CancellationToken cancellationToken = default);
}