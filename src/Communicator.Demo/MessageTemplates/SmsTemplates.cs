namespace Communicator.Demo.MessageTemplates;

public static class SmsTemplates
{
    private const string ServiceName = "PandaTech.Communicator";

    public static string OtpCodeVerificationRequestMessage(string otpCode)
    {
        return
            $"Code: {otpCode} is your {ServiceName} verification code to verify your new phone number in the system. Do not share this code.";
    }
}