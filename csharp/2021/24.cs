using Aoc;

namespace Aoc2021;

public class Solver202124 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var largest = "91599994399395";
        var smallest = "71111591176151";
        var monad = new MONAD(lines);
        return monad.Test(largest) && monad.Test(smallest)
            ? (largest, smallest)
            : "Error";
    }
}

class MONAD
{
    private readonly string[] program;
    private readonly ALU alu;

    public MONAD(string[] program)
    {
        this.program = program;
        this.alu = new();
    }

    public bool Test(string modelNr)
    {
        var input = new Queue<int>(modelNr.AsEnumerable().Select(Helpers.ParseChar));
        alu.Run(program, input);
        return alu.Value("z") == 0;
    }
}

class ALU
{
    private readonly int[] registers = new int[4];

    public void Run(IEnumerable<string> instructions, Queue<int> input)
    {
        for (int i = 0; i < registers.Length; i++)
        {
            registers[i] = 0;
        }
        foreach (var instruction in instructions)
        {
            Execute(instruction.Split(' '), input);
        }
    }

    private void Execute(string[] instruction, Queue<int> input)
    {
        switch (instruction[0])
        {
            case "inp":
                registers[VarIndex(instruction[1])] = input.Dequeue();
                break;
            case "add":
                registers[VarIndex(instruction[1])] += Value(instruction[2]);
                break;
            case "mul":
                registers[VarIndex(instruction[1])] *= Value(instruction[2]);
                break;
            case "div":
                registers[VarIndex(instruction[1])] /= Value(instruction[2]);
                break;
            case "mod":
                registers[VarIndex(instruction[1])] %= Value(instruction[2]);
                break;
            case "eql":
                registers[VarIndex(instruction[1])] =
                    registers[VarIndex(instruction[1])] == Value(instruction[2]) ? 1 : 0;
                break;
        }
    }

    public int Value(string v)
    {
        return Char.IsLower(v[0]) ? registers[VarIndex(v)] : int.Parse(v);
    }

    private int VarIndex(string v)
    {
        return v[0] - 'w';
    }
}