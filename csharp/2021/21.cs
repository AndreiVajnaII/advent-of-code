using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Aoc;

namespace Aoc2021;

public class Solver202121 : ISolver
{
    private static readonly Regex inputRegex = new(@"Player \d starting position: (\d+)", RegexOptions.Compiled);
    public dynamic Solve(string[] lines)
    {
        int[] positions = lines.Select(Parse).ToArray();
        var die = new DeterministicDie();
        var deterministicGame = new Game(die, positions, 1000);
        deterministicGame.Play();
        var diracGame = new Game(new DiracDie(), positions, 21);
        diracGame.Play();
        var wonByPlayer1 = diracGame.completedStates
                .Where(state => state.Key.Scores[0] > state.Key.Scores[1])
                .Sum(state => state.Value);
        return (deterministicGame.completedStates.Single().Key.Scores.Min() * die.Rolls,
            Math.Max(wonByPlayer1, diracGame.completedStates.Values.Sum() - wonByPlayer1));
    }

    private int Parse(string s)
    {
        var m = inputRegex.Match(s);
        return int.Parse(m.Groups[1].Value);
    }
}

class Game
{
    private readonly Die die;
    private readonly int targetScore;
    private Dictionary<GameState, long> states = new();
    public Dictionary<GameState, long> completedStates { get; private set; } = new();
    public Game(Die die, int[] startingPositions, int targetScore)
    {
        this.die = die;
        this.targetScore = targetScore;
        states[new GameState(startingPositions)] = 1L;
    }

    public void Play()
    {
        while (states.Count() > 0)
        {
            states = TakeTurn();
        }
    }

    public Dictionary<GameState, long> TakeTurn()
    {
        var newStates = new Dictionary<GameState, long>();
        foreach (var (state, times) in states.SelectMany(state => RollDice()
            .Select(roll => (state.Key.Move(roll.Value, targetScore), roll.Times * state.Value))))
        {
            if (state.Won)
            {
                if (!completedStates.ContainsKey(state))
                {
                    completedStates[state] = 0;
                }
                completedStates[state] += times;
            }
            else
            {
                if (!newStates.ContainsKey(state))
                {
                    newStates[state] = 0;
                }
                newStates[state] += times;
            }
        }
        return newStates;
    }

    public IEnumerable<(int Value, int Times)> RollDice()
    {
        return die.Roll(3);
    }
}

struct GameState
{
    private readonly ImmutableArray<int> positions;
    public ImmutableArray<int> Scores { get; }
    private readonly int player;
    public bool Won { get; private set; }

    public GameState(int[] startingPositions)
    {
        positions = startingPositions.ToImmutableArray();
        Scores = new int[positions.Length].ToImmutableArray();
        player = 0;
        Won = false;
    }

    private GameState(int[] positions, int[] scores, int player, bool won)
    {
        this.positions = positions.ToImmutableArray();
        Scores = scores.ToImmutableArray();
        this.player = player;
        this.Won = won;
    }

    public GameState Move(int spaces, int targetScore)
    {
        int[] newPositions = positions.ToArray();
        int[] newScores = Scores.ToArray();
        newPositions[player] = (positions[player] - 1 + spaces) % 10 + 1;
        newScores[player] += newPositions[player];
        return new GameState(newPositions, newScores, (player + 1) % Scores.Length,
            newScores[player] >= targetScore);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            positions.Aggregate(HashCode.Combine),
            Scores.Aggregate(HashCode.Combine),
            player);
    }

    public override bool Equals(object? obj)
    {
        return obj is GameState state &&
                positions.SequenceEqual(state.positions) &&
                Scores.SequenceEqual(state.Scores) &&
                player == state.player;
    }
}

abstract class Die
{
    public int Rolls { get; protected set; } = 0;

    public abstract IEnumerable<(int Value, int Times)> Roll(int times);

}

class DeterministicDie : Die
{
    public override IEnumerable<(int Value, int Times)> Roll(int times)
    {
        yield return (Enumerable.Range(0, times).Select(_ => Roll()).Sum(), 1);
    }

    private int Roll()
    {
        Rolls++;
        return (Rolls - 1) % 100 + 1;
    }
}

class DiracDie : Die
{
    private Dictionary<int, IEnumerable<(int Value, int Times)>> results = new();
    public override IEnumerable<(int Value, int Times)> Roll(int times)
    {
        if (results.ContainsKey(times))
        {
            return results[times];
        }
        IEnumerable<IEnumerable<int>> combinedRolls = Roll().Combine(Roll());
        for (int i = 2; i < times; i++)
        {
            combinedRolls = combinedRolls.Combine(Roll());
        }
        var sums = combinedRolls.Select(rolls => rolls.Sum()).ToArray();
        var groups = sums.GroupBy(value => value).ToArray();
        var rolls = groups.Select(group => (group.Key, group.Count())).ToArray();
        results[times] = rolls;
        return rolls;
    }

    private IEnumerable<int> Roll()
    {
        yield return 1;
        yield return 2;
        yield return 3;
    }
}