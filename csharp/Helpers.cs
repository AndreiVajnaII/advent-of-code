public static class Helpers {
    public static string PadDay(string day)
    {
        return day.Length < 2 ? $"0{day}" : day;
    }

    public static uint ParseBinary(string s)
    {
        return Convert.ToUInt32(s, 2);
    }

    public static int ParseChar(char c)
    {
        return (int)(c - '0');
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

    public static (T, T) AsTuple2<T>(this IEnumerable<T> enumerable)
    {
        var (first, second, _) = enumerable;
        return (first, second);
    }

    public static T[,] ToArray2D<T>(this IEnumerable<IEnumerable<T>> enumerable)
    {
        var array = enumerable.Select(e => e.ToArray()).ToArray();
        T[,] array2D = new T[array.Length, array[0].Length];
        for (int row = 0; row < array2D.GetLength(0); row++)
        {
            for (int col = 0; col < array2D.GetLength(1); col++)
            {
                array2D[row, col] = array[row][col];
            }
        }
        return array2D;
    }
}

public class Grid2D<T>
{
    private static (int X, int Y)[] neighbours = new [] {(1, 0), (0, 1), (-1, 0), (0, -1)};

    private T[,] grid;

    public Grid2D(T[,] grid)
    {
        this.grid = grid;
    }

    public T ValueAt(Point p)
    {
        return grid[p.Y, p.X];
    }

    public IEnumerable<Point> CoordEnumerable()
    {
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                yield return new Point(x, y);
            }
        }
    }

    public int getWidth()
    {
        return grid.GetLength(1);
    }

    public int getHeight()
    {
        return grid.GetLength(0);
    }

    public IEnumerable<T> Adjacents(Point p)
    {
        return neighbours
            .Select(d => new Point(p.X + d.X, p.Y + d.Y))
            .Where(IsInBounds)
            .Select(ValueAt);
    }

    public bool IsInBounds(Point p)
    {
        return grid.GetLowerBound(0) <= p.Y && p.Y <= grid.GetUpperBound(0)
            && grid.GetLowerBound(1) <= p.X && p.X <= grid.GetUpperBound(1);
    }

}

public struct Point
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"{X},{Y}";
    }
}