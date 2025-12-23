using Communicator.Models;

namespace Communicator.Helpers;

internal static class EmailMessageValidator
{
   internal static void Validate(EmailMessage emailMessage)
   {
      if (emailMessage.Recipients is null || emailMessage.Recipients.Count == 0)
      {
         throw new ArgumentException("Recipients must contain at least one email address.", nameof(emailMessage));
      }

      if (emailMessage.Recipients.Any(e => !ValidationHelper.IsEmail(e)))
      {
         throw new ArgumentException("Recipients contains an invalid email address.", nameof(emailMessage));
      }

      if (emailMessage.Cc.Any(e => !ValidationHelper.IsEmail(e)))
      {
         throw new ArgumentException("Cc contains an invalid email address.", nameof(emailMessage));
      }

      if (emailMessage.Bcc.Any(e => !ValidationHelper.IsEmail(e)))
      {
         throw new ArgumentException("Bcc contains an invalid email address.", nameof(emailMessage));
      }
   }
}