using Aoc;

namespace Aoc2023;

public class Solver202304 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var score = new int[11];
        score[0] = 0;
        score[1] = 1;
        for (var i = 2; i < score.Length; i++)
        {
            score[i] = 2 * score[i - 1];
        }

        var cardValues = lines.Select(CardValue).ToArray();

        var scores = new Dictionary<int, int>();

        return (
            cardValues
                .Select(value => score[value])
                .Sum(),
            cardValues
                .Select((value, i) => Play(cardValues, i, scores))
                .Sum()
        );
    }

    private int CardValue(string line)
    {
        var numbersStrings = line.Split(": ")[1].Split(" | ");
        var winners = ParseNumbers(numbersStrings[0]);
        var mine = ParseNumbers(numbersStrings[1]);
        return winners.Intersect(mine).Count();
    }

    private static int Play(int[] cardValues, int start, Dictionary<int, int> scores)
    {
        if (scores.TryGetValue(start, out var cachedValue)) {
            return cachedValue;
        }
        var value = cardValues[start];
        var sum = 1;
        for (int i = 1; i <= value; i++)
        {
            sum += Play(cardValues, start + i, scores);
        }

        scores[start] = sum;
        return sum;
    }

    private static HashSet<int> ParseNumbers(string numbersString)
    {
        return numbersString.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToHashSet();
    }
}
