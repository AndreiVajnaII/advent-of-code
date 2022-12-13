using Aoc;
using static Aoc.Graph;

namespace Aoc2022;

public class Solver202212 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var heights = new Grid2D<char>(lines.ToArray2D());
        var startPoint = heights.PositionOf('S');
        heights[startPoint] = 'a';
        var endPoint = heights.PositionOf('E');
        heights[endPoint] = 'z';

        return (
            ShortestPath(startPoint,
                point => heights.AdjacentPoints(point, Grid2D.OrthogonalNeighbours)
                    .Where(adjacentPoint => heights[adjacentPoint] - heights[point] <= 1),
                (_, _) => 1,
                point => point == endPoint)[endPoint],
            ShortestPath(endPoint,
                    point => heights.AdjacentPoints(point, Grid2D.OrthogonalNeighbours)
                        .Where(adjacentPoint => heights[adjacentPoint] - heights[point] >= -1),
                    (_, _) => 1)
                .Where(entry => heights[entry.Key] == 'a')
                .Min(entry => entry.Value)
        );
    }
}