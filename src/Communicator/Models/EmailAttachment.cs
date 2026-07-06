namespace Communicator.Models;

/// <summary>A single email attachment.</summary>
/// <param name="fileName">File name shown to the recipient.</param>
/// <param name="content">Raw attachment bytes.</param>
public class EmailAttachment(string fileName, byte[] content)
{
    /// <summary>File name shown to the recipient.</summary>
    public string FileName { get; set; } = fileName;

    /// <summary>Raw attachment bytes.</summary>
    public byte[] Content { get; set; } = content;
}
