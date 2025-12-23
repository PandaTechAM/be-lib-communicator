using Communicator.Models;
using Communicator.Services.Interfaces;

namespace Communicator.Demo;

public static class TestEndpoints
{
   public static WebApplication MapTestEndpoints(this WebApplication app)
   {
      app.MapPost("/email/test",
            async (IEmailService email, SendEmailRequest req, CancellationToken ct) =>
            {
               var msg = new EmailMessage
               {
                  Channel = req.Channel ?? "GeneralSender",
                  Recipients = req.To,
                  Subject = req.Subject,
                  Body = req.Body,
                  IsBodyHtml = req.IsHtml,
                  Cc = req.Cc ?? [],
                  Bcc = req.Bcc ?? []
               };

               var response = await email.SendAsync(msg, ct);
               return Results.Ok(new
               {
                  response
               });
            })
         .WithName("SendTestEmail")
         .WithTags("Haik")
         .WithOpenApi();
      return app;
   }

   public sealed record SendEmailRequest(
      List<string> To,
      string Subject,
      string Body,
      bool IsHtml = false,
      string? Channel = "GeneralSender",
      List<string>? Cc = null,
      List<string>? Bcc = null);
}