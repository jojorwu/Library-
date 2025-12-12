namespace Pathfinding;

public class Node
{
    public int X { get; }
    public int Y { get; }
    public bool IsWalkable { get; }
    public int GCost { get; set; } = int.MaxValue;
    public int HCost { get; set; }
    public int FCost => GCost + HCost;
    public Node? Parent { get; set; }

    public Node(int x, int y, bool isWalkable)
    {
        X = x;
        Y = y;
        IsWalkable = isWalkable;
    }
}
