using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Aoc;

namespace Aoc2023;

public class Solver202302 : ISolver
{
    private static readonly Regex gameRegex = new(@"Game (\d+): (.+)");
    private static readonly string[] colours = new[] { "red", "green", "blue" };

    public dynamic Solve(string[] lines)
    {
        var games = lines.Select(ParseGame);
        return (
            games
                .Where(game => game.Turns.All(IsPossible))
                .Sum(game => game.Id),
            games
                .Select(MinimumSet)
                .Select(EnumerableExtensions.Product)
                .Sum()
        );
    }

    private static bool IsPossible(Turn turn)
    {
        return turn.GetCount("red") <= 12
          && turn.GetCount("green") <= 13
          && turn.GetCount("blue") <= 14;
    }

    private int[] MinimumSet(Game game)
    {
        return colours.Select(colour => game.Turns.Max(turn => turn.GetCount(colour))).ToArray();
    }

    private Game ParseGame(string line)
    {
        var match = gameRegex.Match(line);
        return new Game
        {
            Id = int.Parse(match.Groups[1].Value),
            Turns = ParseTurns(match.Groups[2].Value)
        };
    }

    private Turn[] ParseTurns(string turnsString)
    {
        return turnsString.Split("; ").Select(ParseTurn).ToArray();
    }

    private Turn ParseTurn(string turnString)
    {
        return new Turn(turnString.Split(", ")
            .Select(countString => countString.Split(" "))
            .ToImmutableDictionary(split => split[1], split => int.Parse(split[0])));
    }
}

internal class Game
{
    public int Id { get; set; }
    public Turn[] Turns { get; set; } = Array.Empty<Turn>();
}

internal class Turn
{
    private readonly IImmutableDictionary<string, int> _counts;

    public Turn(IImmutableDictionary<string, int> counts)
    {
        _counts = counts;
    }

    public int GetCount(string colour) => _counts.GetValueOrDefault(colour);
}