using Communicator.Models;

namespace Communicator.Helpers;

internal static class EmailMessageValidator
{
   internal static void Validate(EmailMessage emailMessage)
   {
      if (emailMessage.Recipients.Count == 0)
      {
         throw new ArgumentException("At least one recipient is required", nameof(emailMessage.Recipients));
      }

      if (emailMessage.Recipients.Any(email => !ValidationHelper.IsEmail(email)))
      {
         throw new ArgumentException("Invalid email address", nameof(emailMessage.Recipients));
      }

      if (emailMessage.Cc.Count != 0 && emailMessage.Cc.Any(email => !ValidationHelper.IsEmail(email)))
      {
         throw new ArgumentException("Invalid email address", nameof(emailMessage.Cc));
      }

      if (emailMessage.Bcc.Count != 0 && emailMessage.Bcc.Any(email => !ValidationHelper.IsEmail(email)))
      {
         throw new ArgumentException("Invalid email address", nameof(emailMessage.Bcc));
      }
   }
}