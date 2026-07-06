using Communicator.Models;
using Communicator.Models.GeneralResponses;

namespace Communicator.Services.Interfaces;

/// <summary>Sends SMS messages through the configured providers.</summary>
public interface ISmsService
{
    /// <summary>Send a single SMS and return a normalized response per recipient.</summary>
    Task<List<GeneralSmsResponse>> SendAsync(SmsMessage smsMessage, CancellationToken ct = default);

    /// <summary>Send multiple SMS messages and return a normalized response per recipient.</summary>
    Task<List<GeneralSmsResponse>> SendBulkAsync(List<SmsMessage> smsMessageList, CancellationToken ct = default);
}
