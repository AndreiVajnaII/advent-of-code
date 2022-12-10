using Aoc;

namespace Aoc2022;

public class Solver202210 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var cpu = new Cpu(40, 6);
        var cycles = lines.Select(line =>
        {
            var terms = line.Split(" ");
            return terms[0] switch
            {
                "noop" => cpu.Noop(),
                "addx" => cpu.AddX(int.Parse(terms[1])),
                _ => throw new ArgumentException("invalid command: " + line)
            };
        }).Flatten().ToArray();

        return (cycles.WithIndex().TakeEvery(40, 19).Take(6).Select(SignalStrength).Sum(),
            cpu.Screen);
    }

    private static int SignalStrength((int x, int index) cycle) => cycle.x * (cycle.index + 1);

    private class Cpu
    {
        private int x = 1;
        private readonly Grid2D<char> screen;
        private int cycle;
        private int row;

        public Cpu(int width, int height)
        {
            screen = new Grid2D<char>(width, height);
        }
        
        public string Screen => string.Join(Environment.NewLine, screen.GridValueEnumerable()
            .Select(line => string.Join("", line)));

        public IEnumerable<int> Noop()
        {
            Plot();
            yield return x;
        }

        public IEnumerable<int> AddX(int value)
        {
            Plot();
            yield return x;
            Plot();
            yield return x;
            x += value;
        }
        
        private void Plot()
        {
            screen.SetValueAt(new Point(cycle, row),
                x == cycle || x - 1 == cycle || x + 1 == cycle ? '#' : '.');
            if (++cycle == screen.Width)
            {
                cycle = 0;
                row++;
            }
        }
    }
}