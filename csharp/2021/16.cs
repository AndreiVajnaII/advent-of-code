public class Solver202116 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var binary = HexToBinary(lines[0]);
        var packet = ParsePacket(new TransmissionStream(binary));
        return (Traverse(packet).Sum(p => p.Version), packet.Eval());
    }

    private static Packet ParsePacket(TransmissionStream stream)
    {
        uint version = stream.ReadNumber(3);
        uint type = stream.ReadNumber(3);
        if (type == 4)
        {
            string v = String.Join("", ParseLiteral(stream));
            ulong literal = Helpers.ParseBinaryLong(v);
            return new LiteralPacket(version, type, literal);
        }
        else
        {
            char lengthType = stream.Read(1).Single();
            if (lengthType == '0')
            {
                return new OperatorPacket(version, type,
                    ParsePackets(stream.SubStream((int)stream.ReadNumber(15))).ToArray());
            }
            else
            {
                return new OperatorPacket(version, type,
                    ((int)stream.ReadNumber(11)).Times(() => ParsePacket(stream)).ToArray());
            }
        }
    }

    private static IEnumerable<Packet> ParsePackets(TransmissionStream stream)
    {
        while (!stream.IsComplete)
        {
            yield return ParsePacket(stream);
        }
    }

    private static IEnumerable<string> ParseLiteral(TransmissionStream stream)
    {
        var group = stream.ReadString(5);
        yield return group.Substring(1, 4);
        if (group[0] == '1')
        {
            group = stream.ReadString(5);
            yield return group.Substring(1, 4);
        }
    }

    private static IEnumerable<Packet> Traverse(Packet packet)
    {
        return packet.SubPackets.SelectMany(Traverse).Prepend(packet);
    }

    private static string HexToBinary(string hex)
    {
        return String.Join("", hex.Select(HexToBinary));
    }

    private static string HexToBinary(char h) => h switch
    {
        '0' => "0000",
        '1' => "0001",
        '2' => "0010",
        '3' => "0011",
        '4' => "0100",
        '5' => "0101",
        '6' => "0110",
        '7' => "0111",
        '8' => "1000",
        '9' => "1001",
        'A' => "1010",
        'B' => "1011",
        'C' => "1100",
        'D' => "1101",
        'E' => "1110",
        'F' => "1111",
        _ => throw new InvalidOperationException("invalid char " + h)
    };
}

class TransmissionStream
{
    private CharEnumerator enumerator;
    public bool IsComplete { get; private set; }

    public TransmissionStream(string binary)
    {
        this.enumerator = binary.GetEnumerator();
        IsComplete = !enumerator.MoveNext();
    }

    public uint ReadNumber(int n)
    {
        return Helpers.ParseBinary(ReadString(n));
    }

    public string ReadString(int n)
    {
        return String.Join("", Read(n));
    }

    public IEnumerable<char> Read(int n)
    {
        for (int i = 0; i < n; i++)
        {
            if (IsComplete)
            {
                throw new InvalidOperationException("Stream already ended");
            }
            yield return enumerator.Current;
            IsComplete = !enumerator.MoveNext();
        }
    }

    public TransmissionStream SubStream(int n)
    {
        return new TransmissionStream(ReadString(n));
    }
}

abstract class Packet
{
    public uint Version { get; private set; }
    public uint Type { get; private set; }
    public Packet[] SubPackets { get; private set; }

    public Packet(uint version, uint type, Packet[] subPackets)
    {
        Version = version;
        Type = type;
        SubPackets = subPackets;
    }

    public abstract ulong Eval();
}

class LiteralPacket : Packet
{
    public ulong Literal { get; private set; }

    public LiteralPacket(uint version, uint type, ulong literal)
        : base(version, type, new Packet[0])
    {
        Literal = literal;
    }

    public override ulong Eval() => Literal;
}

class OperatorPacket : Packet
{
    public OperatorPacket(uint version, uint type, Packet[] subPackets)
        : base(version, type, subPackets) { }

    public override ulong Eval() => Type switch
    {
        0 => SubPackets.Select(p => p.Eval()).Aggregate((a, b) => a + b),
        1 => SubPackets.Select(p => p.Eval()).Aggregate((a, b) => a * b),
        2 => SubPackets.Select(p => p.Eval()).Min(),
        3 => SubPackets.Select(p => p.Eval()).Max(),
        5 => SubPackets[0].Eval() > SubPackets[1].Eval() ? 1UL : 0UL,
        6 => SubPackets[0].Eval() < SubPackets[1].Eval() ? 1UL : 0UL,
        7 => SubPackets[0].Eval() == SubPackets[1].Eval() ? 1UL : 0UL,
        _ => throw new InvalidOperationException("Unknown Type " + Type)
    };
}