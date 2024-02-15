using System.Net.Http.Headers;
using System.Text;
using Communicator.Enums;
using Communicator.Helpers;
using Communicator.Models;
using Communicator.Models.Dexatel;
using Communicator.Models.Twilio;
using Communicator.Options;
using Communicator.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Communicator.Services.Implementations;

internal class SmsService(CommunicatorOptions options, IHttpClientFactory httpClientFactory) : ISmsService
{
    private SmsConfiguration _smsConfiguration = null!;
    private HttpClient _httpClient = null!;
    private string _channel = null!;

    public async Task SendAsync(SmsMessage smsMessage, CancellationToken cancellationToken = default)
    {
        smsMessage = ValidateAndTransformRecipients(smsMessage);
        SmsMessageValidator.Validate(smsMessage);

        GetProviderConfigurationAndGenerateHttpClient(smsMessage.Channel);

        await SendSmsAsync(smsMessage, cancellationToken);
    }

    public async Task SendBulkAsync(List<SmsMessage> smsMessageList, CancellationToken cancellationToken = default)
    {
        smsMessageList = ValidateAndTransformRecipients(smsMessageList);

        foreach (var smsMessage in smsMessageList)
        {
            SmsMessageValidator.Validate(smsMessage);

            GetProviderConfigurationAndGenerateHttpClient(smsMessage.Channel);

            await SendSmsAsync(smsMessage, cancellationToken);
        }
    }

    private static SmsMessage ValidateAndTransformRecipients(SmsMessage smsMessage)
    {
        smsMessage.Recipients = smsMessage.Recipients.ValidateAndTransform();

        return smsMessage;
    }

    private static List<SmsMessage> ValidateAndTransformRecipients(List<SmsMessage> smsMessageList)
    {
        foreach (var smsMessage in smsMessageList)
        {
            smsMessage.Recipients = smsMessage.Recipients.ValidateAndTransform();
        }

        return smsMessageList;
    }

    private void GetProviderConfigurationAndGenerateHttpClient(string channel)
    {
        _channel = channel;
        _smsConfiguration = GetSmsConfigurationByChannel();
        _httpClient = GenerateProviderHttpClient();
    }

    private SmsConfiguration GetSmsConfigurationByChannel()
    {
        return options.SmsConfigurations?.FirstOrDefault(x => x.Key == _channel).Value
               ?? throw new ArgumentException("No valid provider with given channel");
    }

    private static SmsProviders GetSmsProvider(SmsConfiguration smsConfiguration)
    {
        Enum.TryParse(typeof(SmsProviders), smsConfiguration.Provider, out var providerValue);

        if (providerValue is null)
        {
            throw new Exception("Wrong provider");
        }

        var provider = (SmsProviders)providerValue;

        return provider;
    }

    private HttpClient GenerateProviderHttpClient()
    {
        var client = httpClientFactory.CreateClient(_channel);

        var provider = GetSmsProvider(_smsConfiguration);

        var properties = _smsConfiguration.Properties;

        switch (provider)
        {
            case SmsProviders.Dexatel:
                client.DefaultRequestHeaders.Add("X-Dexatel-Key", properties["X-Dexatel-Key"]);
                break;

            case SmsProviders.Twilio:
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        Encoding.ASCII.GetBytes($"{properties["SID"]}:{properties["AUTH_TOKEN"]}")));
                break;
        }

        return client;
    }

    private async Task SendSmsAsync(SmsMessage smsMessage, CancellationToken cancellationToken)
    {
        var provider = GetSmsProvider(_smsConfiguration);

        switch (provider)
        {
            case SmsProviders.Dexatel:
                await SendSmsViaDexatelAsync(smsMessage, cancellationToken);
                break;

            case SmsProviders.Twilio:
                await SendSmsViaTwilioAsync(smsMessage, cancellationToken);
                break;
        }
    }

    private async Task<List<SendSmsResponse>> SendSmsViaDexatelAsync(SmsMessage smsMessage,
        CancellationToken cancellationToken)
    {
        var request = new DexatelSmsSendRequest
        {
            Data = new DexatelSmsSendRequestData
            {
                From = _smsConfiguration.From,
                To = smsMessage.Recipients.MakeDistinct(),
                Text = smsMessage.Message,
                Channel = "SMS"
            }
        };

        var response = await _httpClient.PostAsync(
            $"{SmsProviderIntegrations.BaseUrls[_smsConfiguration.Provider]}/v1/messages",
            new StringContent(
                JsonConvert.SerializeObject(request, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    }
                }),
                Encoding.UTF8,
                "application/json"), cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        var responseObject = JsonConvert.DeserializeObject<DexatelSmsSendResponse>(responseContent,
            new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            });

        return responseObject?.Data.Select(d => new SendSmsResponse
        {
            To = d.To, OuterSmsId = d.Id, ProviderSentDate = d.CreateDate, Status = d.Status
        }).ToList() ?? [];
    }

    private async Task<List<SendSmsResponse>> SendSmsViaTwilioAsync(SmsMessage smsMessage,
        CancellationToken cancellationToken)
    {
        var result = new List<SendSmsResponse>();

        foreach (var phone in smsMessage.Recipients.MakeDistinct())
        {
            var response = await _httpClient.PostAsync(
                $"{SmsProviderIntegrations.BaseUrls[_smsConfiguration.Provider]}/2010-04-01/Accounts/{_smsConfiguration.Properties["SID"]}/Messages.json",
                new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("To", phone),
                    new KeyValuePair<string, string>("From", _smsConfiguration.From),
                    new KeyValuePair<string, string>("Body", smsMessage.Message)
                }), cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            var responseObject = JsonConvert.DeserializeObject<TwilioSmsSendResponse>(responseContent,
                new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    }
                });

            if (responseObject != null)
            {
                result.Add(new SendSmsResponse
                {
                    To = responseObject.To,
                    OuterSmsId = responseObject.Sid,
                    ProviderSentDate = responseObject.DateCreated,
                    Status = responseObject.Status,
                });
            }
            else
            {
                result.Add(new SendSmsResponse());
            }
        }

        return result;
    }
}