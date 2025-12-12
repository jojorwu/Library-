using System;

namespace Pathfinding;

internal static class PathfindingUtils
{
    internal const int MOVE_STRAIGHT_COST = 10;
    internal const int MOVE_DIAGONAL_COST = 14;

    internal static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Math.Abs(nodeA.X - nodeB.X);
        int dstY = Math.Abs(nodeA.Y - nodeB.Y);

        if (dstX > dstY)
            return MOVE_DIAGONAL_COST * dstY + MOVE_STRAIGHT_COST * (dstX - dstY);
        return MOVE_DIAGONAL_COST * dstX + MOVE_STRAIGHT_COST * (dstY - dstX);
    }
}
