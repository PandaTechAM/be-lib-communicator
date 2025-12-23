namespace Communicator.Models.GeneralResponses;

public class GeneralSmsResponse
{
   public required string From { get; set; }
   public required string To { get; set; }
   public required string OuterSmsId { get; set; }
   public required string Status { get; set; }
   public DateTime CreateDate { get; set; }
   public DateTime UpdateDate { get; set; }
   public required string Body { get; set; }
}