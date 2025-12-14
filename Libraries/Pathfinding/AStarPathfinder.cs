using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pathfinding;

public class AStarPathfinder
{
    public async Task<PathResult> FindPath(int[,] grid, int startX, int startY, int endX, int endY, bool smoothPath = false, bool useCache = true, bool findClosestIfBlocked = false)
    {
        long gridHash = PathfindingUtils.GetGridHash(grid);
        var cacheKey = (gridHash, startX, startY, endX, endY, smoothPath, findClosestIfBlocked);
        if (useCache && PathCache.TryGetValue(cacheKey, out var cachedResult))
        {
            return cachedResult;
        }

        var pathResult = await CalculatePathAsync(grid, startX, startY, endX, endY, smoothPath, findClosestIfBlocked);

        if (useCache)
        {
            PathCache.Set(cacheKey, pathResult);
        }

        return pathResult;
    }

    private Task<PathResult> CalculatePathAsync(int[,] grid, int startX, int startY, int endX, int endY, bool smoothPath, bool findClosestIfBlocked)
    {
        return Task.Run(() =>
        {
            int height = grid.GetLength(0);
            int width = grid.GetLength(1);

            if (startX < 0 || startX >= width || startY < 0 || startY >= height ||
                endX < 0 || endX >= width || endY < 0 || endY >= height)
            {
                return new PathResult(new List<Node>(), 0);
            }

            var nodes = new Node[height, width];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    nodes[y, x] = new Node(x, y, grid[y, x]);
                }
            }

            var startNode = nodes[startY, startX];
            var endNode = nodes[endY, endX];

            if (!startNode.IsWalkable) return new PathResult(new List<Node>(), 0);

            if (!endNode.IsWalkable)
            {
                if (findClosestIfBlocked)
                {
                    var nearestNode = FindNearestWalkableNode(nodes, startNode, endNode);
                    if (nearestNode == null) return new PathResult(new List<Node>(), 0);
                    endNode = nearestNode;
                }
                else
                {
                    return new PathResult(new List<Node>(), 0);
                }
            }

            if (startNode == endNode) return new PathResult(new List<Node> { startNode }, 0);

            startNode.GCost = 0;
            startNode.HCost = PathfindingUtils.GetDistance(startNode, endNode);

            var openSet = new PriorityQueue<Node, int>();
            openSet.Enqueue(startNode, startNode.FCost);
            var closedSet = new HashSet<Node>();

            PathResult pathResult = new PathResult(new List<Node>(), 0);

            while (openSet.Count > 0)
            {
                var currentNode = openSet.Dequeue();
                closedSet.Add(currentNode);

                if (currentNode == endNode)
                {
                    var path = RetracePath(startNode, endNode);
                    pathResult = new PathResult(path, endNode.GCost);
                    break;
                }

                foreach (var neighbor in GetNeighbors(nodes, currentNode))
                {
                    if (!neighbor.IsWalkable || closedSet.Contains(neighbor)) continue;

                    var newCostToNeighbor = currentNode.GCost + PathfindingUtils.GetDistance(currentNode, neighbor) + neighbor.MovementCost;
                    if (newCostToNeighbor < neighbor.GCost)
                    {
                        neighbor.GCost = newCostToNeighbor;
                        neighbor.HCost = PathfindingUtils.GetDistance(neighbor, endNode);
                        neighbor.Parent = currentNode;
                        openSet.Enqueue(neighbor, neighbor.FCost);
                    }
                }
            }

            if (smoothPath)
            {
                pathResult = PathSmoother.SmoothPath(grid, pathResult);
            }

            return pathResult;
        });
    }

    private Node FindNearestWalkableNode(Node[,] nodes, Node startNode, Node targetNode)
    {
        int maxRadius = Math.Max(nodes.GetLength(0), nodes.GetLength(1));
        for (int radius = 1; radius < maxRadius; radius++)
        {
            var candidates = new List<Node>();
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    if (Math.Abs(y) != radius && Math.Abs(x) != radius) continue;

                    int checkX = targetNode.X + x;
                    int checkY = targetNode.Y + y;

                    if (checkX >= 0 && checkX < nodes.GetLength(1) && checkY >= 0 && checkY < nodes.GetLength(0))
                    {
                        Node candidateNode = nodes[checkY, checkX];
                        if (candidateNode.IsWalkable && candidateNode != startNode)
                        {
                            candidates.Add(candidateNode);
                        }
                    }
                }
            }

            if (candidates.Any())
            {
                Node bestNode = null;
                int bestDistance = int.MaxValue;
                foreach (var candidate in candidates)
                {
                    int distance = PathfindingUtils.GetDistance(startNode, candidate);
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestNode = candidate;
                    }
                }
                if (bestNode != null)
                {
                    return bestNode;
                }
            }
        }
        return null;
    }

    private static List<Node> RetracePath(Node startNode, Node endNode)
    {
        var path = new List<Node>();
        var currentNode = endNode;
        while (currentNode != null)
        {
            path.Add(currentNode);
            if(currentNode == startNode)
            {
                path.Reverse();
                return path;
            }
            currentNode = currentNode.Parent;
        }

        return new List<Node>();
    }

    private static IEnumerable<Node> GetNeighbors(Node[,] nodes, Node node)
    {
        var neighbors = new List<Node>();
        int height = nodes.GetLength(0);
        int width = nodes.GetLength(1);

        for (int yOffset = -1; yOffset <= 1; yOffset++)
        {
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                if (xOffset == 0 && yOffset == 0) continue;

                int checkY = node.Y + yOffset;
                int checkX = node.X + xOffset;

                if (checkY >= 0 && checkY < height && checkX >= 0 && checkX < width)
                {
                    if (Math.Abs(xOffset) == 1 && Math.Abs(yOffset) == 1)
                    {
                        if (!nodes[node.Y, checkX].IsWalkable || !nodes[checkY, node.X].IsWalkable) continue;
                    }
                    neighbors.Add(nodes[checkY, checkX]);
                }
            }
        }
        return neighbors;
    }
}
