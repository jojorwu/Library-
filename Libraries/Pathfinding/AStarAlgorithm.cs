using System.Collections.Generic;

namespace Pathfinding;

internal static class AStarAlgorithm
{
    internal static PathResult Run(PathfindingGrid grid, Node startNode, Node endNode)
    {
        startNode.GCost = 0;
        startNode.HCost = PathfindingUtils.GetDistance(startNode, endNode);

        var openSet = new PriorityQueue<Node, int>();
        openSet.Enqueue(startNode, startNode.FCost);
        var closedSet = new HashSet<Node>();

        while (openSet.Count > 0)
        {
            var currentNode = openSet.Dequeue();
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                var path = RetracePath(startNode, endNode);
                return new PathResult(path, endNode.GCost);
            }

            foreach (var neighbor in grid.GetNeighbors(currentNode))
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

        return new PathResult(new List<Node>(), 0);
    }

    private static List<Node> RetracePath(Node startNode, Node endNode)
    {
        var path = new List<Node>();
        var currentNode = endNode;
        while (currentNode != null)
        {
            path.Add(currentNode);
            if (currentNode == startNode)
            {
                path.Reverse();
                return path;
            }
            currentNode = currentNode.Parent;
        }

        return new List<Node>();
    }
}
