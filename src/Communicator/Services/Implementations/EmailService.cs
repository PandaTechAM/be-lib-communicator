using Communicator.Helpers;
using MimeKit;
using MailKit.Net.Smtp;
using Communicator.Models;
using Communicator.Services.Interfaces;
using Communicator.Options;

namespace Communicator.Services.Implementations;

internal class EmailService(PandaCommunicatorOptions options)
    : IEmailService
{
    private EmailConfiguration _emailConfiguration;
    
    public async Task SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        EmailMessageValidator.Validate(emailMessage);
        
        var message = CreateMimeMessage(emailMessage);
        await SendEmailAsync(message, cancellationToken);
    }

    public async Task SendBulkAsync(List<EmailMessage> emailMessages, CancellationToken cancellationToken = default)
    {
        foreach (var emailMessage in emailMessages)
        {
            EmailMessageValidator.Validate(emailMessage);
        }

        foreach (var message in emailMessages.Select(CreateMimeMessage))
        {
            await SendEmailAsync(message, cancellationToken);
        }
    }

    private MimeMessage CreateMimeMessage(EmailMessage emailMessage)
    {
        _emailConfiguration = GetEmailConfigurationByChannel(emailMessage.Channel);

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_emailConfiguration.SenderEmail));
        message.To.AddRange(emailMessage.Recipients.MakeDistinct().Select(MailboxAddress.Parse));
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

        if (emailMessage.Attachments.Count != 0)
        {
            foreach (var attachment in emailMessage.Attachments)
            {
                builder.Attachments.Add(attachment.FileName, attachment.Content);
            }
        }

        message.Body = builder.ToMessageBody();

        if (emailMessage.Cc.Count != 0)
        {
            message.Cc.AddRange(emailMessage.Cc.MakeDistinct().Select(MailboxAddress.Parse));
        }

        if (emailMessage.Bcc.Count != 0)
        {
            message.Bcc.AddRange(emailMessage.Bcc.MakeDistinct().Select(MailboxAddress.Parse));
        }

        return message;
    }

    private async Task SendEmailAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        using var smtpClient = new SmtpClient();
        smtpClient.Timeout = _emailConfiguration.TimeoutMs;
        
        await smtpClient.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, _emailConfiguration.UseSsl,
            cancellationToken);
        await smtpClient.AuthenticateAsync(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword, cancellationToken);
        await smtpClient.SendAsync(message, cancellationToken);
        await smtpClient.DisconnectAsync(true, cancellationToken);
    }

    private EmailConfiguration GetEmailConfigurationByChannel(string channel)
    {
        return options.EmailConfigurations?.FirstOrDefault(x => x.Channel == channel)
            ?? throw new ArgumentException("No valid provider with given channel");
    }
}