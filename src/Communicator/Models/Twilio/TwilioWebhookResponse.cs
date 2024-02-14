namespace Communicator.Models.Twilio;

public class TwilioWebhookResponse
{
    public string Body { get; set; } = null!;
    public string MessageSid { get; set; } = null!;
    public string SmsSid { get; set; } = null!;
    public string AccountSid { get; set; } = null!;
    public string MessagingServiceSid { get; set; } = null!;
    public string From { get; set; } = null!;
    public string To { get; set; } = null!;
    public int NumMedia { get; set; }
}