using System.Net;
using System.Net.Sockets;
using Communicator.Helpers;
using Communicator.Models;
using Communicator.Options;
using Communicator.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Communicator.Services.Implementations;

internal sealed class EmailService(CommunicatorOptions options) : IEmailService
{
   public async Task<string> SendAsync(EmailMessage emailMessage, CancellationToken ct = default)
   {
      EmailMessageValidator.Validate(emailMessage);

      var config = GetEmailConfigurationByChannel(emailMessage.Channel);
      var mime = CreateMimeMessage(config, emailMessage);

      using var client = CreateClient(config);

      await ConnectAndAuthAsync(client, config, ct);
      var response = await client.SendAsync(mime, ct);
      await client.DisconnectAsync(true, ct);
      return response;
   }

   public async Task<List<string>> SendBulkAsync(List<EmailMessage> emailMessages,
      CancellationToken ct = default)
   {
      if (emailMessages.Count == 0)
      {
         return [];
      }

      foreach (var t in emailMessages)
      {
         EmailMessageValidator.Validate(t);
      }

      var responses = new string[emailMessages.Count];

      var groups = emailMessages
                   .Select((msg, idx) => (msg, idx))
                   .GroupBy(x => x.msg.Channel);

      foreach (var group in groups)
      {
         var config = GetEmailConfigurationByChannel(group.Key);

         using var client = CreateClient(config);

         await ConnectAndAuthAsync(client, config, ct);

         foreach (var (msg, idx) in group)
         {
            var mime = CreateMimeMessage(config, msg);
            responses[idx] = await client.SendAsync(mime, ct);
         }

         await client.DisconnectAsync(true, ct);
      }

      return responses.ToList();
   }

   private static SmtpClient CreateClient(EmailConfiguration config)
   {
      var client = new SmtpClient
      {
         Timeout = config.TimeoutMs,
         CheckCertificateRevocation = true
      };

      return client;
   }

   private static async Task ConnectAndAuthAsync(SmtpClient client, EmailConfiguration config, CancellationToken ct)
   {
      var options = ResolveSocketOptions(config.SmtpPort);
      client.Timeout = config.TimeoutMs;

      var addresses = await Dns.GetHostAddressesAsync(config.SmtpServer, ct);

      var ordered = addresses
                    .OrderByDescending(a => a.AddressFamily == AddressFamily.InterNetwork) // IPv4 first
                    .ToArray();

      Exception? last = null;

      foreach (var ip in ordered)
      {
         NetworkStream? stream = null;

         try
         {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(config.TimeoutMs);

            var socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(new IPEndPoint(ip, config.SmtpPort), cts.Token);

            stream = new NetworkStream(socket, ownsSocket: true);

            await client.ConnectAsync(stream, config.SmtpServer, config.SmtpPort, options, cts.Token);

            stream = null; // MailKit now owns the connection

            await client.AuthenticateAsync(config.SmtpUsername, config.SmtpPassword, cts.Token);
            return;
         }
         catch (Exception ex)
         {
            last = ex;

            try
            {
               if (stream is not null)
               {
                  await stream.DisposeAsync();
               }
            }
            catch
            {
               // ignored
            }

            try
            {
               if (client.IsConnected)
               {
                  await client.DisconnectAsync(true, CancellationToken.None);
               }
            }
            catch
            {
               // ignored
            }
         }
      }

      throw last ?? new TimeoutException("SMTP connect failed.");
   }

   private static SecureSocketOptions ResolveSocketOptions(int port)
   {
      return port switch
      {
         465 => SecureSocketOptions.SslOnConnect,
         587 => SecureSocketOptions.StartTls,
         _ => SecureSocketOptions.StartTlsWhenAvailable
      };
   }

   private static MimeMessage CreateMimeMessage(EmailConfiguration config, EmailMessage emailMessage)
   {
      var senderEmail = !string.IsNullOrWhiteSpace(config.SenderEmail) ? config.SenderEmail : config.SmtpUsername;

      var message = new MimeMessage();

      if (!string.IsNullOrWhiteSpace(config.SenderName) && !string.IsNullOrWhiteSpace(senderEmail))
      {
         message.From.Add(new MailboxAddress(config.SenderName, senderEmail));
      }
      else if (!string.IsNullOrWhiteSpace(senderEmail))
      {
         message.From.Add(MailboxAddress.Parse(senderEmail));
      }
      else
      {
         throw new InvalidOperationException("SenderEmail (or SmtpUsername fallback) is required.");
      }

      message.To.AddRange(ParseDistinct(emailMessage.Recipients));
      message.Subject = emailMessage.Subject;

      var builder = new BodyBuilder();

      if (emailMessage.IsBodyHtml)
      {
         builder.HtmlBody = emailMessage.Body;
      }
      else
      {
         builder.TextBody = emailMessage.Body;
      }

      if (emailMessage.Attachments is { Count: > 0 })
      {
         foreach (var a in emailMessage.Attachments)
         {
            builder.Attachments.Add(a.FileName, a.Content);
         }
      }

      if (emailMessage.Cc is { Count: > 0 })
      {
         message.Cc.AddRange(ParseDistinct(emailMessage.Cc));
      }

      if (emailMessage.Bcc is { Count: > 0 })
      {
         message.Bcc.AddRange(ParseDistinct(emailMessage.Bcc));
      }

      message.Body = builder.ToMessageBody();
      return message;
   }

   private static IEnumerable<InternetAddress> ParseDistinct(IEnumerable<string> emails)
   {
      return emails
             .Where(e => !string.IsNullOrWhiteSpace(e))
             .Distinct(StringComparer.OrdinalIgnoreCase)
             .Select(MailboxAddress.Parse);
   }

   private EmailConfiguration GetEmailConfigurationByChannel(string channel)
   {
      var config = options.EmailConfigurations?.FirstOrDefault(x => x.Key == channel)
                          .Value;

      return config ??
             throw new ArgumentException("No valid email configuration for the given channel.", nameof(channel));
   }
}