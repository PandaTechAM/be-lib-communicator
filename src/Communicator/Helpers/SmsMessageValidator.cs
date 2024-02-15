using Communicator.Models;

namespace Communicator.Helpers;

internal static class SmsMessageValidator
{
    internal static void Validate(SmsMessage smsMessage)
    {
        if (smsMessage.Recipients.Count == 0)
        {
            throw new ArgumentException("At least one recipient is required", nameof(smsMessage.Recipients));
        }

        char[] charsToCheck = ['(', ')', ' '];
        if (smsMessage.Recipients.Any(number => number.Any(c => charsToCheck.Contains(c))))
        {
            throw new ArgumentException("Invalid phone number", nameof(smsMessage.Recipients));
        }
    }
}