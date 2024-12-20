﻿using Communicator.Helpers;
using Communicator.Models;
using Communicator.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Communicator.Services.Implementations;

internal class FakeEmailService(ILogger<FakeEmailService> logger) : IEmailService
{
   public Task<string> SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
   {
      EmailMessageValidator.Validate(emailMessage);

      logger.LogCritical("Email sent to {Recipient}\n Email subject is {Subject} \n Email body is {Body}",
         emailMessage.Recipients,
         emailMessage.Subject,
         emailMessage.Body);

      return Task.FromResult("2.0.0 OK");
   }

   public Task<List<string>> SendBulkAsync(List<EmailMessage> emailMessages,
      CancellationToken cancellationToken = default)
   {
      foreach (var emailMessage in emailMessages)
      {
         EmailMessageValidator.Validate(emailMessage);

         logger.LogCritical("Email sent to {Recipient} \n Email subject is {Subject} \n Email body is {Body}",
            emailMessage.Recipients,
            emailMessage.Subject,
            emailMessage.Body);
      }

      return Task.FromResult(new List<string>
      {
         "2.0.0 OK"
      });
   }
}