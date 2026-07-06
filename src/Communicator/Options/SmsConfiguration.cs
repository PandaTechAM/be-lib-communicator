namespace Communicator.Options;

/// <summary>Settings for a single named SMS channel, including provider and provider-specific properties.</summary>
public class SmsConfiguration
{
    /// <summary>SMS provider name, e.g. "Dexatel" or "Twilio".</summary>
    public string Provider { get; set; } = null!;

    /// <summary>Sender identifier (phone number or alphanumeric sender ID).</summary>
    public string From { get; set; } = null!;

    /// <summary>Provider-specific properties such as API keys or channel identifiers.</summary>
    public Dictionary<string, string> Properties { get; set; } = new();

    /// <summary>HTTP request timeout in milliseconds for the provider call. Defaults to 10000.</summary>
    public int TimeoutMs { get; set; } = 10000;
}
