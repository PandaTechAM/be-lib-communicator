namespace Communicator.Models.Twilio;

/// <summary>Related subresource URIs referenced by a Twilio message resource.</summary>
public class SubresourceUris
{
    /// <summary>Relative URI of the message media list.</summary>
    public string Media { get; set; } = null!;
}
