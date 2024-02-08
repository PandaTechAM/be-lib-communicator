using Communicator.Extensions;
using RegexBox;

namespace Communicator.Helpers;

internal static class SmsRecipients
{
    internal static string ValidateAndTransform(this string recipient)
    {
        return PandaValidator.IsPandaFormattedPhoneNumber(recipient)
            ? recipient.RemovePhoneFormatParenthesesAndAdditionSign()
            : recipient;
    }

    internal static List<string> ValidateAndTransform(this List<string> recipients)
    {
        if (recipients.Count == 0)
        {
            throw new ArgumentException("At least one recipient is required", nameof(recipients));
        }

        foreach (var recipient in recipients.Where(PandaValidator.IsPandaFormattedPhoneNumber))
        {
            recipient.RemovePhoneFormatParenthesesAndAdditionSign();
        }

        return recipients;
    }
}