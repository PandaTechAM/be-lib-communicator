namespace Communicator.Demo.MessageTemplates;

public static class EmailTemplates
{
    private const string ServiceName = "PandaTech.Communicator";
    private const string CompanyName = "PandaTech";
    private const string Minutes = "15";
    private const string Hours = "24";

    private const string GreenButtonStyle = """
                                            .button {
                                                background-color: #4CAF50;
                                                border: none;
                                                color: white;
                                                padding: 15px 32px;
                                                text-align: center;
                                                text-decoration: none;
                                                display: inline-block;
                                                font-size: 16px;
                                                margin: 4px 2px;
                                                cursor: pointer;
                                                border-radius: 25px;
                                                box-shadow: 0 4px 8px 0 rgba(0,0,0,0.2);
                                                transition: 0.3s;
                                            }
                                            .button:hover {
                                                background-color: #45a049;
                                                box-shadow: 0 8px 16px 0 rgba(0,0,0,0.2);
                                            }
                                            """;

    private const string RedButtonStyle = """
                                          .button {
                                              background-color: #D9534F; /* Bootstrap's btn-danger */
                                              border: none;
                                              color: white;
                                              padding: 15px 32px;
                                              text-color: white;
                                              text-align: center;
                                              text-decoration: none;
                                              display: inline-block;
                                              font-size: 16px;
                                              margin: 4px 2px;
                                              cursor: pointer;
                                              border-radius: 25px;
                                              box-shadow: 0 4px 8px 0 rgba(0,0,0,0.2);
                                              transition: 0.3s;
                                          }
                                          .button:hover {
                                              background-color: #C9302C; /* Darker shade of red for hover effect */
                                              box-shadow: 0 8px 16px 0 rgba(0,0,0,0.2);
                                          }
                                          """;

    public static string AddEmailAddressTemplate(string firstName, string lastName, string link)
    {
        return $$"""
                 <!DOCTYPE html>
                 <html>
                 <head>
                   <style>
                     body { font-family: 'Arial'; }
                     {{GreenButtonStyle}}
                   </style>
                 </head>
                 <body>
                   <p>Hello {{FullName(firstName, lastName)}},</p>
                   <p>You have requested to add a new email address to your {{ServiceName}} account. To confirm and complete this action, please click the button below:</p>
                   <a href='{{link}}' class='button'>Add Email Address</a>
                   <p>This link will expire in {{Minutes}} minutes. If you did not request this change or if it was made by mistake, no action is needed.</p>
                   <p>If you're having trouble clicking the "Add Email Address" button, copy and paste the URL below into your web browser:</p>
                   <p><a href='{{link}}'>{{link}}</a></p>
                   <p>Thank you for updating your account,</p>
                   <p>The {{CompanyName}} Team</p>
                 </body>
                 </html>
                 """;
    }

    private static string FullName(string firstName, string lastName)
    {
        return $"{firstName} {lastName}".Trim();
    }
}