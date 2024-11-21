namespace Communicator.Helpers;

internal static class DistinctMaker
{
   internal static List<T> MakeDistinct<T>(this IEnumerable<T> list)
   {
      return list.Distinct()
                 .ToList();
   }
}