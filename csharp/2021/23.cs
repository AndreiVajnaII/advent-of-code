using System.Collections.Immutable;

public class Solver202123 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var extendendLines = lines.ToList();
        extendendLines.Insert(3, "  #D#B#A#C#  ");
        extendendLines.Insert(3, "  #D#C#B#A#  ");
        var organizer = new AmphipodOrganizer();
        return (organizer.Organize(Parse(lines, 2)),
            organizer.Organize(Parse(extendendLines.ToArray(), 4)));
    }

    private static char[,] Parse(string[] lines, int roomSize)
    {
        char[,] rooms = new char[4, roomSize];
        for (int level = 0; level < rooms.GetLength(1); level++)
        {
            for (int room = 0; room < rooms.GetLength(0); room++)
            {
                rooms[room, level] = lines[roomSize + 1 - level][3 + room * 2];
            }
        }
        return rooms;
    }
}

class AmphipodOrganizer
{
    public int Organize(char[,] rooms)
    {
        var statesQueue = new PriorityQueue<AmphipodState, int>();
        var states = new HashSet<AmphipodState>();
        var roomStacks = new ImmutableStack<char>[rooms.GetLength(0)];
        for (int i = 0; i < rooms.GetLength(0); i++)
        {
            roomStacks[i] = ImmutableStack<char>.Empty;
            for (int level = 0; level < rooms.GetLength(1); level++)
            {
                roomStacks[i] = roomStacks[i].Push(rooms[i, level]);
            }
        }
        statesQueue.Enqueue(new AmphipodState(new char[11].ToImmutableArray(), roomStacks.ToImmutableArray(),
            0, 0, rooms.GetLength(1)), 0);
        while (statesQueue.Count > 0)
        {
            var state = statesQueue.Dequeue();
            for (int from = 0; from < state.Hallway.Length; from++)
            {
                if (state.Hallway[from] != '\0')
                {
                    if (state.CanMoveToRoom(from))
                    {
                        var newState = state.MoveToRoom(from);
                        if (newState.IsFinal())
                        {
                            return newState.Energy;
                        }
                        statesQueue.Enqueue(newState, newState.Cost);
                    }
                }
            }
            foreach (var newState in state.ComputeMoveToHallwayStates())
            {
                if (!states.Contains(newState))
                {
                    states.Add(newState);
                    statesQueue.Enqueue(newState, newState.Cost);
                }
            }
        }
        return 0;
    }
}

class AmphipodState
{
    public ImmutableArray<char> Hallway { get; private set; }
    public ImmutableArray<ImmutableStack<char>> Rooms { get; private set; }
    public int Cost { get; private set; }
    public int Energy { get; private set; }
    public int RoomSize { get; private set; }

    private string HallwayString { get => String.Join("", Hallway); }
    private string RoomsString
    {
        get => String.Join("", Rooms.Select(room => String.Join("", room).PadRight(RoomSize)));
    }

    public AmphipodState(ImmutableArray<char> hallway, ImmutableArray<ImmutableStack<char>> rooms,
        int cost, int energy, int roomSize)
    {
        Hallway = hallway;
        Rooms = rooms;
        Cost = cost;
        Energy = energy;
        RoomSize = roomSize;
    }

    public AmphipodState MoveToRoom(int from)
    {
        char amphipod = Hallway[from];
        var to = AmphipodTargetRoom(amphipod);
        var newRooms = Rooms.SetItem(to, Rooms[to].Push(amphipod));
        var newHallway = Hallway.SetItem(from, '\0');
        var newEnergy = (Math.Abs(RoomHallwayIndex(to) - from) + (RoomSize - newRooms[to].Count() + 1))
            * AmphipodEnergy(amphipod);
        return new AmphipodState(newHallway, newRooms, Cost, Energy + newEnergy, RoomSize);
    }

    public AmphipodState MoveToHallway(int from, int to)
    {
        char amphipod;
        var newRooms = Rooms.SetItem(from, Rooms[from].Pop(out amphipod));
        var newHallway = Hallway.SetItem(to, amphipod);
        var newEnergy = (Math.Abs(to - RoomHallwayIndex(from)) + (RoomSize - newRooms[from].Count()))
            * AmphipodEnergy(amphipod);
        var fromIndex = RoomHallwayIndex(from);
        var targetIndex = RoomHallwayIndex(AmphipodTargetRoom(amphipod));
        var cost = (Math.Abs(to - fromIndex) + Math.Abs(to - targetIndex)
            - Math.Abs(targetIndex - fromIndex))
            * AmphipodEnergy(amphipod);
        if (fromIndex == targetIndex)
        {
            cost += 2 * AmphipodEnergy(amphipod);
        }
        return new AmphipodState(newHallway, newRooms, Cost + cost, Energy + newEnergy, RoomSize);
    }

    public bool IsFinal()
    {
        for (int i = 0; i < Rooms.Length; i++)
        {
            if (Rooms[i].Count() != RoomSize || Rooms[i].Any(c => AmphipodTargetRoom(c) != i))
            {
                return false;
            }
        }
        return true;
    }

    private int RoomHallwayIndex(int roomIndex)
    {
        return 2 + 2 * roomIndex;
    }

    private int AmphipodTargetRoom(char amphipod) => amphipod - 'A';

    private int AmphipodEnergy(char amphipod) => amphipod switch
    {
        'A' => 1,
        'B' => 10,
        'C' => 100,
        'D' => 1000,
        _ => throw new InvalidOperationException("Unknown amphipod " + amphipod)
    };

    public IEnumerable<int> GetDestinations(int roomIndex)
    {
        int start = RoomHallwayIndex(roomIndex);
        for (int i = start - 1; i >= 0 && Hallway[i] == '\0'; i--)
        {
            if (i % 2 != 0 || i == 0)
            {
                yield return i;
            }
        }
        for (int i = start + 1; i < Hallway.Length && Hallway[i] == '\0'; i++)
        {
            if (i % 2 != 0 || i == 10)
            {
                yield return i;
            }
        }
    }

    public bool CanMoveToRoom(int from)
    {
        var amphipod = Hallway[from];
        var to = AmphipodTargetRoom(amphipod);
        return Rooms[to].All(c => c == amphipod) && !IsRoadBlocked(from, RoomHallwayIndex(to));
    }

    private bool IsRoadBlocked(int from, int to)
    {
        (from, to) = from > to ? (to, from - 1) : (from + 1, to);
        for (int i = from; i <= to; i++)
        {
            if (Hallway[i] != '\0')
            {
                return true;
            }
        }
        return false;
    }

    public override bool Equals(object? obj)
    {
        return obj is AmphipodState state &&
               HallwayString == state.HallwayString &&
               RoomsString == state.RoomsString &&
               Cost == state.Cost &&
               Energy == state.Energy &&
               RoomSize == state.RoomSize;
    }

    public override int GetHashCode()
    {

        return HashCode.Combine(HallwayString, RoomsString, Cost, Energy, RoomSize);
    }

    public IEnumerable<AmphipodState> ComputeMoveToHallwayStates()
    {
        return Rooms.Select((room, roomIndex) => (room, roomIndex))
            .Where(item => item.room.Count() > 0 && !item.room.All(c => AmphipodTargetRoom(c) == item.roomIndex))
            .Select(item => (amphipod: item.room.Peek(), item.roomIndex,
                destinations: GetDestinations(item.roomIndex)))
            .SelectMany(item => item.destinations.Select(to => (item.amphipod, item.roomIndex, to)))
            .Select(item => MoveToHallway(item.roomIndex, item.to));
    }
}
