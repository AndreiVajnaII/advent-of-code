using Aoc;

namespace Aoc2021;

public class Solver202104 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var numbers = lines[0].Split(',').Select(int.Parse);
        var cards = new List<BingoCard>();
        for (uint row = 2; row < lines.Length; row++)
        {
            var cardRows = new int[5][];
            for (uint i = 0; i < 5; i++, row++)
            {
                cardRows[i] = lines[row].Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse).ToArray();
            }
            cards.Add(new BingoCard(cardRows));
        }

        var plays = from play in PlayBingo(cards, numbers)
                    where play.Winner is not null
                    select play.Winner.Score() * play.number;
        return (plays.First(), plays.Last());

    }

    private IEnumerable<(BingoCard? Winner, int number)> PlayBingo(List<BingoCard> cards, IEnumerable<int> numbers)
    {
        foreach (var number in numbers)
        {
            foreach (var card in cards)
            {
                if (!card.IsWinner())
                {
                    card.Mark(number);
                    if (card.IsWinner())
                    {
                        yield return (card, number);
                    }
                }
            }
        }
    }
}

class BingoCard
{
    private BingoTile[,] tiles;
    private bool isWinner = false;
    private bool isDirty = true;

    public BingoCard(int[][] rows)
    {
        tiles = new BingoTile[5, 5];
        for (uint i = 0; i < tiles.GetLength(0); i++)
        {
            for (uint j = 0; j < tiles.GetLength(1); j++)
            {
                tiles[i, j] = new BingoTile(rows[i][j]);
            }
        }
    }

    public void Mark(int number)
    {
        var tile = tiles.Cast<BingoTile>().FirstOrDefault(tile => tile.Number == number);
        if (tile is not null)
        {
            tile.Mark();
            isDirty = true;
        }
    }

    public bool IsWinner()
    {
        if (!isDirty)
        {
            return isWinner;
        }
        isDirty = false;
        for (uint row = 0; row < tiles.GetLength(0); row++)
        {
            bool winner = true;
            for (uint col = 0; col < tiles.GetLength(1); col++)
            {
                if (!tiles[row, col].Marked)
                {
                    winner = false;
                }
            }
            if (winner)
            {
                return isWinner = winner;
            }
        }
        for (uint col = 0; col < tiles.GetLength(1); col++)
        {
            bool winner = true;
            for (uint row = 0; row < tiles.GetLength(0); row++)
            {
                if (!tiles[row, col].Marked)
                {
                    winner = false;
                }
            }
            if (winner)
            {
                return isWinner = winner;
            }
        }
        isWinner = false;
        return false;
    }

    public int Score()
    {
        return (from tile in tiles.Cast<BingoTile>()
                where !tile.Marked
                select tile.Number
                ).Sum();
    }
}

class BingoTile
{
    public int Number { get; private set; }
    public bool Marked { get; private set; }
    public BingoTile(int number)
    {
        Number = number;
        Marked = false;
    }

    public void Mark() => Marked = true;
}