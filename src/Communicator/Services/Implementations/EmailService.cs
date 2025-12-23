using System.Diagnostics;
using System.Net;
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
      DumpNetState(config.SmtpServer);

      var socketOptions = ResolveSocketOptions(config.SmtpPort);

      var sw = Stopwatch.StartNew();
      Console.WriteLine(
         $"[{DateTimeOffset.UtcNow:O}] [EmailService] ConnectAsync {config.SmtpServer}:{config.SmtpPort} socket={socketOptions} timeout={client.Timeout} ...");
      await client.ConnectAsync(config.SmtpServer, config.SmtpPort, socketOptions, ct);
      Console.WriteLine(
         $"[{DateTimeOffset.UtcNow:O}] [EmailService] Connect OK ({sw.ElapsedMilliseconds}ms) secure={client.IsSecure} tls={client.SslProtocol}");

      if (!string.IsNullOrWhiteSpace(config.SmtpUsername))
      {
         Console.WriteLine($"[{DateTimeOffset.UtcNow:O}] [EmailService] AuthenticateAsync ...");
         await client.AuthenticateAsync(config.SmtpUsername, config.SmtpPassword, ct);
         Console.WriteLine($"[{DateTimeOffset.UtcNow:O}] [EmailService] Authenticate OK ({sw.ElapsedMilliseconds}ms)");
      }
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

   private static void DumpNetState(string host)
   {
      AppContext.TryGetSwitch("System.Net.DisableIPv6", out var sw);
      var env = Environment.GetEnvironmentVariable("DOTNET_SYSTEM_NET_DISABLEIPV6");

      Console.WriteLine(
         $"[{DateTimeOffset.UtcNow:O}] [EmailService] DisableIPv6Switch={sw} DOTNET_SYSTEM_NET_DISABLEIPV6='{env}'");

      try
      {
         var addrs = Dns.GetHostAddresses(host);
         Console.WriteLine($"[{DateTimeOffset.UtcNow:O}] [EmailService] DNS {host} => " +
                           string.Join(", ", addrs.Select(a => $"{a}({a.AddressFamily})")));
      }
      catch (Exception ex)
      {
         Console.WriteLine($"[{DateTimeOffset.UtcNow:O}] [EmailService] DNS ERROR {ex.GetType().Name}: {ex.Message}");
      }
   }
}