using Aoc;

namespace Aoc2022;

public class Solver202202 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var rounds = lines.Select(line => line.Split(" ").AsTuple2()).ToArray();
        return (
            Play(rounds, Evaluate1),
            Play(rounds, Evaluate2)
        );
    }

    private static int Play(IEnumerable<(string, string)> rounds, Func<(string, string), (Shape, Shape)> evaluate)
    {
        return rounds.Select(evaluate).Select(Score).Sum();
    }

    private static (Shape, Shape) Evaluate1((string their, string mine) round)
    {
        return (ToShape(round.their), ToShape(round.mine));
    }

    private static (Shape, Shape) Evaluate2((string their, string outcome) round)
    {
        var theirShape = ToShape(round.their);
        var myShape = ComputeShape(theirShape, round.outcome);
        
        return (theirShape, myShape);
    }

    private static Shape ComputeShape(Shape theirShape, string outcome) => outcome switch
    {
        "X" => LoserOf(theirShape),
        "Y" => theirShape,
        "Z" => LoserOf(LoserOf(theirShape)),
        _ => throw new ArgumentException("Invalid value " + outcome)
    };

    private static int Score((Shape their, Shape mine) round)
    {
        return ((int)round.mine) + OutcomeScore(round);
    }

    private static int OutcomeScore((Shape their, Shape mine) round)
    {
        if (round.their == round.mine) return 3;
        else if (round.their == LoserOf(round.mine)) return 6;
        else return 0;
    }

    private static Shape LoserOf(Shape winner) => winner switch
    {
        Shape.Rock => Shape.Scissors,
        Shape.Paper => Shape.Rock,
        Shape.Scissors => Shape.Paper,
        _ => throw new ArgumentException("Invalid shape " + winner)
    };

    private static Shape ToShape(string v) => v switch
    {
        "A" => Shape.Rock,
        "B" => Shape.Paper,
        "C" => Shape.Scissors,
        "X" => Shape.Rock,
        "Y" => Shape.Paper,
        "Z" => Shape.Scissors,
        _ => throw new ArgumentException("Invalid shape " + v)
    };

    private enum Shape {Rock = 1, Paper, Scissors}
}
