public class Solver202106 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var timers = lines[0].Split(',').Select(int.Parse);
        return (CountFish(timers, 80), CountFish(timers, 256));
    }

    private long CountFish(IEnumerable<int> timers, int days) {
        var timerCounts = new long[9];
        foreach (var timer in timers)
        {
            timerCounts[timer]++;
        }
        while (days-- > 0)
        {
            var newborn = timerCounts[0];
            timerCounts = timerCounts.Skip(1).Append(newborn).ToArray();
            timerCounts[6] += newborn;
        }
        return timerCounts.Sum();
    }
}