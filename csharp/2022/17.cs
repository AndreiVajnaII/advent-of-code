using System.Collections.Immutable;
using Aoc;

namespace Aoc2022;

public class Solver202217 : ISolver
{
    private static readonly string[][] Shapes =
    {
        new[]
        {
            "####"
        },
        new[]
        {
            " # ",
            "###",
            " # "
        },
        new[]
        {
            "###",
            "  #",
            "  #"
        },
        new[]
        {
            "#",
            "#",
            "#",
            "#"
        },
        new[]
        {
            "##",
            "##"
        }
    };

    public dynamic Solve(string[] lines)
    {
        var slow = new Map(lines[0]);
        var fast = new Map(lines[0]);

        for (var i = 0; i < 2022; i++)
        {
            slow.DropRock();
            fast.DropRock();
        }

        var part1 = slow.Height;
        
        var done = false;
        var rockIncrement = 1;
        var heightIncrement = 1;
        while (!done)
        {
            slow.DropRock();
            fast.DropRock();
            fast.DropRock();
            if (slow.TopRow == fast.TopRow && slow.WindIndex == fast.WindIndex && slow.ShapeIndex == fast.ShapeIndex)
            {
                rockIncrement = fast.RockCount - slow.RockCount;
                heightIncrement = fast.Height - slow.Height;
                done = true;
            }
        }

        const long rockCount = 1000000000000L;
        var nrIncrements = (rockCount - slow.RockCount) / rockIncrement;
        for (var i = slow.RockCount + nrIncrements * rockIncrement; i < rockCount; i++)
        {
            slow.DropRock();
        }

        return (part1, slow.Height + nrIncrements * heightIncrement);
    }

    private class Map
    {
        private readonly IList<bool[]> map = new List<bool[]>();
        private readonly CircularList<char> winds;
        private readonly CircularList<string[]> shapes;

        public int Height => map.Count;
        public int RockCount;
        public int WindIndex => winds.Index;
        public int ShapeIndex => shapes.Index;

        public Map(string windSequence)
        {
            winds = new CircularList<char>(windSequence.ToImmutableArray());
            shapes = new CircularList<string[]>(Shapes.ToImmutableArray());
        }
        
        public void DropRock()
        {
            var x = 2;
            var y = Height + 3;
            var shape = shapes.Next();
            var done = false;
            while (!done)
            {
                var newX = x + (winds.Next() == '<' ? -1 : 1);
                if (CanMove(shape, newX, y))
                {
                    x = newX;
                }

                if (CanMove(shape, x, y - 1))
                {
                    y -= 1;
                }
                else
                {
                    done = true;
                }
            }

            SetShape(shape, x, y);
            RockCount++;
        }

        private bool CanMove(IReadOnlyList<string> shape, int shapeX, int shapeY)
        {
            if (shapeY < 0 || shapeX < 0 || shapeX + shape[0].Length > 7)
            {
                return false;
            }

            for (var y = 0; y < shape.Count; y++)
            {
                for (var x = 0; x < shape[y].Length; x++)
                {
                    if (shapeY + y < Height && shape[y][x] == '#' && map[shapeY + y][shapeX + x])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void SetShape(IReadOnlyList<string> shape, int shapeX, int shapeY)
        {
            for (var y = 0; y < shape.Count; y++)
            {
                if (shapeY + y == Height)
                {
                    map.Add(Enumerable.Repeat(false, 7).ToArray());
                }

                for (var x = 0; x < shape[y].Length; x++)
                {
                    if (shape[y][x] == '#')
                    {
                        map[shapeY + y][shapeX + x] = true;
                    }
                }
            }
        }

        private string RowAt(int y)
        {
            return string.Join("", map[y].Select(v => v ? '#' : '.'));
        }

        public string TopRow => RowAt(Height - 1);
    }

    private class CircularList<T>
    {
        private readonly ImmutableArray<T> list;

        public int Index { get; private set; }

        public CircularList(ImmutableArray<T> list)
        {
            this.list = list;
        }

        public T Next()
        {
            var item = list[Index];
            Index++;
            if (Index == list.Length)
            {
                Index = 0;
            }

            return item;
        }
    }
}