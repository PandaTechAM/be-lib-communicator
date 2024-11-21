namespace Communicator.Extensions;

internal static class StringExtensions
{
   internal static string RemovePhoneFormatParenthesesAndAdditionSign(this string phoneString)
   {
      return phoneString
             .Trim()
             .Replace("(", "")
             .Replace(")", "")
             .Replace(" ", "")
             .Replace("+", "");
   }

   internal static List<string> RemovePhoneFormatParenthesesAndAdditionSign(this List<string> phoneStrings)
   {
      return phoneStrings.Select(x => x.Trim()
                                       .Replace("(", "")
                                       .Replace(")", "")
                                       .Replace(" ", "")
                                       .Replace("+", ""))
                         .ToList();
   }
}