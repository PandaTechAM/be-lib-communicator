using Communicator.Extensions;
using RegexBox;

namespace Communicator.Helpers;

internal static class SmsRecipients
{
   private static readonly char[] CharsToCheck = ['+', '(', ')', ' '];

   internal static string Transform(this string recipient)
   {
      if (PandaValidator.IsPandaFormattedPhoneNumber(recipient)
          || recipient.Any(c => CharsToCheck.Contains(c)))
      {
         recipient = recipient.RemovePhoneFormatParenthesesAndAdditionSign();
      }

      return recipient;
   }

   internal static List<string> Transform(this List<string> recipients)
   {
      foreach (var recipient in recipients.Where(PandaValidator.IsPandaFormattedPhoneNumber))
      {
         recipient.RemovePhoneFormatParenthesesAndAdditionSign();
      }

      if (recipients.Any(x => x.Any(n => CharsToCheck.Contains(n))))
      {
         recipients = recipients.RemovePhoneFormatParenthesesAndAdditionSign();
      }

      return recipients;
   }
}