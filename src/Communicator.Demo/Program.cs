using Communicator.Demo.MessageTemplates;
using Communicator.Extensions;
using Communicator.Models;
using Communicator.Options;
using Communicator.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddPandaCommunicator(options =>
{
    options.SmsFake = false;
    options.SmsConfigurations =
    [
        new SmsConfiguration
        {
            Provider = "Dexatel",
            BaseUrl = "https://api.dexatel.com",
            From = "sender_name",
            Properties = new()
            {
                { "X-Dexatel-Key", "key" }
            },
            TimeoutMs = 10000,
            Channel = "GeneralSender"
        },

        new SmsConfiguration
        {
            Provider = "Twilio",
            BaseUrl = "https://api.twilio.com",
            From = "sender_number",
            Properties = new()
            {
                { "SID", "key" },
                { "AUTH_TOKEN", "token" }
            },
            TimeoutMs = 10000,
            Channel = "TransactionalSender"
        }
    ];
    options.EmailFake = false;
    options.EmailConfigurations =
    [
        new EmailConfiguration
        {
            SmtpServer = "smtp.test.com",
            SmtpPort = 465, // 587
            SmtpUsername = "test",
            SmtpPassword = "test123",
            SenderEmail = "test@test.com",
            UseSsl = true, // false
            TimeoutMs = 10000,
            Channel = "GeneralSender"
        },

        new EmailConfiguration
        {
            SmtpServer = "smtp.gmail.com",
            SmtpPort = 465, // 587
            SmtpUsername = "vazgen",
            SmtpPassword = "vazgen123",
            SenderEmail = "vazgencho@gmail.com",
            UseSsl = true, // false
            TimeoutMs = 10000,
            Channel = "TransactionalSender"
        }
    ];
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
        Channel = "GeneralSender"
    };
    await emailService.SendAsync(email);

    var sms = new SmsMessage
    {
        Recipients = ["+37493910593"],
        Message = SmsTemplates.OtpCodeVerificationRequestMessage("123456"),
        Channel = "GeneralSender"
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
        Channel = "GeneralSender"
    };
    await emailService.SendAsync(email);

    var sms = new SmsMessage
    {
        Recipients = ["+37493910593", "+37493910593", "+37493910594", "+37493910595"],
        Message = "Barev erjankutyun",
        Channel = "GeneralSender"
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
        Channel = "GeneralSender"
    };
    await emailService.SendAsync(email);
    return Results.Ok("Email sent successfully.");
});

app.MapGet("/send/email/multiple-recipients/html-body", async (IEmailService emailService) =>
{
    var email = new EmailMessage
    {
        Recipients = ["a@a.com", "b@b.com", "c@c.com"],
        Subject = "Some subject",
        Body = EmailTemplates.AddEmailAddressTemplate("Test", "Test", "https://www.google.com/"),
        IsBodyHtml = true,
        Channel = "GeneralSender"
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
        Channel = "GeneralSender"
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
        Channel = "TransactionalSender"
    };
    await smsService.SendAsync(sms);
    return Results.Ok("SMS sent successfully.");
});

app.Run();