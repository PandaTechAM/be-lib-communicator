# Pandatech.Communicator

Send email via SMTP and SMS via Dexatel or Twilio through a DI-friendly, multi-channel API. Supports both
`appsettings.json` and programmatic configuration, named channels per transport, and a fake mode for local development.

Targets **`net8.0`**, **`net9.0`**, and **`net10.0`**.

---

## Table of Contents

1. [Features](#features)
2. [Installation](#installation)
3. [Registration](#registration)
4. [Configuration](#configuration)
5. [Channels](#channels)
6. [Sending Email](#sending-email)
7. [Sending SMS](#sending-sms)
8. [Fake Mode](#fake-mode)

---

## Features

- Email over SMTP using MailKit — TLS negotiation, optional authentication, CC/BCC, attachments, HTML body
- SMS via Dexatel and Twilio with a unified `GeneralSmsResponse`
- Named channels — configure multiple senders per transport (e.g. `TransactionalSender`, `MarketingSender`) and pick
  the right one per message
- Validation on every send call — recipients, addresses, and phone numbers are checked before any network call
- Fake mode — logs messages at `Critical` instead of sending; zero external calls in development or test environments
- Supports both `WebApplicationBuilder` and plain `IServiceCollection` registration

---

## Installation

```bash
dotnet add package Pandatech.Communicator
```

---

## Registration

### WebApplicationBuilder

```csharp
builder.AddCommunicator();           // reads from appsettings.json "Communicator" section
// or
builder.AddCommunicator(options => { /* programmatic setup */ });
```

### IServiceCollection

```csharp
services.AddCommunicator(configuration);
// or
services.AddCommunicator(configuration, options => { /* programmatic setup */ });
```

Both register `IEmailService` and `ISmsService` into DI as scoped services.

---

## Configuration

### appsettings.json

```json
{
  "Communicator": {
    "EmailFake": false,
    "SmsFake": false,
    "EmailConfigurations": {
      "TransactionalSender": {
        "SmtpServer": "smtp.gmail.com",
        "SmtpPort": 587,
        "SmtpUsername": "you@example.com",
        "SmtpPassword": "app-password",
        "SenderEmail": "no-reply@example.com",
        "SenderName": "My App",
        "TimeoutMs": 10000
      },
      "MarketingSender": {
        "SmtpServer": "smtp.sendgrid.net",
        "SmtpPort": 587,
        "SmtpUsername": "apikey",
        "SmtpPassword": "SG.xxx",
        "SenderEmail": "marketing@example.com",
        "TimeoutMs": 10000
      }
    },
    "SmsConfigurations": {
      "TransactionalSender": {
        "Provider": "Dexatel",
        "From": "MyApp",
        "TimeoutMs": 10000,
        "Properties": {
          "X-Dexatel-Key": "your-dexatel-api-key"
        }
      },
      "NotificationSender": {
        "Provider": "Twilio",
        "From": "+15550001234",
        "TimeoutMs": 10000,
        "Properties": {
          "SID": "ACxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
          "AUTH_TOKEN": "your-auth-token"
        }
      }
    }
  }
}
```

### Programmatic

```csharp
builder.AddCommunicator(options =>
{
    options.EmailConfigurations = new Dictionary<string, EmailConfiguration>
    {
        ["TransactionalSender"] = new()
        {
            SmtpServer   = "smtp.gmail.com",
            SmtpPort     = 587,
            SmtpUsername = "you@example.com",
            SmtpPassword = "app-password",
            SenderEmail  = "no-reply@example.com",
            SenderName   = "My App"
        }
    };

    options.SmsConfigurations = new Dictionary<string, SmsConfiguration>
    {
        ["TransactionalSender"] = new()
        {
            Provider = "Dexatel",
            From     = "MyApp",
            Properties = new() { ["X-Dexatel-Key"] = "your-key" }
        }
    };
});
```

---

## Channels

Channel names are validated at startup against a fixed set of supported names:

```
GeneralSender
TransactionalSender
NotificationSender
MarketingSender
SupportSender
```

Each channel maps to exactly one configuration entry. The `Channel` property on `EmailMessage` and `SmsMessage`
selects which configuration is used for that send call.

---

## Sending Email

```csharp
public class NotificationService(IEmailService emailService)
{
    public async Task SendWelcomeAsync(string userEmail, CancellationToken ct)
    {
        var message = new EmailMessage
        {
            Recipients  = [userEmail],
            Subject     = "Welcome!",
            Body        = "<h1>Thanks for signing up.</h1>",
            IsBodyHtml  = true,
            Channel     = EmailChannels.TransactionalSender,
            Cc          = ["manager@example.com"],
            Attachments = [new EmailAttachment("terms.pdf", pdfBytes)]
        };

        var response = await emailService.SendAsync(message, ct);
    }
}
```

`SendBulkAsync` accepts a list of messages, opens one SMTP connection per channel, and sends all messages for that
channel on the same connection before moving to the next.

---

## Sending SMS

```csharp
public class OtpService(ISmsService smsService)
{
    public async Task SendOtpAsync(string phoneNumber, string code, CancellationToken ct)
    {
        var message = new SmsMessage
        {
            Recipients = [phoneNumber],
            Message    = $"Your code is {code}",
            Channel    = SmsChannels.TransactionalSender
        };

        var responses = await smsService.SendAsync(message, ct);
    }
}
```

Phone numbers are normalized before sending — `+`, `(`, `)`, and spaces are stripped, and Panda-formatted numbers
like `(374)91123456` are handled automatically.

### Provider-specific Properties

| Provider  | Required Properties              |
|-----------|----------------------------------|
| Dexatel   | `X-Dexatel-Key`                  |
| Twilio    | `SID`, `AUTH_TOKEN`              |

---

## Fake Mode

Set `EmailFake: true` or `SmsFake: true` (or both) to replace the real services with fake implementations that log
at `Critical` instead of making any network calls. The same validation still runs.

```json
{
  "Communicator": {
    "EmailFake": true,
    "SmsFake": true
  }
}
```

Useful in local development and CI environments where you want to confirm messages are being sent without delivering
them.

---

## License

MIT