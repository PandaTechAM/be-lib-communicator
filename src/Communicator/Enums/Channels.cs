using System.Reflection;

namespace Communicator.Enums;

internal static class Channels
{
   internal static List<string> EmailChannels => GetEmailChannels();

   internal static List<string> SmsChannels => GetSmsChannels();

   private static List<string> GetEmailChannels()
   {
      var fields = typeof(EmailChannels).GetFields(BindingFlags.Public | BindingFlags.Static);

      return fields
             .Where(field => field.FieldType == typeof(string))
             .Select(field => (string)field.GetValue(null)!)
             .ToList();
   }

   private static List<string> GetSmsChannels()
   {
      var fields = typeof(SmsChannels).GetFields(BindingFlags.Public | BindingFlags.Static);

      return fields
             .Where(field => field.FieldType == typeof(string))
             .Select(field => (string)field.GetValue(null)!)
             .ToList();
   }
}