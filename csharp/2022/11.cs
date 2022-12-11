using System.Text.RegularExpressions;
using Aoc;
using static Aoc.Helpers;

namespace Aoc2022;

public class Solver202211 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var monkeyNotes = GroupLines(lines).Select(group => group.ToArray()).ToArray(); 
        var monkeys = monkeyNotes.Select(Monkey.Parse).ToArray();
        DoRounds(20, monkeys);
        
        var angryMonkeys = monkeyNotes.Select(Monkey.ParseAngry).ToArray();
        var modulo = angryMonkeys.Select(monkey => monkey.DivisibleBy).Product();
        DoRounds(10000, angryMonkeys, item => item % modulo);

        return (
            MonkeyBusiness(monkeys),
            MonkeyBusiness(angryMonkeys)
        );
    }

    private static void DoRounds(int times, IReadOnlyList<Monkey> monkeys, Func<long, long>? adjustItem = null)
    {
        times.TimesDo(() =>
        {
            foreach (var monkey in monkeys)
            {
                foreach (var (item, targetMonkey) in monkey.TakeTurn())
                {
                    monkeys[targetMonkey].GiveItem(adjustItem?.Invoke(item) ?? item);
                }
            }
        });
    }

    private static long MonkeyBusiness(IEnumerable<Monkey> monkeys)
    {
        return monkeys.OrderByDescending(monkey => monkey.InspectedItems).Select(monkey => monkey.InspectedItems)
            .Take(2).Product();
    }

    private class Monkey
    {
        private static readonly Regex ItemsRegex = new Regex("Starting items: (.*)$");
        private static readonly Regex OperationRegex = new Regex("Operation: new = (.*)$");
        private static readonly Regex TestRegex = new Regex("Test: divisible by (\\d+)");
        private static readonly Regex ThrowRegex = new Regex("throw to monkey (\\d+)");
        
        private readonly Queue<long> items;
        private readonly Func<long, long> operation;
        private readonly int ifTrue;
        private readonly int ifFalse;
        private readonly bool isNaughty;
        
        public readonly int DivisibleBy;

        public long InspectedItems { get; private set; }

        private Monkey(IEnumerable<long> initialItems, Func<long, long> operation, int divisibleBy,
            int ifTrue, int ifFalse, bool isNaughty)
        {
            items = new Queue<long>(initialItems);
            this.operation = operation;
            DivisibleBy = divisibleBy;
            this.ifTrue = ifTrue;
            this.ifFalse = ifFalse;
            this.isNaughty = isNaughty;
        }

        public IEnumerable<(long Item, int Monkey)> TakeTurn()
        {
            while (items.Count > 0)
            {
                InspectedItems++;
                var item = items.Dequeue();
                item = operation(item);
                if (!isNaughty) item /= 3;
                yield return (item, item % DivisibleBy == 0 ? ifTrue : ifFalse);
            }
        }

        public void GiveItem(long item)
        {
            items.Enqueue(item);
        }

        private static Monkey ParseMonkey(IReadOnlyList<string> monkeyNotes, bool isNaughty = false)
        {
            var items = GetCapture(ItemsRegex, monkeyNotes[1]).Split(", ").Select(long.Parse);
            var operationStr = GetCapture(OperationRegex, monkeyNotes[2]);
            var divisibleBy = int.Parse(GetCapture(TestRegex, monkeyNotes[3]));
            var ifTrue = int.Parse(GetCapture(ThrowRegex, monkeyNotes[4]));
            var ifFalse = int.Parse(GetCapture(ThrowRegex, monkeyNotes[5]));

            return new Monkey(items, ParseOperation(operationStr), divisibleBy, ifTrue, ifFalse, isNaughty);
        }

        private static string GetCapture(Regex regex, string str)
        {
            var match = regex.Match(str);
            return match.Groups[1].Value;
        }
        
        private static Func<long, long> ParseOperation(string operationStr)
        {
            var terms = operationStr.Split(" ");
            if (terms[1] == "+")
            {
                var value = int.Parse(terms[2]);
                return x => x + value;
            }
            else
            {
                if (terms[2] == "old")
                {
                    return x => x * x;
                }
                else
                {
                    var value = int.Parse(terms[2]);
                    return x => x * value;
                }
            }
        }

        public static Monkey Parse(string[] monkeyNotes) => ParseMonkey(monkeyNotes);
        public static Monkey ParseAngry(string[] monkeyNotes) => ParseMonkey(monkeyNotes, true);
    }
}