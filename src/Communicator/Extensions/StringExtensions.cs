namespace Communicator.Extensions;

public static class StringExtensions
{
    public static string RemovePhoneFormatParenthesesAndAdditionSign(this string phoneString)
    {
        return phoneString.Replace("(", "").Replace(")", "").Replace("+", "");
    }

    public static List<string> RemovePhoneFormatParenthesesAndAdditionSign(this List<string> phoneStrings)
    {
        return phoneStrings.Select(x => x.Replace("(", "").Replace(")", "").Replace("+", "")).ToList();
    }
}