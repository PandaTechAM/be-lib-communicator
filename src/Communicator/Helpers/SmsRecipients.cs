using Communicator.Extensions;

namespace Communicator.Helpers;

internal static class SmsRecipients
{
   private static readonly char[] CharsToCheck = ['+', '(', ')', ' '];

   internal static string Transform(this string recipient)
   {
      if (ValidationHelper.IsPandaFormattedPhoneNumber(recipient) || recipient.Any(c => CharsToCheck.Contains(c)))
      {
         return recipient.RemovePhoneFormatParenthesesAndAdditionSign();
      }

      return recipient;
   }

   internal static List<string> Transform(this List<string> recipients)
   {
      return recipients.Select(r => r.Transform())
                       .ToList();
   }
}