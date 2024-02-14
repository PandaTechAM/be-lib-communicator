namespace Communicator.Helpers;

public static class DistinctMaker
{
    public static List<T> MakeDistinct<T>(this IEnumerable<T> list)
    {
        return list.Distinct().ToList();
    }
}