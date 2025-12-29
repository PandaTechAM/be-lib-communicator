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

internal sealed class SmsService(CommunicatorOptions options, IHttpClientFactory httpClientFactory) : ISmsService
{
   private static readonly JsonSerializerOptions SnakeCaseJson =
      new()
      {
         PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
      };

   private static readonly JsonSerializerOptions CamelCaseJson =
      new()
      {
         PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      };

   public async Task<List<GeneralSmsResponse>> SendAsync(SmsMessage smsMessage, CancellationToken ct = default)
   {
      smsMessage = SmsMessageValidator.ValidateAndTransform(smsMessage);

      var config = GetSmsConfigurationByChannel(smsMessage.Channel);
      using var client = CreateConfiguredClient(smsMessage.Channel, config);

      return await SendSmsAsync(client, config, smsMessage, ct);
   }

   public async Task<List<GeneralSmsResponse>> SendBulkAsync(List<SmsMessage> smsMessageList,
      CancellationToken ct = default)
   {
      var all = new List<GeneralSmsResponse>();

      foreach (var smsMessage in smsMessageList)
      {
         SmsMessageValidator.ValidateAndTransform(smsMessage);

         var config = GetSmsConfigurationByChannel(smsMessage.Channel);
         using var client = CreateConfiguredClient(smsMessage.Channel, config);

         var responses = await SendSmsAsync(client, config, smsMessage, ct);
         all.AddRange(responses);
      }

      return all;
   }

   private SmsConfiguration GetSmsConfigurationByChannel(string channel)
   {
      var config = options.SmsConfigurations?.FirstOrDefault(x => x.Key == channel)
                          .Value;

      return config ?? throw new ArgumentException("No valid provider with given channel.", nameof(channel));
   }

   private static SmsProviders GetSmsProvider(SmsConfiguration smsConfiguration)
   {
      return !Enum.TryParse<SmsProviders>(smsConfiguration.Provider, true, out var provider)
         ? throw new ArgumentException($"Wrong provider: '{smsConfiguration.Provider}'.", nameof(smsConfiguration))
         : provider;
   }

   private HttpClient CreateConfiguredClient(string channel, SmsConfiguration config)
   {
      var client = httpClientFactory.CreateClient(channel);

      var provider = GetSmsProvider(config);
      var props = config.Properties;

      if (provider == SmsProviders.Dexatel)
      {
         if (!props.TryGetValue("X-Dexatel-Key", out var key) || string.IsNullOrWhiteSpace(key))
         {
            throw new ArgumentException("Missing X-Dexatel-Key in SmsConfiguration.Properties.");
         }

         client.DefaultRequestHeaders.TryAddWithoutValidation("X-Dexatel-Key", key);
      }

      if (provider != SmsProviders.Twilio)
      {
         return client;
      }

      if (!props.TryGetValue("SID", out var sid) || string.IsNullOrWhiteSpace(sid))
      {
         throw new ArgumentException("Missing SID in SmsConfiguration.Properties.");
      }

      if (!props.TryGetValue("AUTH_TOKEN", out var token) || string.IsNullOrWhiteSpace(token))
      {
         throw new ArgumentException("Missing AUTH_TOKEN in SmsConfiguration.Properties.");
      }

      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
         "Basic",
         Convert.ToBase64String(Encoding.ASCII.GetBytes($"{sid}:{token}"))
      );

      return client;
   }

   private static async Task<List<GeneralSmsResponse>> SendSmsAsync(HttpClient client,
      SmsConfiguration config,
      SmsMessage smsMessage,
      CancellationToken ct)
   {
      var provider = GetSmsProvider(config);

      return provider switch
      {
         SmsProviders.Dexatel => await SendSmsViaDexatelAsync(client, config, smsMessage, ct),
         SmsProviders.Twilio => await SendSmsViaTwilioAsync(client, config, smsMessage, ct),
         _ => throw new InvalidOperationException()
      };
   }

   private static Uri ResolveBaseUri(HttpClient client, SmsConfiguration config)
   {
      return client.BaseAddress ?? new Uri(SmsProviderIntegrations.BaseUrls[config.Provider]);
   }

   private static async Task<List<GeneralSmsResponse>> SendSmsViaDexatelAsync(HttpClient client,
      SmsConfiguration config,
      SmsMessage smsMessage,
      CancellationToken ct)
   {
      var request = new DexatelSmsSendRequest
      {
         Data = new DexatelSmsSendRequestData
         {
            From = config.From,
            To = smsMessage.Recipients
                           .Distinct()
                           .ToList(),
            Text = smsMessage.Message,
            Channel = "SMS"
         }
      };

      var baseUri = ResolveBaseUri(client, config);
      var url = new Uri(baseUri, "/v1/messages");

      using var content = new StringContent(JsonSerializer.Serialize(request, SnakeCaseJson),
         Encoding.UTF8,
         "application/json");
      using var response = await client.PostAsync(url, content, ct);

      var responseContent = await response.Content.ReadAsStringAsync(ct);
      var responseObject = JsonSerializer.Deserialize<DexatelSmsSendResponse>(responseContent, CamelCaseJson);

      return responseObject?.Data
                           .Select(x => new GeneralSmsResponse
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

   private static async Task<List<GeneralSmsResponse>> SendSmsViaTwilioAsync(HttpClient client,
      SmsConfiguration config,
      SmsMessage smsMessage,
      CancellationToken ct)
   {
      var baseUri = ResolveBaseUri(client, config);
      var sid = config.Properties["SID"];

      var url = new Uri(baseUri, $"/2010-04-01/Accounts/{sid}/Messages.json");

      var result = new List<TwilioSmsSendResponse>();

      foreach (var phone in smsMessage.Recipients
                                      .Distinct()
                                      .ToList())
      {
         using var form = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("To", phone),
            new KeyValuePair<string, string>("From", config.From),
            new KeyValuePair<string, string>("Body", smsMessage.Message)
         ]);

         using var response = await client.PostAsync(url, form, ct);

         var responseContent = await response.Content.ReadAsStringAsync(ct);
         var responseObject = JsonSerializer.Deserialize<TwilioSmsSendResponse>(responseContent, CamelCaseJson);

         result.Add(responseObject ?? new TwilioSmsSendResponse());
      }

      return result.Select(x => new GeneralSmsResponse
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
}