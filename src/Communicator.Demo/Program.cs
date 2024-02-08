using Communicator.Extensions;
using Communicator.Models;
using Communicator.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterPandaCommunicator();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/send", async (IEmailService emailService, ISmsService smsService) =>
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
        Message = "Barev erjankutyun",
        Channel = "GeneralSender"
    };
    await smsService.SendAsync(sms);
    return Results.Ok("Email and SMS sent successfully.");
});

app.Run();