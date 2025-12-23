namespace Communicator.Models.Dexatel;

public class DexatelSmsSendRequestData
{
   public required string From { get; set; }
   public required List<string> To { get; set; }
   public required string Text { get; set; }
   public required string Channel { get; set; }
}