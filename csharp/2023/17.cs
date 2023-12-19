using Aoc;
using static Aoc.Grid2D;
using static Aoc.Helpers;

namespace Aoc2023;

public class Solver202317 : ISolver
{

    public dynamic Solve(string[] lines)
    {
        var grid = lines.ToGrid(ParseChar);

        return (
            CalculateMinimumHeatLoss(grid, (oldState, newState) => newState.StraightLength <= 3),
            CalculateMinimumHeatLoss(grid, (oldState, newState) =>
               oldState.Direction == (0, 0)
               || (newState.StraightLength == 1 && oldState.StraightLength >= 4)
               || (newState.StraightLength > 1 && newState.StraightLength <= 10),
               state => state.StraightLength >= 4)
        );
    }

    private static int CalculateMinimumHeatLoss(Grid2D<int> grid, Func<HeatLossState, HeatLossState, bool> moveLogic,
        Func<HeatLossState, bool>? endLogic = null)
    {
        var heatLoss = new Dictionary<HeatLossState, int>();
        var initialState = new HeatLossState(new Point(0, 0), (0, 0), 0);
        heatLoss[initialState] = 0;

        foreach (var _ in Graph.Traverse(
            initialState,
            state => OrthogonalNeighbours
                .Where(direction => direction != OppositeDirection(state.Direction))
                .Select(direction => new HeatLossState(
                    state.Position.Move(direction), direction,
                    1 + (state.Direction == direction ? state.StraightLength : 0)))
                .Where(newState => grid.IsInBounds(newState.Position))
                .Where(newState => moveLogic(state, newState))
                .Where(newState => !heatLoss.ContainsKey(newState) ||
                    heatLoss[state] + grid[newState.Position] < heatLoss[newState])
                .Do(newState => heatLoss[newState] = heatLoss[state] + grid[newState.Position])
        )) ;

        return heatLoss.Where(pair => pair.Key.Position == grid.BottomRight
                && (endLogic?.Invoke(pair.Key) ?? true))
            .Min(pair => pair.Value);
    }
}

internal class HeatLossState(Point position, Direction direction, int straightLength)
{
    public Point Position { get; private set; } = position;
    public Direction Direction { get; private set; } = direction;
    public int StraightLength { get; private set; } = straightLength;

    public override bool Equals(object? obj)
    {
        if (obj is null || GetType() != obj.GetType()) return false;

        var other = (HeatLossState)obj;
        return other.Position == Position &&
            other.Direction == Direction &&
            other.StraightLength == StraightLength;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Position, Direction, StraightLength);
    }
}