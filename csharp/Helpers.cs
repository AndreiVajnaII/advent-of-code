global using Direction = (int dX, int dY);

using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Aoc;

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

    public static int ParseHex(string s)
    {
        return Convert.ToInt32(s, 16);
    }

    public static ulong ParseBinaryLong(string s)
    {
        return Convert.ToUInt64(s, 2);
    }

    public static int ParseChar(char c)
    {
        return c - '0';
    }

    public static T Id<T>(T x) => x;

    public static long GreatestCommonDivisor(long a, long b)
    {
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }
        return a;
    }

    public static long LeastCommonMultiple(long a, long b)
    {
        return a * b / GreatestCommonDivisor(a, b);
    }

    // group lines separated by blank lines
    public static IEnumerable<IEnumerable<string>> GroupLines(IEnumerable<string> lines)
    {
        var newGroup = true;
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

    public static ImmutableArray<T> InitializeImmutableArray<T>(int size) where T : new()
    {
        var array = new T[size];
        for (var i = 0; i < size; i++)
        {
            array[i] = new T();
        }

        return [.. array];
    }
}

public static class NumberExtensions
{
    /// <summary>
    /// Perform an action the specified number of times
    /// </summary>
    public static void TimesDo(this int times, Action action)
    {
        for (var i = 0; i < times; i++) action();
    }

    /// <summary>
    /// Call a function the specified number of times, returning the results as an IEnumerable
    /// </summary>
    public static IEnumerable<TResult> Times<TResult>(this int times, Func<TResult> f)
    {
        for (var i = 0; i < times; i++) yield return f();
    }

    /// <summary>
    /// Call a function the specified number of times, each time passing the result of the previous invocation 
    /// </summary>
    public static T TimesIterate<T>(this int times, Func<T, T> f, T initialValue)
    {
        var result = initialValue;
        for (var i = 0; i < times; i++)
        {
            result = f(result);
        }
        return result;
    }

    public static IEnumerable<int> EnumerateTo(this int start, int end, bool inclusive = false)
    {
        var direction = Math.Sign(end - start);
        var result = Graph.Walk(start, current => current == end, current => current + direction);
        return inclusive ? result.Append(end) : result;
    }

    public static bool IsBetween(this int x, int first, int second, bool inclusive = false)
        => inclusive
            ? (first <= x && x <= second) || (second <= x && x <= first)
            : (first < x && x < second) || (second < x && x < first);

}

public static class EnumerableExtensions
{
    public static IEnumerable<T> Do<T>(this IEnumerable<T> enumerable, Action<T> action)
        => enumerable.Select(item =>
        {
            action(item);
            return item;
        });

    public static IEnumerable<T> TakeWhileInclusive<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
    {
        foreach (var item in enumerable)
        {
            if (!predicate(item))
            {
                yield return item;
                break;
            }

            yield return item;
        }
    }

    public static IEnumerable<T> TakeEvery<T>(this IEnumerable<T> enumerable, int count, int? startWith = null)
    {
        if (startWith >= count)
        {
            throw new ArgumentException(nameof(startWith) + " has to be < " + nameof(count));
        }

        var i = startWith is null ? count : startWith + 1;
        foreach (var item in enumerable)
        {
            if (--i == 0)
            {
                yield return item;
                i = count;
            }
        }
    }

    // FirstOrDefault returns the default value for a struct, and you can easily make it nullable to return null.
    // This method solves that.
    public static T? FirstOrNull<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) where T : struct
    {
        foreach (var item in enumerable)
        {
            if (predicate(item))
            {
                return item;
            }
        }
        return null;
    }

    public static IEnumerable<(T Item, int Index)> WithIndex<T>(this IEnumerable<T> enumerable)
        => enumerable.Select((item, index) => (item, index));

    public static IEnumerable<int> IndexesOf<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        => enumerable.WithIndex().Where(pair => predicate(pair.Item)).Select(pair => pair.Index);

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
        => enumerable.SelectMany(x => x);

    // [1, 2, 3, 4] => [(1, 2), (1, 3), (1, 4), (2, 3), (2, 4), (3, 4)]
    public static IEnumerable<(T, T)> Pairwise<T>(this IEnumerable<T> enumerable)
        => enumerable.Pair((item, i) => enumerable.Skip(i + 1));
    public static IEnumerable<(T, T)> Pair<T>(this IEnumerable<T> enumerable, Func<T, int, IEnumerable<T>> selector)
        => enumerable.SelectMany((item1, i) => selector(item1, i).Select(item2 => (item1, item2)));
    public static IEnumerable<(T, T)> Pair<T>(this IEnumerable<T> enumerable, Func<T, IEnumerable<T>> selector)
        => enumerable.Pair((item, _) => selector(item));

    public static IEnumerable<(T1, T2)> Pair<T1, T2>(this IEnumerable<T1> e1, IEnumerable<T2> e2)
        => e1.SelectMany(item1 => e2.Select(item2 => (item1, item2)));

    // [1, 2, 3, 4] => [(1, 2), (2, 3), (3, 4)]
    public static IEnumerable<(T, T)> ZipShift<T>(this IEnumerable<T> enumerable)
    {
        using var e = enumerable.GetEnumerator();
        if (!e.MoveNext()) yield break;
        var prev = e.Current;
        while (e.MoveNext())
        {
            yield return (prev, e.Current);
            prev = e.Current;
        }
    }

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
        => string.Join("", enumerable);

    public static int Product(this IEnumerable<int> enumerable)
        => enumerable.Aggregate((a, b) => a * b);

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

    public static (TLeft, TRight) AsTuple2<T, TLeft, TRight>(this IEnumerable<T> enumerable,
        Func<T, TLeft> leftSelector, Func<T, TRight> rightSelector)
    {
        var (first, second, _) = enumerable;
        return (leftSelector(first), rightSelector(second));
    }

    public static (T, T, T) AsTuple3<T>(this IEnumerable<T> enumerable)
    {
        var (first, second, rest) = enumerable;
        var (third, _) = rest;
        return (first, second, third);
    }

    public static T[,] ToArray2D<T>(this IEnumerable<IEnumerable<T>> enumerable)
    {
        var array = enumerable.Select(e => e.ToArray()).ToArray();
        var array2D = new T[array.Length, array[0].Length];
        for (var row = 0; row < array2D.GetLength(0); row++)
        {
            for (var col = 0; col < array2D.GetLength(1); col++)
            {
                array2D[row, col] = array[row][col];
            }
        }
        return array2D;
    }

    public static TResult[,] ToArray2D<TSource, TResult>(this IEnumerable<IEnumerable<TSource>> enumerable,
        Func<TSource, TResult> selector)
        => enumerable.Select(inner => inner.Select(selector)).ToArray2D();

    public static Grid2D<T> ToGrid<T>(this IEnumerable<IEnumerable<T>> enumerable) where T : IEquatable<T>
        => new(enumerable.ToArray2D());

    public static Grid2D<TGrid> ToGrid<TSource, TGrid>(this IEnumerable<IEnumerable<TSource>> enumerable,
        Func<TSource, TGrid> selector) where TGrid : IEquatable<TGrid>
        => new(enumerable.ToArray2D(selector));
}

public static class RegexExtensions
{
    public static IEnumerable<string> GroupValues(this Match match) =>
        match.Groups.Values.Skip(1).Select(group => group.Value);
}

public interface IPointGrid<T>
{
    T this[Point point] { get; set; }

    int Xmin { get; }
    int Xmax { get; }
    int Ymin { get; }
    int Ymax { get; }
}

public static class Grid2D
{
    public static readonly Direction North = (0, -1);
    public static readonly Direction South = (0, 1);
    public static readonly Direction East = (1, 0);
    public static readonly Direction West = (-1, 0);
    public static readonly Direction NorthEast = (1, -1);
    public static readonly Direction NorthWest = (-1, -1);
    public static readonly Direction SouthEast = (1, 1);
    public static readonly Direction SouthWest = (-1, 1);

    public static readonly Direction[] OrthogonalNeighbours = [North, South, East, West];
    public static readonly Direction[] DiagonalNeighbours = [NorthEast, NorthWest, SouthEast, SouthWest];
    public static readonly Direction[] AllNeighbours = OrthogonalNeighbours.Union(DiagonalNeighbours).ToArray();

    public static Direction OppositeDirection(Direction direction) => (-direction.dX, -direction.dY);
}

public class Grid2D<T> : IPointGrid<T> where T : IEquatable<T>
{
    private readonly T[,] grid;

    public int Width => grid.GetLength(1);

    public int Height => grid.GetLength(0);

    public Point TopLeft => new(Xmin, Ymin);

    public Point BottomRight => new(Xmax, Ymax);

    public int Count => Width * Height;

    public int Xmin => grid.GetLowerBound(1);
    public int Xmax => grid.GetUpperBound(1);
    public int Ymin => grid.GetLowerBound(0);
    public int Ymax => grid.GetUpperBound(0);

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

    public T this[Point point]
    {
        get => ValueAt(point);
        set => SetValueAt(point, value);
    }

    public T ValueAt(Point p) => grid[p.Y, p.X];

    public void SetValueAt(Point p, T value)
    {
        grid[p.Y, p.X] = value;
    }

    public IEnumerable<T> ValueEnumerable()
        => CoordEnumerable().Select(ValueAt);

    public IEnumerable<Point> CoordEnumerable()
        => CoordEnumerable(new Point(Xmin, Ymin), new Point(Xmax, Ymax));

    public IEnumerable<Point> CoordEnumerable(Point start, Point end)
        => GridEnumerable(start, end).Flatten();

    public IEnumerable<IEnumerable<T>> GridValueEnumerable()
        => GridEnumerable().Select(line => line.Select(ValueAt));

    public IEnumerable<IEnumerable<Point>> GridEnumerable()
        => GridEnumerable(new Point(Xmin, Ymin), new Point(Xmax, Ymax));

    public IEnumerable<IEnumerable<Point>> GridEnumerable(Point start, Point end)
        => start.Y.EnumerateTo(end.Y, true).Select(y => LineEnumerable(y, start.X, end.X));

    public IEnumerable<IEnumerable<T>> ColumnValueEnumerable()
        => ColumnEnumerable().Select(col => col.Select(ValueAt));

    public IEnumerable<IEnumerable<Point>> ColumnEnumerable()
        => Enumerable.Range(0, Width).Select(x => Enumerable.Range(0, Height).Select(y => new Point(x, y)));

    public IEnumerable<Point> LineEnumerable(int y)
        => LineEnumerable(y, Xmin, Width);

    public IEnumerable<Point> LineEnumerable(int y, int startX, int endX)
        => startX.EnumerateTo(endX, true).Select(x => new Point(x, y));

    public override string ToString()
        => ToString(" ");

    public string ToString(string separator)
        => string.Join(Environment.NewLine, GridValueEnumerable()
            .Select(line => string.Join(separator, line)));

    public Point PositionOf(T value) => CoordEnumerable().First(point => ValueAt(point).Equals(value));

    public IEnumerable<T> Adjacents(Point p, (int X, int Y)[] neighbours)
        => AdjacentPoints(p, neighbours).Select(ValueAt);

    public IEnumerable<Point> AdjacentPoints(Point p, (int X, int Y)[] neighbours)
        => p.SelectAdjacents(neighbours).Where(IsInBounds);

    public IEnumerable<Point> Enumerate(Point start, Func<Point, Point> navigator)
    {
        return IsInBounds(start)
            ? Enumerate(navigator(start), navigator).Prepend(start)
            : Enumerable.Empty<Point>();
    }

    public IEnumerable<T> EnumerateValues(Point start, Func<Point, Point> navigator) =>
        Enumerate(start, navigator).Select(ValueAt);

    public IEnumerable<T> EnumerateValues(IEnumerable<Point> points) =>
        points.Where(IsInBounds).Select(ValueAt);

    public bool IsInBounds(Point p)
        => grid.GetLowerBound(0) <= p.Y && p.Y <= grid.GetUpperBound(0)
            && grid.GetLowerBound(1) <= p.X && p.X <= grid.GetUpperBound(1);

    public Grid2D<TNew> Spawn<TNew>() where TNew : IEquatable<TNew>
    {
        return new Grid2D<TNew>(Width, Height);
    }

    public Grid2D<TNew> Spawn<TNew>(TNew initialValue) where TNew : IEquatable<TNew>
    {
        return new Grid2D<TNew>(Width, Height, initialValue);
    }
}

public class SparseGrid<T>(T? nullValue) : IPointGrid<T?>
{
    public int Xmin { get; set; }
    public int Xmax { get; set; }
    public int Ymin { get; set; }
    public int Ymax { get; set; }
    public int Count => pixels.Count;

    public Point TopLeft => new(Xmin, Ymin);
    public Point BottomRight => new(Xmax, Ymax);

    private readonly Dictionary<Point, T?> pixels = [];

    protected void Set(Point p, T? value)
    {
        pixels[p] = value;
        if (p.X < Xmin) Xmin = p.X;
        if (p.Y < Ymin) Ymin = p.Y;
        if (p.X > Xmax) Xmax = p.X;
        if (p.Y > Ymax) Ymax = p.Y;
    }

    public bool IsSet(Point p) => pixels.ContainsKey(p);

    protected T? ValueAt(Point p)
        => pixels.TryGetValue(p, out T? value) ? value : nullValue;

    public T? this[Point point]
    {
        get => ValueAt(point);
        set => Set(point, value);
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

    public void Set(Point p) => Set(p, null);
}

public readonly struct Point(int x, int y) : IEquatable<Point>
{
    public int X { get; } = x;
    public int Y { get; } = y;

    public bool Equals(Point other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Point other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Point left, Point right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Point left, Point right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Adds dx and dy to the point
    /// </summary>
    public Point Delta(int dx, int dy)
        => new(X + dx, Y + dy);

    /// <summary>
    /// Moves the point in the specified direction, which is represented as a tuple
    /// </summary>
    public Point Move(Direction direction)
        => Delta(direction.dX, direction.dY);

    /// <summary>
    /// Checks whether the point is between the two specified points, including them
    /// </summary>
    public bool IsInBounds(Point lowerBound, Point upperBound)
        => lowerBound.X <= X & X <= upperBound.X
            && lowerBound.Y <= Y && Y <= upperBound.Y;

    public IEnumerable<Point> SelectAdjacents(IEnumerable<(int X, int Y)> neighbours)
    {
        var p = this;
        return neighbours.Select(d => new Point(p.X + d.X, p.Y + d.Y));
    }

    public static IEnumerable<Point> EnumeratePoints(int minX, int minY, int maxX, int maxY)
    {
        for (var y = minY; y <= maxY; y++)
        {
            for (var x = minX; x <= maxX; x++)
            {
                yield return new Point(x, y);
            }
        }
    }

    // moves a point using the specified navigator function until that function returns null
    public IEnumerable<Point> Navigate(Func<Point, Point?> navigator)
    {
        Point? current = this;
        while (current is not null)
        {
            yield return (Point)current;
            current = navigator((Point)current);
        }
    }

    // enumerates points between this point and the specified other point,
    // either in a horizontal or vertical direction, depending on
    // how the other point is situated relatively to this one
    public IEnumerable<Point> NavigateTo(Point other)
    {
        if (other.X != X && other.Y != Y)
        {
            throw new ArgumentException("Direction must be either horizontal or vertical");
        }
        var (dx, dy) = (Math.Sign(other.X - X), Math.Sign(other.Y - Y));
        return Navigate(point => point.Delta(dx, dy)).TakeWhileInclusive(point => point != other);
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

public static class PointExtensions
{
    public static int ManhattanDistance(this Point a, Point b)
    {
        return Math.Abs(b.X - a.X) + Math.Abs(b.Y - a.Y);
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

public class DictionaryWithDefault<TKey, TValue> : Dictionary<TKey, TValue> where TKey : notnull
{
    private readonly Func<TValue> defaultInitializer;

    public DictionaryWithDefault(TValue defaultValue)
    {
        this.defaultInitializer = () => defaultValue;
    }

    public DictionaryWithDefault(Func<TValue> defaultInitializer)
    {
        this.defaultInitializer = defaultInitializer;
    }

    public new TValue this[TKey key]
    {
        get
        {
            if (!ContainsKey(key))
            {
                Add(key, defaultInitializer());
            }
            return base[key];
        }
        set
        {
            base[key] = value;
        }
    }
}

public class CircularEnumerator<T>(IEnumerable<T> enumerable)
{
    private IEnumerator<T> _enumerator = enumerable.GetEnumerator();

    public T Next()
    {
        if (_enumerator.MoveNext())
        {
            return _enumerator.Current;
        }
        _enumerator.Reset();
        _enumerator.MoveNext();
        return _enumerator.Current;
    }

    internal void Reset()
    {
        _enumerator.Reset();
    }
}

public class Counter<T> where T : notnull
{
    public readonly DictionaryWithDefault<T, int> Counts = new(0);

    public Counter(IEnumerable<T> initial)
    {
        foreach (var value in initial)
        {
            Add(value);
        }
    }

    public int Count => Counts.Count;

    public void Add(T value)
    {
        Counts[value]++;
    }

    public void Remove(T value)
    {
        Counts[value]--;
        if (Counts[value] == 0)
        {
            Counts.Remove(value);
        }
    }
}

public static class Graph
{
    public static Dictionary<T, int> ShortestPath<T>(T startPoint,
        Func<T, IEnumerable<T>> getNeighbours,
        Func<T, T, int> getDistance,
        Func<T, bool>? isTarget = null) where T : notnull
    {
        var distances = new Dictionary<T, int>
        {
            [startPoint] = 0
        };
        var queue = new Queue<T>();
        queue.Enqueue(startPoint);
        while (queue.Count > 0)
        {
            var newQueue = new Queue<T>();
            foreach (var point in queue)
            {
                foreach (var neighbour in getNeighbours(point))
                {
                    var distance = distances[point] + getDistance(point, neighbour);
                    if (!distances.ContainsKey(neighbour) || distance < distances[neighbour])
                    {
                        distances[neighbour] = distance;
                        if (isTarget != null && isTarget(neighbour))
                        {
                            return distances;
                        }
                        newQueue.Enqueue(neighbour);
                    }
                }
            }
            queue = newQueue;
        }

        return distances;
    }

    public static IEnumerable<TState> Traverse<TState>(TState initialState,
        Func<TState, IEnumerable<TState>> getNewStates, bool useUniqueStates = false)
    {
        var states = new Queue<TState>();
        states.Enqueue(initialState);
        var visited = new HashSet<TState> { initialState };
        while (states.Count > 0)
        {
            var state = states.Dequeue();
            yield return state;
            foreach (var newState in getNewStates(state).Where(newState => !visited.Contains(newState)))
            {
                visited.Add(newState);
                states.Enqueue(newState);
            }
        }
    }

    public static IEnumerable<T> Walk<T>(T start, Func<T, bool> hasReachedEnd, Func<T, T> next)
    {
        for (var node = start; !hasReachedEnd(node); node = next(node))
        {
            yield return node;
        }
    }
}

public static class TupleHelpers
{
    public static bool IsOverlapping((long Start, long End) interval1, (long Start, long End) interval2)
    {
        return IsInInterval(interval1.Start, interval2)
               || IsInInterval(interval2.Start, interval1);
    }

    public static bool IsInInterval(long value, (long Start, long End) interval)
    {
        return value >= interval.Start && value <= interval.End;
    }
}