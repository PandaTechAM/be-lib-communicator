namespace Communicator.Options;

/// <summary>Root configuration for the communicator: named SMS and email channels plus fake-mode toggles.</summary>
public class CommunicatorOptions
{
    /// <summary>When true, SMS is not sent; a fake service logs instead (useful in development).</summary>
    public bool SmsFake { get; set; }

    /// <summary>SMS channel configurations keyed by channel name (see <see cref="Enums.SmsChannels" />).</summary>
    public Dictionary<string, SmsConfiguration>? SmsConfigurations { get; set; }

    /// <summary>When true, email is not sent; a fake service logs instead (useful in development).</summary>
    public bool EmailFake { get; set; }

    /// <summary>Email channel configurations keyed by channel name (see <see cref="Enums.EmailChannels" />).</summary>
    public Dictionary<string, EmailConfiguration>? EmailConfigurations { get; set; }
}
