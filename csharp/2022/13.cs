using System.Text;
using Aoc;

namespace Aoc2022;

public class Solver202213 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var packets = lines.Where(line => !string.IsNullOrWhiteSpace(line)).Select(Packet.Parse).ToList();
        var orderedPairsSum = packets.Chunk(2).Select(EnumerableExtensions.AsTuple2)
            .Select((pair, i) => pair.Item1 < pair.Item2 ? i + 1 : 0).Sum();

        var divider1 = Packet.Parse("[[2]]");
        var divider2 = Packet.Parse("[[6]]");
        packets.Add(divider1);
        packets.Add(divider2);
        packets.Sort();

        return (
            orderedPairsSum,
            (packets.IndexOf(divider1) + 1) * (packets.IndexOf(divider2) + 1)
        );
    }

    private abstract class Packet : IComparable<Packet>
    {
        public static Packet Parse(string line)
        {
            var stream = line.GetEnumerator();
            stream.MoveNext();
            return Parse(stream);
        }

        protected static Packet Parse(CharEnumerator stream)
        {
            return stream.Current == '['
                ? PacketList.Parse(stream)
                : PacketInt.Parse(stream);
        }

        public static bool operator <(Packet? left, Packet? right)
        {
            if (left is null || right is null) throw new NullReferenceException("cannot compare to null packet");
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(Packet? left, Packet? right)
        {
            if (left is null || right is null) throw new NullReferenceException("cannot compare to null packet");
            return left.CompareTo(right) > 0;
        }

        public int CompareTo(Packet? other)
        {
            if (other is null) throw new NullReferenceException("cannot compare to null packet");
            return CompareToPacket(other);
        }

        protected abstract int CompareToPacket(Packet other);
    }

    private class PacketList : Packet
    {
        private readonly IReadOnlyList<Packet> subPackets;

        private PacketList(IReadOnlyList<Packet> subPackets)
        {
            this.subPackets = subPackets;
        }

        protected override int CompareToPacket(Packet other)
            => CompareToPacketList(other as PacketList ?? new PacketList(new List<Packet> {other}));

        private int CompareToPacketList(PacketList other)
        {
            foreach (var (left, right) in subPackets.Zip(other.subPackets))
            {
                var comparison = left.CompareTo(right);
                if (comparison != 0)
                {
                    return comparison;
                }
            }

            return subPackets.Count.CompareTo(other.subPackets.Count);
        }

        public new static PacketList Parse(CharEnumerator stream)
        {
            var subPackets = new List<Packet>();
            stream.MoveNext();
            while (stream.Current != ']')
            {
                subPackets.Add(Packet.Parse(stream));
                if (stream.Current == ',')
                {
                    stream.MoveNext();
                }
            }

            stream.MoveNext();
            return new PacketList(subPackets);
        }
    }

    private class PacketInt : Packet
    {
        private int Number { get; }

        private PacketInt(int number)
        {
            Number = number;
        }

        public new static PacketInt Parse(CharEnumerator stream)
        {
            var sNumber = new StringBuilder();
            while (char.IsDigit(stream.Current))
            {
                sNumber.Append(stream.Current);
                stream.MoveNext();
            }

            return new PacketInt(int.Parse(sNumber.ToString()));
        }

        protected override int CompareToPacket(Packet other)
        {
            return other is PacketInt otherInt
                ? Number.CompareTo(otherInt.Number)
                : -other.CompareTo(this);
        }
    }
}