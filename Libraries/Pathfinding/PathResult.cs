using System.Collections.Generic;

namespace Pathfinding;

public class PathResult
{
    public List<Node> Nodes { get; }
    public int TotalCost { get; }

    public PathResult(List<Node> nodes, int totalCost)
    {
        Nodes = nodes;
        TotalCost = totalCost;
    }
}
