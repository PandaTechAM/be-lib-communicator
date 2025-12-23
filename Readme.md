# PandaTech.Communicator

A lightweight .NET library for sending **Email** and **SMS** through a simple DI-friendly API.

- **Email:** SMTP submission via **MailKit** (works with Gmail, Microsoft 365, Mailgun, SendGrid SMTP, etc.)
- **SMS:** Built-in integrations:
    - **Dexatel**
    - **Twilio**

> Target framework: **.NET 9**

---

## Install

```bash
dotnet add package PandaTech.Communicator
```

---

## Concepts

### Channels

Channels let you configure multiple providers/senders and choose one at runtime per message.

Built-in channel constants:

```csharp
EmailChannels.GeneralSender
EmailChannels.TransactionalSender
EmailChannels.NotificationSender
EmailChannels.MarketingSender
EmailChannels.SupportSender

SmsChannels.GeneralSender
SmsChannels.TransactionalSender
SmsChannels.NotificationSender
SmsChannels.MarketingSender
SmsChannels.SupportSender
```

---

## Setup

You can configure the library in **Program.cs** or via **appsettings.json**. Both are supported.

### Option A: WebApplicationBuilder

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddCommunicator();
```

### Option B: IServiceCollection

```csharp
services.AddCommunicator(configuration);
```

### Fake mode

For local/dev testing you can enable fake services:

- `SmsFake: true` — does not call providers
- `EmailFake: true` — does not send email

---

## Configuration (appsettings.json)

### Minimal example

```json
{
    "Communicator": {
        "SmsFake": true,
        "EmailFake": false,
        "EmailConfigurations": {
            "GeneralSender": {
                "SmtpServer": "smtp.gmail.com",
                "SmtpPort": 587,
                "SmtpUsername": "info@yourdomain.com",
                "SmtpPassword": "APP_PASSWORD",
                "SenderEmail": "info@yourdomain.com",
                "TimeoutMs": 15000
            }
        }
    }
}
```

### Full example (SMS + Email)

```json
{
    "Communicator": {
        "SmsFake": false,
        "SmsConfigurations": {
            "GeneralSender": {
                "Provider": "Dexatel",
                "From": "sender_name",
                "Properties": {
                    "X-Dexatel-Key": "your-key"
                },
                "TimeoutMs": 10000
            },
            "TransactionalSender": {
                "Provider": "Twilio",
                "From": "sender_number",
                "Properties": {
                    "SID": "your-sid",
                    "AUTH_TOKEN": "your-token"
                },
                "TimeoutMs": 10000
            }
        },
        "EmailFake": false,
        "EmailConfigurations": {
            "GeneralSender": {
                "SmtpServer": "smtp.gmail.com",
                "SmtpPort": 587,
                "SmtpUsername": "info@yourdomain.com",
                "SmtpPassword": "APP_PASSWORD",
                "SenderEmail": "info@yourdomain.com",
                "SenderName": "Your App",
                "TimeoutMs": 15000
            }
        }
    }
}
```

### TLS behavior (Email)

Email TLS is inferred by port:

- **465** → implicit TLS (SSL-on-connect)
- **587** → STARTTLS (recommended)

---

## Configuration (Program.cs)

If you prefer to configure everything in code:

```csharp
builder.AddCommunicator(options =>
{
  options.SmsFake = false;
  options.EmailFake = false;

  options.SmsConfigurations = new()
  {
    ["GeneralSender"] = new SmsConfiguration
    {
      Provider = "Dexatel",
      From = "sender_name",
      Properties = new Dictionary<string, string>
      {
        ["X-Dexatel-Key"] = "your-key"
      },
      TimeoutMs = 10000
    }
  };

  options.EmailConfigurations = new()
  {
    ["GeneralSender"] = new EmailConfiguration
    {
      SmtpServer = "smtp.gmail.com",
      SmtpPort = 587,
      SmtpUsername = "info@yourdomain.com",
      SmtpPassword = "APP_PASSWORD",
      SenderEmail = "info@yourdomain.com",
      SenderName = "Your App",
      TimeoutMs = 15000
    }
  };
});
```

---

## Usage

### Services

Inject and use:

- `ISmsService`
- `IEmailService`

```csharp
app.MapPost("/send/sms", async (ISmsService sms, CancellationToken ct) =>
{
  var msg = new SmsMessage
  {
    Recipients = ["+374XXXXXXXX"],
    Message = "Hello",
    Channel = SmsChannels.GeneralSender
  };

  var result = await sms.SendAsync(msg, ct);
  return Results.Ok(result);
});
```

```csharp
app.MapPost("/send/email", async (IEmailService email, CancellationToken ct) =>
{
  var msg = new EmailMessage
  {
    Recipients = ["user@example.com"],
    Subject = "Hello",
    Body = "Hi from PandaTech.Communicator",
    IsBodyHtml = false,
    Channel = EmailChannels.GeneralSender
  };

  var response = await email.SendAsync(msg, ct);
  return Results.Ok(new { response });
});
```

---

## Message models

### SMS

```csharp
public class SmsMessage
{
  public List<string> Recipients { get; set; } = null!;
  public string Message { get; set; } = null!;
  public string Channel { get; set; } = null!;
}
```

`ISmsService` returns `List<GeneralSmsResponse>`:

```csharp
public class GeneralSmsResponse
{
  public string From { get; set; } = null!;
  public string To { get; set; } = null!;
  public string OuterSmsId { get; set; } = null!;
  public string Status { get; set; } = null!;
  public DateTime CreateDate { get; set; }
  public DateTime UpdateDate { get; set; }
  public string Body { get; set; } = null!;
}
```

### Email

```csharp
public class EmailMessage
{
  public List<string> Recipients { get; set; } = null!;
  public string Subject { get; set; } = null!;
  public string Body { get; set; } = null!;
  public List<string> Cc { get; set; } = [];
  public List<string> Bcc { get; set; } = [];
  public List<EmailAttachment> Attachments { get; set; } = [];
  public bool IsBodyHtml { get; set; } = false;
  public string Channel { get; set; } = null!;
}
```

`IEmailService` returns provider response strings:

- `SendAsync` → `string`
- `SendBulkAsync` → `List<string>`

Example response (varies by provider):

```text
2.0.0 OK <server-response-id>
```

---

## Notes & troubleshooting

- If you use Gmail, prefer **587** with an **App Password** (or OAuth2 outside SMTP).
- Some hosting providers block outbound **465**; 587 is typically allowed for SMTP submission.

---

## License

MIT
