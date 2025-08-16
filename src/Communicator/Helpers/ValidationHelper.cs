using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Communicator.Helpers;

internal static class ValidationHelper
{
   private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(50);


   private static readonly Regex PandaFormattedPhoneNumber =
      new(@"^\(\d{1,5}\)\d{4,15}$",
         RegexOptions.ExplicitCapture | RegexOptions.Compiled,
         RegexTimeout);


   public static bool IsEmail(string email)
   {
      return MailAddress.TryCreate(email, out _);
   }


   public static bool IsPandaFormattedPhoneNumber(string phoneNumber)
   {
      return PandaFormattedPhoneNumber.IsMatch(phoneNumber);
   }
}