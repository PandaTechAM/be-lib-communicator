using Communicator.Extensions;

namespace Communicator.Helpers;

internal static class SmsRecipients
{
   private static readonly char[] CharsToCheck = ['+', '(', ')', ' '];

   internal static string Transform(this string recipient)
   {
      if (ValidationHelper.IsPandaFormattedPhoneNumber(recipient)
          || recipient.Any(c => CharsToCheck.Contains(c)))
      {
         recipient = recipient.RemovePhoneFormatParenthesesAndAdditionSign();
      }

      return recipient;
   }

   internal static List<string> Transform(this List<string> recipients)
   {
      foreach (var recipient in recipients.Where(ValidationHelper.IsPandaFormattedPhoneNumber))
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