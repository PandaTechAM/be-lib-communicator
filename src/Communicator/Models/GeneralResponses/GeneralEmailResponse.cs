using System.Text.RegularExpressions;

namespace Communicator.Models.GeneralResponses;

public class GeneralEmailResponse
{
    public string Status { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string TrackingId { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string Service { get; set; } = null!;

    // Define a regular expression pattern to extract relevant parts of the response
    private const string Pattern = @"^(\d+\.\d+\.\d+)\s+(\w+)\s+(\d+)\s+(\S+)\s+\-\s+(\w+)$";

    public static GeneralEmailResponse Parse(string response)
    {
        // Match the response string against the pattern
        var match = Regex.Match(response, Pattern);

        // Check if the response string matches the expected format
        if (!match.Success || match.Groups.Count != 6)
        {
            throw new ArgumentException("Invalid response string format.");
        }

        // Create a new instance of EmailSendResponse and populate its properties
        return new GeneralEmailResponse
        {
            Status = match.Groups[1].Value,
            Code = match.Groups[2].Value,
            TrackingId = match.Groups[3].Value,
            Id = match.Groups[4].Value,
            Service = match.Groups[5].Value
        };
    }

    public static List<GeneralEmailResponse> Parse(IEnumerable<string> responses)
    {
        var responseList = new List<GeneralEmailResponse>();

        foreach (var match in responses.Select(response => Regex.Match(response, Pattern)))
        {
            // Check if the response string matches the expected format
            if (!match.Success || match.Groups.Count != 6)
            {
                throw new ArgumentException("Invalid response string format.");
            }

            // Create a new instance of EmailSendResponse and populate its properties
            responseList.Add(new GeneralEmailResponse
            {
                Status = match.Groups[1].Value,
                Code = match.Groups[2].Value,
                TrackingId = match.Groups[3].Value,
                Id = match.Groups[4].Value,
                Service = match.Groups[5].Value
            });
        }

        return responseList;
    }
}