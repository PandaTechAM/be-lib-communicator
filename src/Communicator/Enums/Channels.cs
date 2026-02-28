using System.Reflection;

namespace Communicator.Enums;

internal static class Channels
{
   internal static readonly IReadOnlyList<string> EmailChannels = GetEmailChannels();
   internal static readonly IReadOnlyList<string> SmsChannels = GetSmsChannels();

   private static IReadOnlyList<string> GetEmailChannels()
   {
      return typeof(EmailChannels)
             .GetFields(BindingFlags.Public | BindingFlags.Static)
             .Where(f => f.FieldType == typeof(string))
             .Select(f => (string)f.GetValue(null)!)
             .ToArray();
   }

   private static IReadOnlyList<string> GetSmsChannels()
   {
      return typeof(SmsChannels)
             .GetFields(BindingFlags.Public | BindingFlags.Static)
             .Where(f => f.FieldType == typeof(string))
             .Select(f => (string)f.GetValue(null)!)
             .ToArray();
   }
}