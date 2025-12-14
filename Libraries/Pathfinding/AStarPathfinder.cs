using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinding;

public class AStarPathfinder
{
    public async Task<PathResult> FindPath(int[,] grid, int startX, int startY, int endX, int endY, PathfindingOptions? options = null)
    {
        options ??= PathfindingOptions.Default;

        long gridHash = PathfindingUtils.GetGridHash(grid);
        var cacheKey = (gridHash, startX, startY, endX, endY, options.GetHashCode());
        if (options.UseCache && PathCache.TryGetValue(cacheKey, out var cachedResult))
        {
            return cachedResult;
        }

        var pathResult = await CalculatePathAsync(grid, startX, startY, endX, endY, options);

        if (options.UseCache)
        {
            PathCache.Set(cacheKey, pathResult);
        }

        return pathResult;
    }

    private Task<PathResult> CalculatePathAsync(int[,] grid, int startX, int startY, int endX, int endY, PathfindingOptions options)
    {
        return Task.Run(() =>
        {
            var pathfindingGrid = new PathfindingGrid(grid);

            if (startX < 0 || startX >= pathfindingGrid.Width || startY < 0 || startY >= pathfindingGrid.Height ||
                endX < 0 || endX >= pathfindingGrid.Width || endY < 0 || endY >= pathfindingGrid.Height)
            {
                return new PathResult(new List<Node>(), 0);
            }

            var startNode = pathfindingGrid.GetNode(startX, startY);
            var endNode = pathfindingGrid.GetNode(endX, endY);

            if (startNode == null || endNode == null) return new PathResult(new List<Node>(), 0);

            if (!startNode.IsWalkable) return new PathResult(new List<Node>(), 0);

            if (!endNode.IsWalkable)
            {
                if (options.FindClosestIfBlocked)
                {
                    var nearestNode = pathfindingGrid.FindNearestWalkableNode(startNode, endNode);
                    if (nearestNode == null) return new PathResult(new List<Node>(), 0);
                    endNode = nearestNode;
                }
                else
                {
                    return new PathResult(new List<Node>(), 0);
                }
            }

            if (startNode == endNode) return new PathResult(new List<Node> { startNode }, 0);

            var pathResult = AStarAlgorithm.Run(pathfindingGrid, startNode, endNode);

            if (options.SmoothPath)
            {
                pathResult = PathSmoother.SmoothPath(grid, pathResult);
            }

            return pathResult;
        });
    }
}
