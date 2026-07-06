using System.Text.RegularExpressions;

namespace Communicator.Models.GeneralResponses;

/// <summary>Normalized email send result parsed from a raw Gmail or Outlook SMTP response string.</summary>
public class GeneralEmailResponse
{
    // Define a regular expression pattern to extract relevant parts of the response
    private const string GmailPattern = @"^(\d+\.\d+\.\d+)\s+(\w+)\s+(\d+)\s+(\S+)\s+\-\s+(\w+)$";

    private const string OutlookPattern =
        @"^(?<Version>\d+\.\d+\.\d+)\s(?<Status>\w+)\s<(?<EmailId>[^>]+)>\s\[Hostname=(?<Hostname>[^\]]+)\]$";

    /// <summary>SMTP status code triplet (e.g. "2.0.0").</summary>
    public required string Status { get; set; }

    /// <summary>SMTP response code.</summary>
    public required string Code { get; set; }

    /// <summary>Provider tracking id, when present in the response.</summary>
    public string? TrackingId { get; set; }

    /// <summary>Message id assigned by the server.</summary>
    public required string Id { get; set; }

    /// <summary>Service or host name that accepted the message.</summary>
    public required string Service { get; set; }

    /// <summary>
    ///     Parse a single raw SMTP response string; throws <see cref="ArgumentException" /> if the format is
    ///     unrecognized.
    /// </summary>
    public static GeneralEmailResponse Parse(string response)
    {
        var match = Regex.Match(response, GmailPattern);

        if (!match.Success)
        {
            match = Regex.Match(response, OutlookPattern);

            if (!match.Success)
            {
                throw new ArgumentException("Invalid response string format.");
            }
        }

        if (match.Groups.Count == 5)
        {
            return new GeneralEmailResponse
            {
                Status = match.Groups[1].Value,
                Code = match.Groups[2].Value,
                TrackingId = null,
                Id = match.Groups[3].Value,
                Service = match.Groups[4].Value
            };
        }

        if (match.Groups.Count == 6)
        {
            return new GeneralEmailResponse
            {
                Status = match.Groups[1].Value,
                Code = match.Groups[2].Value,
                TrackingId = match.Groups[3].Value,
                Id = match.Groups[4].Value,
                Service = match.Groups[5].Value
            };
        }

        throw new ArgumentException("Invalid response string format.");
    }

    /// <summary>
    ///     Parse multiple raw SMTP response strings; throws <see cref="ArgumentException" /> on the first unrecognized
    ///     format.
    /// </summary>
    public static List<GeneralEmailResponse> Parse(IEnumerable<string> responses)
    {
        var responseList = new List<GeneralEmailResponse>();

        foreach (var response in responses)
        {
            var match = Regex.Match(response, GmailPattern);

            if (!match.Success)
            {
                match = Regex.Match(response, OutlookPattern);

                if (!match.Success)
                {
                    throw new ArgumentException("Invalid response string format.");
                }
            }

            switch (match.Groups.Count)
            {
                case 5:
                    responseList.Add(new GeneralEmailResponse
                    {
                        Status = match.Groups[1].Value,
                        Code = match.Groups[2].Value,
                        TrackingId = null,
                        Id = match.Groups[3].Value,
                        Service = match.Groups[4].Value
                    });
                    break;
                case 6:
                    responseList.Add(new GeneralEmailResponse
                    {
                        Status = match.Groups[1].Value,
                        Code = match.Groups[2].Value,
                        TrackingId = match.Groups[3].Value,
                        Id = match.Groups[4].Value,
                        Service = match.Groups[5].Value
                    });
                    break;
            }
        }

        return responseList;
    }
}
