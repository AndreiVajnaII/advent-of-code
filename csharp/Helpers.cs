public class Helpers {
    public static string PadDay(string day) {
        return day.Length < 2 ? $"0{day}" : day;
    }

    public static uint ParseBinary(string s) {
        return Convert.ToUInt32(s, 2);
    }
}

public static class IEnumerableExtensions {
    public static IEnumerable<(T, T)> Pairwise<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.SelectMany((item1, i) => enumerable.Skip(i + 1).Select(item2 => (item1, item2)));
    }

    public static void Deconstruct<T>(this IEnumerable<T> enumerable, out T first, out IEnumerable<T> rest)
    {
        first = enumerable.First();
        rest = enumerable.Skip(1);
    }

    public static void Deconstruct<T>(this IEnumerable<T> enumerable, out T first, out T second, out IEnumerable<T> rest)
    {
        (first, var tail) = enumerable;
        (second, rest) = tail;
    }
}