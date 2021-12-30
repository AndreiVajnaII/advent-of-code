public static class Helpers
{
    public static string PadDay(string day)
    {
        return day.Length < 2 ? $"0{day}" : day;
    }

    public static uint ParseBinary(string s)
    {
        return Convert.ToUInt32(s, 2);
    }

    public static ulong ParseBinaryLong(string s)
    {
        return Convert.ToUInt64(s, 2);
    }

    public static int ParseChar(char c)
    {
        return (int)(c - '0');
    }

    // group lines separated by blank lines
    public static IEnumerable<IEnumerable<string>> GroupLines(string[] lines)
    {
        bool newGroup = true;
        var group = new List<string>();
        foreach (var line in lines)
        {
            if (line == "")
            {
                yield return group;
                group = new List<string>();
                newGroup = true;
            }
            else
            {
                group.Add(line);
                if (newGroup)
                {
                    newGroup = false;
                }
            }
        }
        yield return group;
    }
}

public static class NumberExtensions
{
    public static void TimesDo(this Int32 times, Action action)
    {
        for (int i = 0; i < times; i++) action();
    }

    public static IEnumerable<TResult> Times<TResult>(this Int32 times, Func<TResult> f)
    {
        for (int i = 0; i < times; i++) yield return f();
    }

    public static T TimesIterate<T>(this Int32 times, Func<T, T> f, T initialValue)
    {
        T result = initialValue;
        for (int i = 0; i < times; i++)
        {
            result = f(result);
        }
        return result;
    }
}

public static class IEnumerableExtensions
{
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
        => enumerable.SelectMany(x => x);
    public static IEnumerable<(T, T)> Pairwise<T>(this IEnumerable<T> enumerable)
        => enumerable.Pair((item, i) => enumerable.Skip(i + 1));
    public static IEnumerable<(T, T)> Pair<T>(this IEnumerable<T> enumerable, Func<T, int, IEnumerable<T>> selector)
        => enumerable.SelectMany((item1, i) => selector(item1, i).Select(item2 => (item1, item2)));
    public static IEnumerable<(T, T)> Pair<T>(this IEnumerable<T> enumerable, Func<T, IEnumerable<T>> selector)
        => enumerable.Pair((item, _) => selector(item));

    public static IEnumerable<(T1, T2)> Pair<T1, T2>(this IEnumerable<T1> e1, IEnumerable<T2> e2)
        => e1.SelectMany(item1 => e2.Select(item2 => (item1, item2)));

    public static IEnumerable<IEnumerable<T>> Combine<T>(this IEnumerable<T> enumerable, IEnumerable<T> other)
        => enumerable.SelectMany(first => other.Select(second => new T[] { first, second }));
    public static IEnumerable<IEnumerable<T>> Combine<T>(this IEnumerable<IEnumerable<T>> enumerable, IEnumerable<T> other)
        => enumerable.SelectMany(first => other.Select(second => first.Append(second)));
    public static IEnumerable<IEnumerable<T>> CombineAll<T>(this IEnumerable<IEnumerable<T>> enumerable)
    {
        var (first, second, rest) = enumerable;
        return rest.Aggregate(first.Combine(second), (result, current) => result.Combine(current));
    }

    public static IEnumerable<T> Without<T>(this IEnumerable<T> enumerable, T item)
        => enumerable.Where(i => i is not null && !i.Equals(item));

    public static string AsString(this IEnumerable<char> enumerable)
        => String.Join("", enumerable);

    public static long Product(this IEnumerable<long> enumerable)
        => enumerable.Aggregate((a, b) => a * b);

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

public class Grid2D
{
    public static readonly (int X, int Y)[] orthogonalNeighbours = new[] { (1, 0), (0, 1), (-1, 0), (0, -1) };
    public static readonly (int X, int Y)[] diagonalNeighbours = new[] { (1, 1), (1, -1), (-1, 1), (-1, -1) };
    public static readonly (int X, int Y)[] allNeighbours = orthogonalNeighbours.Union(diagonalNeighbours).ToArray();
}

public class Grid2D<T>
{
    private T[,] grid;

    public int Width
    {
        get
        {
            return grid.GetLength(1);
        }
    }

    public int Height
    {
        get
        {
            return grid.GetLength(0);
        }
    }

    public Point BottomRight
    {
        get
        {
            return new Point(Width - 1, Height - 1);
        }
    }

    public int Count
    {
        get
        {
            return Width * Height;
        }
    }

    public Grid2D(T[,] grid)
    {
        this.grid = grid;
    }

    public Grid2D(int width, int height)
    {
        this.grid = new T[height, width];
    }

    public Grid2D(int width, int height, T initialValue) : this(width, height)
    {
        foreach (var p in CoordEnumerable())
        {
            SetValueAt(p, initialValue);
        }
    }

    public T ValueAt(Point p) => grid[p.Y, p.X];

    public void SetValueAt(Point p, T value)
    {
        grid[p.Y, p.X] = value;
    }

    public IEnumerable<T> ValueEnumerable() => CoordEnumerable().Select(ValueAt);

    public IEnumerable<Point> CoordEnumerable() => GridEnumerable().Flatten();

    public IEnumerable<IEnumerable<T>> GridValueEnumerable()
        => GridEnumerable().Select(line => line.Select(ValueAt));

    public IEnumerable<IEnumerable<Point>> GridEnumerable()
        => Enumerable.Range(0, Height).Select(LineEnumerable);

    public IEnumerable<Point> LineEnumerable(int y)
        => Enumerable.Range(0, Width).Select(x => new Point(x, y));

    public override string ToString()
        => String.Join(Environment.NewLine, GridValueEnumerable()
            .Select(line => String.Join(' ', line)));

    public IEnumerable<T> Adjacents(Point p, (int X, int Y)[] neighbours)
        => AdjacentPoints(p, neighbours).Select(ValueAt);

    public IEnumerable<Point> AdjacentPoints(Point p, (int X, int Y)[] neighbours)
        => p.SelectAdjacents(neighbours).Where(IsInBounds);

    public bool IsInBounds(Point p)
        => grid.GetLowerBound(0) <= p.Y && p.Y <= grid.GetUpperBound(0)
        && grid.GetLowerBound(1) <= p.X && p.X <= grid.GetUpperBound(1);

    public Grid2D<TNew> Spawn<TNew>()
    {
        return new Grid2D<TNew>(Width, Height);
    }

    public Grid2D<TNew> Spawn<TNew>(TNew initialValue)
    {
        return new Grid2D<TNew>(Width, Height, initialValue);
    }
}

public class SparseGrid<T>
{
    public int Xmin { get; private set; }
    public int Xmax { get; private set; }
    public int Ymin { get; private set; }
    public int Ymax { get; private set; }
    public int Count { get => pixels.Count; }

    private readonly IDictionary<Point, T?> pixels = new Dictionary<Point, T?>();
    private readonly T? nullValue;

    public SparseGrid(T? nullValue)
    {
        this.nullValue = nullValue;
    }

    public void Set(Point p, T? value)
    {
        pixels[p] = value;
        if (p.X < Xmin) Xmin = p.X;
        if (p.Y < Ymin) Ymin = p.Y;
        if (p.X > Xmax) Xmax = p.X;
        if (p.Y > Ymax) Ymax = p.Y;
    }

    public T? ValueAt(Point p)
    {
        return pixels.ContainsKey(p) ? pixels[p] : nullValue;
    }

    public bool IsInBounds(Point p)
    {
        return Xmin <= p.X && p.X <= Xmax
            && Ymin <= p.Y && p.Y <= Ymax;
    }
}

public class SparseGrid : SparseGrid<object>
{
    public SparseGrid() : base(null) { }

    public void Set(Point p)
    {
        Set(p, null);
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

    public IEnumerable<Point> SelectAdjacents((int X, int Y)[] neighbours)
    {
        var p = this;
        return neighbours.Select(d => new Point(p.X + d.X, p.Y + d.Y));
    }

    public static IEnumerable<Point> EnumeratePoints(int minX, int minY, int maxX, int maxY)
    {
        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                yield return new Point(x, y);
            }
        }
    }

    public static Point Parse(string s)
    {
        var (x, y) = s.Split(',').Select(int.Parse).AsTuple2();
        return new Point(x, y);
    }

    public override string ToString()
    {
        return $"{X},{Y}";
    }
}

public class Graph<T> : Dictionary<T, ISet<T>> where T : notnull
{
    public void AddTwoWay(T start, T end)
    {
        AddOneWay(start, end);
        AddOneWay(end, start);
    }

    public void AddOneWay(T start, T end)
    {
        if (!ContainsKey(start))
        {
            Add(start, new HashSet<T>());
        }
        this[start].Add(end);
    }

    public TVisitor Traverse<TVisitor>(T current, T end, TVisitor visitor) where TVisitor : GraphVisitor<T>
    {
        if (current.Equals(end))
        {
            visitor.End(end);
        }
        else
        {
            visitor.Visit(current);
            foreach (var neighbour in this[current])
            {
                if (!visitor.HasVisited(neighbour))
                {
                    Traverse(neighbour, end, visitor);
                }
            }
            visitor.Unvisit(current);
        }
        return visitor;
    }
}

public class GraphVisitor<T>
{
    private IGraphVisitPolicy<T> visitPolicy;

    public GraphVisitor() : this(new DefaultGraphVisitPolicy<T>()) { }

    public GraphVisitor(IGraphVisitPolicy<T> visitPolicy)
    {
        this.visitPolicy = visitPolicy;
    }

    public virtual void Visit(T node)
    {
        visitPolicy.Visit(node);
    }

    public virtual void End(T node) { }

    public virtual bool HasVisited(T node)
    {
        return visitPolicy.HasCompletelyVisited(node);
    }

    public virtual void Unvisit(T node)
    {
        visitPolicy.Unvisit(node);
    }
}

public interface IGraphVisitPolicy<T>
{
    void Visit(T node);
    void Unvisit(T node);
    bool HasCompletelyVisited(T node);
}

public class DefaultGraphVisitPolicy<T> : IGraphVisitPolicy<T>
{
    private ISet<T> visited = new HashSet<T>();
    public virtual bool HasCompletelyVisited(T node)
    {
        return visited.Contains(node);
    }

    public virtual void Visit(T node)
    {
        visited.Add(node);
    }

    public virtual void Unvisit(T node)
    {
        visited.Remove(node);
    }

}

class DictionaryWithDefault<TKey, TValue> : Dictionary<TKey, TValue> where TKey : notnull
{
    private readonly TValue defaultValue;
    public DictionaryWithDefault(TValue defaultValue)
    {
        this.defaultValue = defaultValue;
    }

    public new TValue this[TKey key]
    {
        get
        {
            if (!ContainsKey(key))
            {
                Add(key, defaultValue);
            }
            return base[key];
        }
        set
        {
            base[key] = value;
        }
    }
}
