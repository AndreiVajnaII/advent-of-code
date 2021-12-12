public class Solver202112 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var graph = new Graph<string>();
        foreach (var line in lines)
        {
            var (start, end) = line.Split('-').AsTuple2();
            graph.AddTwoWay(start, end);
        }
        return (
            graph.Traverse("start", "end", new CaveVisitor(new Part1VisitPolicy())).Paths,
            graph.Traverse("start", "end", new CaveVisitor(new Part2VisitPolicy())).Paths);
    }

}

class CaveVisitor : GraphVisitor<string>
{
    public int Paths { get; private set; }

    public CaveVisitor(IGraphVisitPolicy<string> visitPolicy) : base(visitPolicy) { }

    public override void End(string node)
    {
        base.End(node);
        Paths++;
    }

}

class Part1VisitPolicy : DefaultGraphVisitPolicy<string>
{
    public override bool HasCompletelyVisited(string node)
    {
        return !node.All(Char.IsUpper) && base.HasCompletelyVisited(node);
    }
}

class Part2VisitPolicy : Part1VisitPolicy
{
    private string? twiceVisited = null;
    public override bool HasCompletelyVisited(string node)
    {
        return base.HasCompletelyVisited(node)
            && !(IsSmallCave(node) && twiceVisited is null);
    }

    public override void Visit(string node)
    {
        if (IsSmallCave(node) && base.HasCompletelyVisited(node))
        {
            twiceVisited = node;
        }
        base.Visit(node);
    }

    public override void Unvisit(string node)
    {
        if (twiceVisited == node)
        {
            twiceVisited = null;
        }
        else
        {
            base.Unvisit(node);
        }
    }

    private bool IsSmallCave(string node)
    {
        return node != "start" && node != "end" && node.All(Char.IsLower);
    }
}
