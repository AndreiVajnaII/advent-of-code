using System.Text;
using Aoc;

namespace Aoc2021;

public class Solver202103 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var bitArray = lines.Select(line => line.ToCharArray()).ToArray();
        StringBuilder gamma = new();
        StringBuilder epsilon = new();
        for (uint pos = 0; pos < bitArray[0].Length; pos++)
        {
            var (mc, lc) = AnalyzeBit(bitArray, pos);
            gamma.Append(mc);
            epsilon.Append(lc);
        }
        string O2 = FindRating(bitArray, (bitArray, pos) => {
            var (mc, _) = AnalyzeBit(bitArray, pos);
            return mc;
        });
        string CO2 = FindRating(bitArray, (bitArray, pos) => {
            var (_, lc) = AnalyzeBit(bitArray, pos);
            return lc;
        });

        return (
            Helpers.ParseBinary(gamma.ToString()) * Helpers.ParseBinary(epsilon.ToString()),
            Helpers.ParseBinary(O2) * Helpers.ParseBinary(CO2)
        );
    }

    private (char MostCommon, char LeastCommon) AnalyzeBit(char[][] bitArray, uint pos) {
        int count0 = 0;
        for (uint row = 0; row < bitArray.Length; row++)
        {
            count0 += bitArray[row][pos] == '0' ? 1 : -1;
        }
        return count0 > 0 ? ('0', '1') : ('1', '0');
    }

    private string FindRating(char[][] bitArray, Func<char[][], uint, char> findFilterBit) {
        for (uint pos = 0; pos < bitArray[0].Length && bitArray.Length > 1; pos++) {
            var filterBit = findFilterBit(bitArray, pos);
            bitArray = bitArray.Where(number => number[pos] == filterBit).ToArray();
        }
        return new String(bitArray[0]);
    }
}