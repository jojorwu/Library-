using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pathfinding;

public class AStarPathfinder
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public Task<List<Node>> FindPath(int[,] grid, int startX, int startY, int endX, int endY)
    {
        return Task.Run(() =>
        {
            int height = grid.GetLength(0);
            int width = grid.GetLength(1);

            // Input validation
            if (startX < 0 || startX >= width || startY < 0 || startY >= height ||
                endX < 0 || endX >= width || endY < 0 || endY >= height)
            {
                return new List<Node>(); // Out of bounds
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

            if (!startNode.IsWalkable || !endNode.IsWalkable)
            {
                return new List<Node>(); // Start or end node is not walkable
            }

            if (startNode == endNode) return new List<Node> { startNode };

            startNode.GCost = 0;
            startNode.HCost = GetDistance(startNode, endNode);

            var openSet = new PriorityQueue<Node, int>();
            openSet.Enqueue(startNode, startNode.FCost);
            var closedSet = new HashSet<Node>();

            while (openSet.Count > 0)
            {
                var currentNode = openSet.Dequeue();
                closedSet.Add(currentNode);

                if (currentNode == endNode)
                {
                    return RetracePath(startNode, endNode);
                }

                foreach (var neighbor in GetNeighbors(nodes, currentNode))
                {
                    if (!neighbor.IsWalkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    var newCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor) + neighbor.MovementCost;
                    if (newCostToNeighbor < neighbor.GCost)
                    {
                        neighbor.GCost = newCostToNeighbor;
                        neighbor.HCost = GetDistance(neighbor, endNode);
                        neighbor.Parent = currentNode;
                        openSet.Enqueue(neighbor, neighbor.FCost);
                    }
                }
            }

            return new List<Node>();
        });
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
                if (xOffset == 0 && yOffset == 0)
                    continue;

                int checkY = node.Y + yOffset;
                int checkX = node.X + xOffset;

                if (checkY >= 0 && checkY < height && checkX >= 0 && checkX < width)
                {
                    // Prevent cutting corners
                    if (Math.Abs(xOffset) == 1 && Math.Abs(yOffset) == 1)
                    {
                        if (!nodes[node.Y, checkX].IsWalkable || !nodes[checkY, node.X].IsWalkable)
                        {
                            continue;
                        }
                    }
                    neighbors.Add(nodes[checkY, checkX]);
                }
            }
        }

        return neighbors;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Math.Abs(nodeA.X - nodeB.X);
        int dstY = Math.Abs(nodeA.Y - nodeB.Y);

        if (dstX > dstY)
            return MOVE_DIAGONAL_COST * dstY + MOVE_STRAIGHT_COST * (dstX - dstY);
        return MOVE_DIAGONAL_COST * dstX + MOVE_STRAIGHT_COST * (dstY - dstX);
    }
}
