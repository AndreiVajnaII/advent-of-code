using Aoc;

namespace Aoc2022;

public class Solver202221 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var monkeys = lines.Select(line => line.Split(": ").AsTuple2())
            .ToDictionary(tuple => tuple.Item1, tuple => ParseMathOp(tuple.Item2));
        var root = (monkeys["root"] as MathOpNode)!;
        var result = (root.Evaluate(monkeys) as Result)!;
        monkeys["humn"] = new UnknownValue();
        var left = monkeys[root.LeftTerm].Evaluate(monkeys);
        var right = monkeys[root.RightTerm].Evaluate(monkeys);
        var resolvedValue = left is IUnknownResult unknown
            ? unknown.FindUnknown((right as Result)!.Value)
            : (right as IUnknownResult)!.FindUnknown((left as Result)!.Value);
        return (result.Value, resolvedValue);
    }

    private static IAbstractMathOpNode ParseMathOp(string mathOpStr)
    {
        var terms = mathOpStr.Split(" ");
        if (terms.Length == 1) return new ValueNode(long.Parse(terms[0]));
        return terms[1] switch
        {
            "+" => new MathOpNode(terms[0], terms[2], new Sum()),
            "*" => new MathOpNode(terms[0], terms[2], new Product()),
            "-" => new MathOpNode(terms[0], terms[2], new Difference()),
            "/" => new MathOpNode(terms[0], terms[2], new Division()),
            _ => throw new ArgumentException("invalid operation: " + terms[1])
        };
    }
    
    private interface IResult
    {
        
    }

    private class Result : IResult
    {
        public long Value { get; }
        
        public Result(long value)
        {
            Value = value;
        }
    }
    
    private interface IUnknownResult : IResult
    {
        long FindUnknown(long value);
    }

    private class UnknownResult : IUnknownResult
    {
        public long FindUnknown(long value)
        {
            return value;
        }
    }

    private class UnknownResultOperation : IUnknownResult
    {
        private readonly IResult left;
        private readonly IResult right;
        private readonly IMathOperation operation;

        public UnknownResultOperation(IResult left, IResult right, IMathOperation operation)
        {
            this.left = left;
            this.right = right;
            this.operation = operation;
        }

        public long FindUnknown(long value)
        {
            return left is IUnknownResult unknown
                ? unknown.FindUnknown(operation.FindLeft(value, (right as Result)!.Value))
                : (right as IUnknownResult)!.FindUnknown(operation.FindRight(value, (left as Result)!.Value));
        }
    }

    private interface IAbstractMathOpNode
    {
        IResult Evaluate(IDictionary<string, IAbstractMathOpNode> mathOpNodes);
    }

    private class ValueNode : IAbstractMathOpNode
    {
        private readonly long value;

        public ValueNode(long value)
        {
            this.value = value;
        }

        public IResult Evaluate(IDictionary<string, IAbstractMathOpNode> mathOpNodes)
        {
            return new Result(value);
        }
    }

    private class MathOpNode : IAbstractMathOpNode
    {
        private readonly IMathOperation operation;
        public string LeftTerm { get; }
        public string RightTerm { get; }

        public MathOpNode(string leftTerm, string rightTerm, IMathOperation operation)
        {
            LeftTerm = leftTerm;
            RightTerm = rightTerm;
            this.operation = operation;
        }

        public IResult Evaluate(IDictionary<string, IAbstractMathOpNode> mathOpNodes)
        {
            var left = mathOpNodes[LeftTerm].Evaluate(mathOpNodes);
            var right = mathOpNodes[RightTerm].Evaluate(mathOpNodes);
            return left is Result leftResult && right is Result rightResult
                ? new Result(operation.Evaluate(leftResult, rightResult))
                : new UnknownResultOperation(left, right, operation);
        }
    }
    
    private class UnknownValue : IAbstractMathOpNode
    {
        public IResult Evaluate(IDictionary<string, IAbstractMathOpNode> mathOpNodes)
        {
            return new UnknownResult();
        }
    }

    private interface IMathOperation
    {
        long Evaluate(Result left, Result right);
        long FindLeft(long value, long right);
        long FindRight(long value, long left);
    }

    private class Sum : IMathOperation
    {
        public long Evaluate(Result left, Result right) => left.Value + right.Value;
        public long FindLeft(long value, long right) => value - right;
        public long FindRight(long value, long left) => value - left;
    }

    private class Product : IMathOperation
    {
        public long Evaluate(Result left, Result right) => left.Value * right.Value;
        public long FindLeft(long value, long right) => value / right;
        public long FindRight(long value, long left) => value / left;
    }

    private class Difference : IMathOperation
    {
        public long Evaluate(Result left, Result right) => left.Value - right.Value;
        public long FindLeft(long value, long right) => value + right;
        public long FindRight(long value, long left) => left - value;
    }

    private class Division : IMathOperation
    {
        public long Evaluate(Result left, Result right) => left.Value / right.Value;
        public long FindLeft(long value, long right) => value * right;
        public long FindRight(long value, long left) => left / value;
    }

}