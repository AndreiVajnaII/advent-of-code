public class Solver202107 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var crabs = lines[0].Split(",").Select(int.Parse);
        var fuelEfficiencyCalculator = new FuelEfficiencyCalculator(crabs);
        return (
            fuelEfficiencyCalculator.Calculate(Math.Abs),
            fuelEfficiencyCalculator.Calculate(steps => steps * (steps + 1) / 2)
        );
    }

}

class FuelEfficiencyCalculator
{
    private IEnumerable<int> crabs;
    private int min, max;
    public FuelEfficiencyCalculator(IEnumerable<int> crabs)
    {
        this.crabs = crabs;
        this.min = crabs.Min();
        this.max = crabs.Max();
    }

    public int Calculate(Func<int, int> consumptionRate)
    {
        return Enumerable.Range(min, max - min + 1)
            .Select(CountConsumption(consumptionRate))
            .Min();
    }

    private Func<int, int> CountConsumption(Func<int, int> consumptionRate)
    {
        return (int position) => crabs.Select(crab => consumptionRate(Math.Abs(crab - position))).Sum();
    }
}