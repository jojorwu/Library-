using System;
using System.Collections.Generic;

namespace Pathfinding;

public static class PathSmoother
{
    public static List<Node> SmoothPath(int[,] grid, List<Node> path)
    {
        if (path == null)
        {
            return new List<Node>();
        }
        if (path.Count < 2)
        {
            return path;
        }

        var newPath = new List<Node> { path[0] };
        int currentIndex = 0;

        while (currentIndex < path.Count - 1)
        {
            int lastVisibleIndex = currentIndex + 1;
            for (int i = currentIndex + 2; i < path.Count; i++)
            {
                if (HasLineOfSight(grid, path[currentIndex], path[i]))
                {
                    lastVisibleIndex = i;
                }
                else
                {
                    break; // Stop if line of sight is broken
                }
            }
            newPath.Add(path[lastVisibleIndex]);
            currentIndex = lastVisibleIndex;
        }

        return newPath;
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
            // Check current node for obstacle
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

            // Check for obstacles when moving diagonally
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
