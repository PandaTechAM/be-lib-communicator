namespace Communicator.Models.Twilio;

public class TwilioSmsSendResponse
{
   public string Body { get; set; } = null!;
   public int NumSegments { get; set; }
   public string Direction { get; set; } = null!;
   public string From { get; set; } = null!;
   public DateTime DateUpdated { get; set; }
   public int? Price { get; set; } // is this string?
   public string? ErrorMessage { get; set; }
   public string Uri { get; set; } = null!;
   public string AccountSid { get; set; } = null!;
   public string To { get; set; } = null!;
   public DateTime DateCreated { get; set; }
   public string Status { get; set; } = null!;
   public string Sid { get; set; } = null!;
   public DateTime? DateSent { get; set; }
   public string? MessagingServiceSid { get; set; }
   public int? ErrorCode { get; set; }
   public string PriceUnit { get; set; } = null!;
   public string ApiVersion { get; set; } = null!;
   public SubresourceUris SubresourceUris { get; set; } = null!;
}