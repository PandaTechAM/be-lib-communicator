using Communicator.Demo.MessageTemplates;
using Communicator.Enums;
using Communicator.Extensions;
using Communicator.Models;
using Communicator.Options;
using Communicator.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddCommunicator(options =>
{
    options.SmsFake = false;
    options.SmsConfigurations = new Dictionary<string, SmsConfiguration>
    {
        {
            "GeneralSender", new SmsConfiguration
            {
                Provider = "Dexatel",
                From = "sender_name",
                Properties = new Dictionary<string, string>
                {
                    { "X-Dexatel-Key", "key" }
                },
                TimeoutMs = 10000
            }
        },
        {
            "TransactionalSender", new SmsConfiguration
            {
                Provider = "Twilio",
                From = "sender_number",
                Properties = new Dictionary<string, string>
                {
                    { "SID", "key" },
                    { "AUTH_TOKEN", "token" }
                },
                TimeoutMs = 10000
            }
        }
    };
    options.EmailFake = false;
    options.EmailConfigurations = new Dictionary<string, EmailConfiguration>
    {
        {
            "GeneralSender2", new EmailConfiguration
            {
                SmtpServer = "smtp.test.com",
                SmtpPort = 465, // 587
                SmtpUsername = "test",
                SmtpPassword = "test123",
                SenderEmail = "test@test.com",
                UseSsl = true, // false
                TimeoutMs = 10000
            }
        },
        {
            "TransactionalSender", new EmailConfiguration
            {
                SmtpServer = "smtp.gmail.com",
                SmtpPort = 465, // 587
                SmtpUsername = "vazgen",
                SmtpPassword = "vazgen123",
                SenderEmail = "vazgencho@gmail.com",
                UseSsl = true, // false
                TimeoutMs = 10000
            }
        }
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/send/email-and-sms", async (IEmailService emailService, ISmsService smsService) =>
{
    var email = new EmailMessage
    {
        Recipients = ["a@a.com"],
        Subject = "Some subject",
        Body = "Some body",
        IsBodyHtml = false,
        Channel = EmailChannels.GeneralSender
    };
    await emailService.SendAsync(email);

    var sms = new SmsMessage
    {
        Recipients = ["+37493910593"],
        Message = "123456",
        Channel = SmsChannels.GeneralSender
    };
    await smsService.SendAsync(sms);
    return Results.Ok("Email and SMS sent successfully.");
});

app.MapGet("/send/email-and-sms/multiple-recipients", async (IEmailService emailService, ISmsService smsService) =>
{
    var email = new EmailMessage
    {
        Recipients = ["a@a.com", "a@a.com", "a@a.com", "a@a.com", "b@b.com"],
        Subject = "Some subject",
        Body = "Some body",
        IsBodyHtml = false,
        Channel = EmailChannels.GeneralSender
    };
    await emailService.SendAsync(email);

    var sms = new SmsMessage
    {
        Recipients = ["+37493910593", "+37493910593", "+37493910594", "+37493910595"],
        Message = "Barev erjankutyun",
        Channel = SmsChannels.GeneralSender
    };
    await smsService.SendAsync(sms);
    return Results.Ok("Email and SMS sent successfully.");
});

app.MapGet("/send/email/html-body", async (IEmailService emailService) =>
{
    var email = new EmailMessage
    {
        Recipients = ["a@a.com"],
        Subject = "Some subject",
        Body = EmailTemplates.AddEmailAddressTemplate("Test", "Test", "https://www.google.com/"),
        IsBodyHtml = true,
        Channel = EmailChannels.GeneralSender
    };
    await emailService.SendAsync(email);
    return Results.Ok("Email sent successfully.");
});

app.MapGet("/send/email/html-body/with-response", async (IEmailService emailService) =>
{
    var email = new EmailMessage
    {
        Recipients = ["a@a.com"],
        Subject = "Some subject",
        Body = EmailTemplates.AddEmailAddressTemplate("Test", "Test", "https://www.google.com/"),
        IsBodyHtml = true,
        Channel = EmailChannels.GeneralSender
    };
    var response = await emailService.SendAsync(email);
    return Results.Ok(response);
});

app.MapGet("/send/email/multiple-recipients/html-body", async (IEmailService emailService) =>
{
    var email = new EmailMessage
    {
        Recipients = ["a@a.com", "b@b.com", "c@c.com"],
        Subject = "Some subject",
        Body = EmailTemplates.AddEmailAddressTemplate("Test", "Test", "https://www.google.com/"),
        IsBodyHtml = true,
        Channel = EmailChannels.GeneralSender
    };
    await emailService.SendAsync(email);
    return Results.Ok("Email sent successfully.");
});

app.MapGet("/send/sms/multiple-recipients", async (ISmsService smsService) =>
{
    var sms = new SmsMessage
    {
        Recipients = ["+37493910593", "+37493910593", "+37493910594", "+37493910595"],
        Message = "Barev erjankutyun",
        Channel = SmsChannels.GeneralSender
    };
    await smsService.SendAsync(sms);
    return Results.Ok("SMS sent successfully.");
});

app.MapGet("/send/sms/twilio", async (ISmsService smsService) =>
{
    var sms = new SmsMessage
    {
        Recipients = ["+37495988247"],
        Message = "Barev erjankutyun",
        Channel = SmsChannels.TransactionalSender
    };
    await smsService.SendAsync(sms);
    return Results.Ok("SMS sent successfully.");
});

app.MapGet("/send/bulk/email/html-body", async (IEmailService emailService) =>
{
    var emailMessageList = new List<EmailMessage>();
    for (var i = 0; i < 5; i++)
    {
        var random = new Random();
        emailMessageList.Add(new EmailMessage
        {
            Recipients = ["a1@a.com", "a2@a.com", "a5@a.com", $"a{random.Next(1, 5)}@a.com"],
            Subject = "Some subject",
            Body = EmailTemplates.AddEmailAddressTemplate("Test", "Test", "https://www.google.com/"),
            IsBodyHtml = true,
            Channel = EmailChannels.GeneralSender
        });
    }

    await emailService.SendBulkAsync(emailMessageList);
    return Results.Ok("Emails sent successfully.");
});

app.MapGet("/send/bulk/sms/multiple-recipients", async (ISmsService smsService) =>
{
    var smsMessageList = new List<SmsMessage>();
    for (var i = 1; i <= 5; i++)
    {
        smsMessageList.Add(new SmsMessage
        {
            Recipients = ["+37493910593", $"+3749391059{i}"],
            Message = "Barev erjankutyun",
            Channel = SmsChannels.GeneralSender
        });
    }

    await smsService.SendBulkAsync(smsMessageList);
    return Results.Ok("SMS messages sent successfully.");
});

app.Run();