using System;

namespace Pathfinding;

public class PathfindingOptions
{
    public bool SmoothPath { get; set; } = false;
    public bool UseCache { get; set; } = true;
    public bool FindClosestIfBlocked { get; set; } = false;

    public static PathfindingOptions Default => new();

    public override int GetHashCode()
    {
        return HashCode.Combine(SmoothPath, UseCache, FindClosestIfBlocked);
    }
}
