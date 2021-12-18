public class Solver202118 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        return (lines.Select(ParseSnailfishNumber).Aggregate((a, b) => a + b).Magnitude,
            lines.Pair(item => lines.Without(item))
                .Select(pair => (ParseSnailfishNumber(pair.Item1) + ParseSnailfishNumber(pair.Item2)))
                .Select(n => n.Magnitude).Max());
    }

    private static SnailfishNumber ParseSnailfishNumber(string s)
    {
        return ParseSnailfishNumber(s.GetEnumerator());
    }
    private static SnailfishNumber ParseSnailfishNumber(CharEnumerator stream)
    {
        stream.MoveNext();
        if (stream.Current == '[')
        {
            var left = ParseSnailfishNumber(stream);
            stream.MoveNext(); // ,
            var right = ParseSnailfishNumber(stream);
            stream.MoveNext(); // ]
            return new Pair(left, right);
        }
        else
        {
            return new RegularNumber(Helpers.ParseChar(stream.Current));
        }
    }
}

public abstract class SnailfishNumber
{
    public abstract int Magnitude { get; }

    public static SnailfishNumber operator +(SnailfishNumber a, SnailfishNumber b)
    {
        return new Pair(a, b).Reduce();
    }

    public SnailfishNumber Reduce()
    {
        var infoList = Traverse().ToList();
        bool done = false;
        while (!done)
        {
            done = true;
            for (int i = 0; i < infoList.Count; i++)
            {
                var info = infoList[i];
                if (info.ShouldExplode)
                {
                    var left = infoList[i - 1];
                    var right = infoList[i + 1];
                    var newNumber = new RegularNumber(0);
                    if (info.IsRightChild)
                    {
                        info.Parent!.Right = newNumber;
                    }
                    else 
                    {
                        info.Parent!.Left = newNumber;
                    }
                    if (i - 3 >= 0)
                    {
                        infoList[i - 3].Value!.Number += left.Value!.Number;
                    }
                    if (i + 3 < infoList.Count)
                    {
                        infoList[i + 3].Value!.Number += right.Value!.Number;
                    }
                    info.Value = newNumber;
                    infoList.RemoveAt(i+1);
                    infoList.RemoveAt(i-1);
                    done = false;
                }
            }
            for (int i = 0; i < infoList.Count; i++)
            {
                var info = infoList[i];
                if (info.ShouldSplit)
                {
                    int value = info.Value!.Number;
                    var left = new RegularNumber(value / 2);
                    var right = new RegularNumber(value - left.Number);
                    var splitPair = new Pair(left, right);
                    if (info.IsRightChild)
                    {
                        info.Parent!.Right = splitPair;
                    }
                    else
                    {
                        info.Parent!.Left = splitPair;
                    }
                    infoList.RemoveAt(i);
                    infoList.Insert(i, new SnailfishNumberInfo(right, splitPair, info.Depth + 1, true));
                    infoList.Insert(i, new SnailfishNumberInfo(null, info.Parent, info.Depth, info.IsRightChild));
                    infoList.Insert(i, new SnailfishNumberInfo(left, splitPair, info.Depth + 1, false));
                    done = false;
                    break;
                }
            }
        }
        return this;
    }

    public IEnumerable<SnailfishNumberInfo> Traverse() => Traverse(0, null, false);

    public abstract IEnumerable<SnailfishNumberInfo> Traverse(int depth, Pair? parent, bool isRightChild);
}

public class Pair : SnailfishNumber
{
    public SnailfishNumber Left { get; set; }
    public SnailfishNumber Right { get; set; }

    public override int Magnitude => 3 * Left.Magnitude + 2 * Right.Magnitude;

    public Pair(SnailfishNumber left, SnailfishNumber right)
    {
        Left = left;
        Right = right;
    }

    public override IEnumerable<SnailfishNumberInfo> Traverse(int depth, Pair? parent, bool isRightChild)
    {
        return Left.Traverse(depth + 1, this, false)
            .Append(new SnailfishNumberInfo(null, parent, depth, isRightChild))
            .Concat(Right.Traverse(depth + 1, this, true));
    }

    public override string ToString()
    {
        return $"[{Left},{Right}]";
    }
}

public class RegularNumber : SnailfishNumber
{
    public int Number { get; set; }

    public override int Magnitude { get => Number; }

    public RegularNumber(int number)
    {
        Number = number;
    }

    public override IEnumerable<SnailfishNumberInfo> Traverse(int depth, Pair? parent, bool isRightChild)
    {
        yield return new SnailfishNumberInfo(this, parent, depth, isRightChild);
    }

    public override string ToString()
    {
        return Number.ToString();
    }
}
public class SnailfishNumberInfo
{
    public RegularNumber? Value { get; set; }
    public Pair? Parent { get; private set; }
    public int Depth { get; private set; }
    public bool IsRightChild { get; private set; }
    public bool ShouldExplode { get => Value is  null && Depth == 4; }
    public bool ShouldSplit { get => Value?.Number >= 10; }

    public SnailfishNumberInfo(RegularNumber? value, Pair? parent, int depth, bool isRightChild)
    {
        Value = value;
        Parent = parent;
        Depth = depth;
        IsRightChild = isRightChild;
    }

    public override string ToString()
    {
        return $"{(Value is null ? "-" : Value.Number.ToString())} {Depth}";
    }
}