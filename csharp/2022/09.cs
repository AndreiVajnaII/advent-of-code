using Aoc;

namespace Aoc2022;

public class Solver202209 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var moves = lines.Select(Parse).Flatten().ToArray();
        return (
            MoveRope(moves, 1),
            MoveRope(moves, 9)
        );
    }

    private static dynamic MoveRope(IEnumerable<Func<Point, Point>> moves, int knotCount)
    {
        var head = new Point(0, 0);
        var knots = Enumerable.Repeat(new Point(0, 0), knotCount).ToArray().AsSpan();
        var visited = new HashSet<Point> {knots[^1]};

        foreach (var move in moves)
        {
            head = move(head);
            var prev = head;
            foreach (ref var tail in knots)
            {
                tail = MoveTail(tail, prev);
                prev = tail;
            }

            visited.Add(knots[^1]);
        }

        return visited.Count;
    }

    private static Point MoveTail(Point tail, Point head)
    {
        var dx = head.X - tail.X;
        var dy = head.Y - tail.Y;
        var absDx = Math.Abs(dx);
        var absDy = Math.Abs(dy);
        return absDx > 1 || absDy > 1
            ? tail.Delta(dx == 0 ? 0 : dx / absDx, dy == 0 ? 0 : dy / absDy)
            : tail;
    }

    private static IEnumerable<Func<Point, Point>> Parse(string line)
    {
        var terms = line.Split(" ");
        return Enumerable.Repeat(terms[0] switch
        {
            "R" => MoveCommand(1, 0),
            "U" => MoveCommand(0, -1),
            "L" => MoveCommand(-1, 0),
            "D" => MoveCommand(0, 1),
            _ => throw new ArgumentException("Invalid command " + line)
        }, int.Parse(terms[1]));
    }

    private static Func<Point, Point> MoveCommand(int dx, int dy)
    {
        return point => point.Delta(dx, dy);
    }
}