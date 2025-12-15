namespace Pathfinding;

public class Node
{
    public int X { get; }
    public int Y { get; }
    public int MovementCost { get; }
    public bool IsWalkable => MovementCost < int.MaxValue;
    public int GCost { get; set; } = int.MaxValue;
    public int HCost { get; set; }
    public int FCost => GCost + HCost;
    public Node? Parent { get; set; }

    public Node(int x, int y, int movementCost)
    {
        X = x;
        Y = y;
        MovementCost = movementCost;
    }
}
