using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Communicator.Enums;
using Communicator.Helpers;
using Communicator.Models;
using Communicator.Models.Dexatel;
using Communicator.Models.GeneralResponses;
using Communicator.Models.Twilio;
using Communicator.Options;
using Communicator.Services.Interfaces;

namespace Communicator.Services.Implementations;

internal class SmsService(CommunicatorOptions options, IHttpClientFactory httpClientFactory) : ISmsService
{
   private static JsonSerializerOptions SnakeCaseJsonSerializerOption =>
      new()
      {
         PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
      };

   private string _channel = null!;
   private HttpClient _httpClient = null!;
   private SmsConfiguration _smsConfiguration = null!;

   public async Task<List<GeneralSmsResponse>> SendAsync(SmsMessage smsMessage,
      CancellationToken cancellationToken = default)
   {
      smsMessage = SmsMessageValidator.ValidateAndTransform(smsMessage);

      GetProviderConfigurationAndGenerateHttpClient(smsMessage.Channel);

      return await SendSmsAsync(smsMessage, cancellationToken);
   }

   public async Task<List<GeneralSmsResponse>> SendBulkAsync(List<SmsMessage> smsMessageList,
      CancellationToken cancellationToken = default)
   {
      var generalSmsResponses = new List<GeneralSmsResponse>();

      foreach (var smsMessage in smsMessageList)
      {
         SmsMessageValidator.ValidateAndTransform(smsMessage);

         GetProviderConfigurationAndGenerateHttpClient(smsMessage.Channel);

         generalSmsResponses.AddRange(await SendSmsAsync(smsMessage, cancellationToken));
      }

      return generalSmsResponses;
   }

   private void GetProviderConfigurationAndGenerateHttpClient(string channel)
   {
      _channel = channel;
      _smsConfiguration = GetSmsConfigurationByChannel();
      _httpClient = GenerateProviderHttpClient();
   }

   private SmsConfiguration GetSmsConfigurationByChannel()
   {
      return options.SmsConfigurations?.FirstOrDefault(x => x.Key == _channel)
                    .Value
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

      return SetRequestHeaders(client, provider, properties);
   }

   private HttpClient SetRequestHeaders(HttpClient client,
      SmsProviders provider,
      Dictionary<string, string> properties)
   {
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

   private async Task<List<GeneralSmsResponse>> SendSmsAsync(SmsMessage smsMessage,
      CancellationToken cancellationToken)
   {
      var provider = GetSmsProvider(_smsConfiguration);

      switch (provider)
      {
         case SmsProviders.Dexatel:
            return await SendSmsViaDexatelAsync(smsMessage, cancellationToken);

         case SmsProviders.Twilio:
            return await SendSmsViaTwilioAsync(smsMessage, cancellationToken);

         default:
            throw new InvalidOperationException();
      }
   }

   private async Task<List<GeneralSmsResponse>> SendSmsViaDexatelAsync(SmsMessage smsMessage,
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

      var response = await PostAsyncViaDexatelHttpClient(request, cancellationToken);

      var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

      var responseObject =
         JsonSerializer.Deserialize<DexatelSmsSendResponse>(responseContent, SnakeCaseJsonSerializerOption);

      return responseObject?.Data
                           .Select(x =>
                              new GeneralSmsResponse
                              {
                                 From = x.From,
                                 To = x.To,
                                 Status = x.Status,
                                 CreateDate = x.CreateDate,
                                 UpdateDate = x.UpdateDate,
                                 OuterSmsId = x.Id,
                                 Body = x.Text
                              })
                           .ToList() ?? [];
   }

   private async Task<HttpResponseMessage> PostAsyncViaDexatelHttpClient(DexatelSmsSendRequest request,
      CancellationToken cancellationToken)
   {
      return await _httpClient.PostAsync(
         $"{SmsProviderIntegrations.BaseUrls[_smsConfiguration.Provider]}/v1/messages",
         new StringContent(
            JsonSerializer.Serialize(request, SnakeCaseJsonSerializerOption),
            Encoding.UTF8,
            "application/json"),
         cancellationToken);
   }

   private async Task<List<GeneralSmsResponse>> SendSmsViaTwilioAsync(SmsMessage smsMessage,
      CancellationToken cancellationToken)
   {
      var result = new List<TwilioSmsSendResponse>();

      foreach (var phone in smsMessage.Recipients.MakeDistinct())
      {
         var response = await PostAsyncViaTwilioHttpClient(phone, smsMessage.Message, cancellationToken);

         var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

         var responseObject =
            JsonSerializer.Deserialize<TwilioSmsSendResponse>(responseContent, SnakeCaseJsonSerializerOption);

         result.Add(responseObject ?? new TwilioSmsSendResponse());
      }

      return result.Select(x =>
                      new GeneralSmsResponse
                      {
                         From = x.From,
                         To = x.To,
                         Status = x.Status,
                         CreateDate = x.DateCreated,
                         UpdateDate = x.DateUpdated,
                         OuterSmsId = x.Sid,
                         Body = x.Body
                      })
                   .ToList();
   }

   private async Task<HttpResponseMessage> PostAsyncViaTwilioHttpClient(string phone,
      string smsMessage,
      CancellationToken cancellationToken)
   {
      return await _httpClient.PostAsync(
         $"{SmsProviderIntegrations.BaseUrls[_smsConfiguration.Provider]}/2010-04-01/Accounts/{_smsConfiguration.Properties["SID"]}/Messages.json",
         new FormUrlEncodedContent(new[]
         {
            new KeyValuePair<string, string>("To", phone),
            new KeyValuePair<string, string>("From", _smsConfiguration.From),
            new KeyValuePair<string, string>("Body", smsMessage)
         }),
         cancellationToken);
   }
}