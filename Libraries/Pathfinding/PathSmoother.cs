using System;
using System.Collections.Generic;

namespace Pathfinding;

public static class PathSmoother
{
    public static PathResult SmoothPath(int[,] grid, PathResult originalPath)
    {
        if (originalPath.Nodes == null || originalPath.Nodes.Count < 2)
        {
            return originalPath;
        }

        var newPathNodes = new List<Node> { originalPath.Nodes[0] };
        int currentIndex = 0;

        while (currentIndex < originalPath.Nodes.Count - 1)
        {
            int lastVisibleIndex = currentIndex + 1;
            for (int i = currentIndex + 2; i < originalPath.Nodes.Count; i++)
            {
                if (HasLineOfSight(grid, originalPath.Nodes[currentIndex], originalPath.Nodes[i]))
                {
                    lastVisibleIndex = i;
                }
                else
                {
                    break;
                }
            }
            newPathNodes.Add(originalPath.Nodes[lastVisibleIndex]);
            currentIndex = lastVisibleIndex;
        }

        int newCost = GetPathCost(newPathNodes);
        return new PathResult(newPathNodes, newCost);
    }

    private static int GetPathCost(List<Node> path)
    {
        int totalCost = 0;
        for (int i = 0; i < path.Count - 1; i++)
        {
            totalCost += PathfindingUtils.GetDistance(path[i], path[i + 1]);
            totalCost += path[i + 1].MovementCost;
        }
        return totalCost;
    }

    private static bool HasLineOfSight(int[,] grid, Node start, Node end)
    {
        int x0 = start.X;
        int y0 = start.Y;
        int x1 = end.X;
        int y1 = end.Y;

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            if (grid[y0, x0] >= int.MaxValue) return false;

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            int x_old = x0;
            int y_old = y0;

            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }

            if (x0 != x_old && y0 != y_old)
            {
                if (grid[y_old, x0] >= int.MaxValue || grid[y0, x_old] >= int.MaxValue)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
