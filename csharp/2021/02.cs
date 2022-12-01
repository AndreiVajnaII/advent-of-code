using System.Reflection;
using Aoc;

namespace Aoc2021;

public class Solver202102 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        DepthCoords depthCoords = new();
        AimCoords aimCoords = new();
        foreach (var command in lines.Select(ParseCommand))
        {
            command(depthCoords);
            command(aimCoords);
        }
        return (depthCoords.Multiply(), aimCoords.Multiply());
    }

    private (string, int) SplitCommand(string command)
    {
        var terms = command.Split(" ");
        return (terms[0], int.Parse(terms[1]));
    }

    private Action<Coords> ParseCommand(string command)
    {
        var terms = command.Split(" ");
        var amount = int.Parse(terms[1]);
        var method = typeof(Coords).GetMethod(terms[0], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        return coords => method?.Invoke(coords, new object[] {amount});
    }
}

abstract class Coords
{
    protected int Depth = 0;
    protected int Position = 0;
    public abstract void Forward(int amount);
    public abstract void Down(int amount);
    public abstract void Up(int amount);
    public int Multiply() {
        return Depth * Position;
    }
}

class DepthCoords : Coords
{

    public override void Forward(int amount)
    {
        Position += amount;
    }

    public override void Down(int amount)
    {
        Depth += amount;
    }

    public override void Up(int amount)
    {
        Depth -= amount;
    }
}

class AimCoords : Coords
{
    private int Aim = 0;

    public override void Forward(int amount)
    {
        Position += amount;
        Depth += Aim * amount;
    }

    public override void Down(int amount)
    {
        Aim += amount;
    }

    public override void Up(int amount)
    {
        Aim -= amount;
    }
}