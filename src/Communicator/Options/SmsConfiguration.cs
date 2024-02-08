using Communicator.Enums;

namespace Communicator.Options;

public class SmsConfiguration
{
    public int SmsProvider { get; set; }
    public string Properties { get; set; } = null!; // todo just for sample
}