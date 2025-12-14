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

    internal static long GetGridHash(int[,] grid)
    {
        const long fnvPrime = 1099511628211;
        long hash = unchecked((long)14695981039346656037);

        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                hash = (hash ^ grid[y, x]) * fnvPrime;
            }
        }
        return hash;
    }
}
