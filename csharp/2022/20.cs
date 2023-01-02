using Aoc;

namespace Aoc2022;

public class Solver202220 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var input = lines.Select(long.Parse).ToArray();
        const long decryptKey = 811589153L;

        return (
            Decrypt(CreateList(input), input.Length, 1),
            Decrypt(CreateList(input.Select(v => v * decryptKey).ToArray()), input.Length, 10));
    }

    private static ListNode CreateList(long[] input)
    {
        var list = new ListNode(input[0]);
        var prev = list;
        for (var i = 1; i < input.Length; i++)
        {
            var node = new ListNode(input[i]);
            prev.OriginalNext = node;
            prev.Next = node;
            node.Prev = prev;
            prev = node;
        }

        prev.Next = list;
        list.Prev = prev;
        return list;
    }

    private static long Decrypt(ListNode list, int length, int times)
    {
        ListNode? zero = null;

        while (times > 0)
        {
            for (var current = list; current != null; current = current.OriginalNext)
            {
                if (current.Value == 0)
                {
                    zero = current;
                    continue;
                }

                current.Prev!.Next = current.Next;
                current.Next!.Prev = current.Prev;
                var prev = current.Prev;
                if (current.Value > 0)
                {
                    for (var i = 0; i < current.Value % (length - 1); i++)
                    {
                        prev = prev!.Next;
                    }
                }
                else
                {
                    for (var i = 0; i < -current.Value % (length - 1); i++)
                    {
                        prev = prev!.Prev;
                    }
                }

                current.Prev = prev;
                current.Next = prev!.Next;
                prev.Next = current;
                current.Next!.Prev = current;
            }

            times--;
        }

        var p = zero!;
        var sum = 0L;
        for (var i = 0; i < 1000; i++) p = p!.Next;
        sum += p!.Value;
        for (var i = 0; i < 1000; i++) p = p!.Next;
        sum += p!.Value;
        for (var i = 0; i < 1000; i++) p = p!.Next;
        sum += p!.Value;
        return sum;
    }

    private class ListNode
    {
        public long Value { get; }
        public ListNode? OriginalNext { get; set; }
        public ListNode? Next { get; set; }
        public ListNode? Prev { get; set; }

        public ListNode(long value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"{Prev!.Value} -> {Value} -> {Next!.Value}";
        }
    }
}