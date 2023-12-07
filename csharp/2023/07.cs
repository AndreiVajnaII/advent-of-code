using Aoc;

namespace Aoc2023;

public class Solver202307 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var bids = lines.Select(ParseBid);

        return (
            Score(bids, HandComparer.Regular),
            Score(bids, HandComparer.UseJoker)
        );
    }

    private static long Score(IEnumerable<(string Hand, long Bid)> bids, IComparer<string> handComparer)
    {
        return bids.OrderBy(bid => bid.Hand, handComparer)
            .Select((bid, i) => (i + 1) * bid.Bid)
            .Sum();
    }

    private (string Hand, long bid) ParseBid(string line)
    {
        var split = line.Split(" ");
        return (split[0], long.Parse(split[1]));
    }
}

internal class HandComparer : IComparer<string>
{
    public static readonly IComparer<string> Regular = new HandComparer(false);
    public static readonly IComparer<string> UseJoker = new HandComparer(true);
    private readonly bool _useJoker;

    private HandComparer(bool useJoker)
    {
        _useJoker = useJoker;
    }

    public int Compare(string? x, string? y)
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        var xValue = HandValue(x, _useJoker);
        var yValue = HandValue(y, _useJoker);

        if (xValue == yValue)
        {
            var (First, Second) = x.Zip(y).FirstOrDefault(pair => pair.First != pair.Second);
            return CardValue(First, _useJoker).CompareTo(CardValue(Second, _useJoker));
        }

        return xValue.CompareTo(yValue);
    }

    public static int HandValue(string hand, bool useJoker = false)
    {
        var cards = hand.ToCharArray();
        var counts = new Counter<char>(cards).Counts;
        var jokerCounts = 0;
        if (useJoker && counts.TryGetValue('J', out jokerCounts))
        {
            counts['J'] = 0;
        }
        var countValues = counts.Values.OrderDescending().ToList();
        countValues[0] += jokerCounts;
        if (countValues[0] == 5) return 7;
        if (countValues[0] == 4) return 6;
        if (countValues[0] == 3 && countValues[1] == 2) return 5;
        if (countValues[0] == 3) return 4;
        if (countValues[0] == 2 && countValues[1] == 2) return 3;
        if (countValues[0] == 2) return 2;
        return 1;
    }

    public static int CardValue(char card, bool useJoker = false) => card switch
    {
        '2' => 2,
        '3' => 3,
        '4' => 4,
        '5' => 5,
        '6' => 6,
        '7' => 7,
        '8' => 8,
        '9' => 9,
        'T' => 10,
        'J' => useJoker ? 1 : 11,
        'Q' => 12,
        'K' => 13,
        'A' => 14,
        _ => 0
    };
}