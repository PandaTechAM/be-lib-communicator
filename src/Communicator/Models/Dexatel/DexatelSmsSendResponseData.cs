namespace Communicator.Models.Dexatel;

public class DexatelSmsSendResponseData
{
   public required string Id { get; set; }
   public required string AccountId { get; set; }
   public required string Text { get; set; }
   public required string From { get; set; }
   public required string To { get; set; }
   public required string Channel { get; set; }
   public required string Status { get; set; }
   public DateTime CreateDate { get; set; }
   public DateTime UpdateDate { get; set; }
   public required string Encoding { get; set; }
}