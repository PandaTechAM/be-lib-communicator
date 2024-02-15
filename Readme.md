# 1. PandaTech.Communicator

- [1. EasyRateLimiter](#1-easyratelimiter)
  - [1.1. Introduction](#11-introduction)
  - [1.2. Installation](#12-installation)
  - [1.3. Setup](#13-setup)
    - [1.3.1. Program.cs Example](#131-programcs-example)
    - [1.3.2. Appsettings.json Example](#132-appsettingsjson-example)
  - [1.4. Configuration Options Explained](#14-configuration-options-explained)
  - [1.5. Limitations](#15-limitations)
  - [1.6. License](#16-license)

## 1.1. Introduction
**PandaTech.Communicator** is aimed to send Email and SMS messages to the clients of your service where you use this library.

- **Email:** By given setup it's easy and fast to configure and setup Email providers for later use by different channels.
- **SMS:** By given setup it's easy and fast to configure and setup already integrated SMS providers for later use by different channels.
  - **Dexatel**
  - **Twilio**

This package is ideal for efficient and reliable messaging in any application.

## 1.2. Installation

Install this NuGet library directly from the IDE package installer or by following command: 

`dotnet add package PandaTech.Communicator`

## 1.3. Setup

To incorporate PandaTech.Communicator into your project, you have a primary method to setup in your `Program.cs`:

`builder.AddCommunicator();`

Configuration options can be specified either in `appsettings.json` or directly in `Program.cs`, with the latter taking precedence.

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

For each channel can be setup same provider, but it's recommended to have different sender, as they are dedicated for different purposes.

### 1.3.1. Program.cs Example

In case of using SSL by setting UseSsl = true use port number 456, otherwise use 587 for non SSL connection. 

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
- **SmsFake:** This setup is for fake SMS service to be used which doesn't send real SMS messages and just return `TTask.CompletedTask`.
- **SmsConfigurations:** SMS configurations given by `appsettings.json` or via `builder.AddCommunicator()` options for SMS.
- **EmailFake:** This setup is for fake Email service to be used which doesn't send real SMS messages and just return `TTask.CompletedTask`.
- **EmailConfigurations:** Email configurations given by `appsettings.json` or via `builder.AddCommunicator()` options for Email.

## 1.5. Limitations

PandaTech.Communicator works with all Email providers, but with only with existing integrations for SMS listed above.

## 1.6. License

PandaTech.Communicator is licensed under the MIT License.