# 1. PandaTech.Communicator

- [1. EasyRateLimiter](#1-easyratelimiter)
    - [1.1. Introduction](#11-introduction)
    - [1.2. Installation](#12-installation)
    - [1.3. Setup](#13-setup)
        - [1.3.1. Program.cs Example](#131-programcs-example)
            - [1.3.1.1. Using WebApplicationBuilder](#1311-using-webapplicationbuilder)
            - [1.3.1.2. Using IServiceCollection](#1312-using-iservicecollection)
        - [1.3.2. Appsettings.json Example](#132-appsettingsjson-example)
    - [1.4. Configuration Options Explained](#14-configuration-options-explained)
    - [1.5. Usage](#15-usage)
        - [1.5.1. Send SMS message](#151-send-sms-message)
        - [1.5.2. Send Email message](#152-send-email-message)
    - [1.6. Limitations](#16-limitations)
    - [1.7. License](#17-license)

## 1.1. Introduction

**PandaTech.Communicator** is aimed to send Email and SMS messages to the clients of your service where you use this
library.

- **Email:** By given setup it's easy and fast to configure and setup Email providers for later use by different
  channels.
- **SMS:** By given setup it's easy and fast to configure and setup already integrated SMS providers for later use by
  different channels.
    - **Dexatel**
    - **Twilio**

This package is ideal for efficient and reliable messaging in any application.

## 1.2. Installation

Install this NuGet library directly from the IDE package installer or by following command:

`dotnet add package PandaTech.Communicator`

## 1.3. Setup

To incorporate PandaTech.Communicator into your project, you have 2 primary methods to setup in your `Program.cs`:

The first method directly using `WebApplicationBuilder` from which `IConfiguration` is directly used. But for the second
method builder is not accessible, so we pass `IConfiguration` into it as parameter.

- `builder.AddCommunicator();`
- `services.AddCommunicator(configuration);`

Configuration options can be specified either in `appsettings.json` or directly in `Program.cs`, with the latter taking
precedence.

There are several supported channels which must be kept during setup:

```csharp
EmailChannels
{
    "GeneralSender",
    "TransactionalSender",
    "NotificationSender",
    "MarketingSender",
    "SupportSender"
};
    
SmsChannels
{
    "GeneralSender",
    "TransactionalSender",
    "NotificationSender",
    "MarketingSender",
    "SupportSender"
};
```

For each channel can be setup same provider, but it's recommended to have different sender, as they are dedicated for
different purposes.

### 1.3.1. Program.cs Example

In case of using SSL by setting UseSsl = true use port number 456, otherwise use 587 for non SSL connection.

#### 1.3.1.1. Using `WebApplicationBuilder`

```csharp
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
            "GeneralSender", new EmailConfiguration
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

```

#### 1.3.1.2. Using `IServiceCollection`

```csharp
services.AddCommunicator(configuration, options =>
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
            "GeneralSender", new EmailConfiguration
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

```

### 1.3.2. Appsettings.json Example

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Communicator": {
    "SmsFake": false,
    "SmsConfigurations": {
      "GeneralSender": {
        "Provider": "Dexatel",
        "From": "sender_name",
        "Properties": {
          "X-Dexatel-Key": "key"
        },
        "TimeoutMs": "10000"
      },
      "TransactionalSender": {
        "Provider": "Twilio",
        "From": "sender_number",
        "Properties": {
          "SID": "key",
          "AUTH_TOKEN": "token"
        },
        "TimeoutMs": "10000"
      }
    },
    "EmailFake": false,
    "EmailConfigurations": {
      "GeneralSender": {
        "SmtpServer": "smtp.gmail.com",
        "SmtpPort": 465, // 587
        "SmtpUsername": "vazgen",
        "SmtpPassword": "vazgen123",
        "SenderEmail": "vazgencho@gmail.com",
        "UseSsl": true, // false
        "TimeoutMs": "10000"
      },
      "TransactionalSender": {
        "SmtpServer": "smtp.gmail.com",
        "SmtpPort": 465, // 587
        "SmtpUsername": "vazgen",
        "SmtpPassword": "vazgen123",
        "SenderEmail": "vazgencho@gmail.com",
        "UseSsl": true, // false
        "TimeoutMs": "10000"
      }
    }
  }
}
```

The configuration options in `appsettings.json` and `program.cs` (priority) are identical.

## 1.4. Configuration Options Explained

- **Communicator:** Global settings for PandaTech.Communicator
- **SmsFake:** This setup is for fake SMS service to be used which doesn't send real SMS messages and just return
  `TTask.CompletedTask`.
- **SmsConfigurations:** SMS configurations given by `appsettings.json` or via `builder.AddCommunicator()` options for
  SMS.
- **EmailFake:** This setup is for fake Email service to be used which doesn't send real SMS messages and just return
  `TTask.CompletedTask`.
- **EmailConfigurations:** Email configurations given by `appsettings.json` or via `builder.AddCommunicator()` options
  for Email.

## 1.5. Usage

In order to use the library, you need to generate `SmsMessage` or `EmailMessage` and use one of the interfaces mentioned
above for the service you need to use.

Both of them support multiple recipients.

The structure of the messages are shown below.

```csharp
public class SmsMessage
{
    public List<string> Recipients { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string Channel { get; set; } = null!;
}

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

Channel is set by `EmailChannels` or by `SmsChannels` classes with constant values.

There are 2 interfaces `ISmsService` and `IEmailService` responsible for individual cases SMS or Email.
Methods for sending SMS/Email messages are:

- **SendAsync:**
- **SendBulkAsync:**

The structure of the service interfaces are shown below.

```csharp
public interface ISmsService
{
    Task<List<GeneralSmsResponse>> SendAsync(SmsMessage smsMessage, CancellationToken cancellationToken = default);
    Task<List<GeneralSmsResponse>> SendBulkAsync(List<SmsMessage> smsMessageList, CancellationToken cancellationToken = default);
}

public interface IEmailService
{
    Task<GeneralEmailResponse> SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
    Task<List<GeneralEmailResponse>> SendBulkAsync(List<EmailMessage> emailMessages, CancellationToken cancellationToken = default);
}
```

## 1.5.1. Send SMS message

```csharp
var sms = new SmsMessage
{
    Recipients = ["+37493910593"],
    Message = "123456",
    Channel = SmsChannels.GeneralSender
};
await smsService.SendAsync(sms);
```

Sms service returns general response which includes general properties in already integrated services.

Both methods return `List<GeneralSmsResponse>` when you use them while sending sms.
If you set a variable to the call, you will be able to use returned response.

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

## 1.5.2. Send Email message

```csharp
var email = new EmailMessage
{
    Recipients = ["a@a.com"],
    Subject = "Some subject",
    Body = "Some body",
    IsBodyHtml = false,
    Channel = EmailChannels.GeneralSender
};
await emailService.SendAsync(email);
```

Both methods return response (SendAsync - `string`; SendBulkAsync - `List<string>`) when you use them while sending
email.
If you set a variable to the call, you will be able to use returned response. Response structure varies on different
email providers, so you can create your ouw return type and map returned string into it.

```text
2.0.0 OK 8ONXSST18NU4.DSCFVS8PQ0Q13@test AM0PR08MB5346.eurprd08.prod.outlook.com
```

## 1.6. Limitations

PandaTech.Communicator works with all Email providers, but with only with existing integrations for SMS listed above.

## 1.7. License

PandaTech.Communicator is licensed under the MIT License.