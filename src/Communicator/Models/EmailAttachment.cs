namespace Communicator.Models;

public class EmailAttachment(string fileName, byte[] content)
{
   public string FileName { get; set; } = fileName;
   public byte[] Content { get; set; } = content;
}