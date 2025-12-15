using System;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding;

internal class PathfindingGrid
{
    private readonly Node[,] _nodes;
    public int Width { get; }
    public int Height { get; }

    public PathfindingGrid(int[,] grid)
    {
        Height = grid.GetLength(0);
        Width = grid.GetLength(1);
        _nodes = new Node[Height, Width];

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                _nodes[y, x] = new Node(x, y, grid[y, x]);
            }
        }
    }

    public Node? GetNode(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return null;
        }
        return _nodes[y, x];
    }

    internal IEnumerable<Node> GetNeighbors(Node node)
    {
        var neighbors = new List<Node>();
        for (int yOffset = -1; yOffset <= 1; yOffset++)
        {
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                if (xOffset == 0 && yOffset == 0) continue;

                int checkY = node.Y + yOffset;
                int checkX = node.X + xOffset;

                if (checkY >= 0 && checkY < Height && checkX >= 0 && checkX < Width)
                {
                    if (Math.Abs(xOffset) == 1 && Math.Abs(yOffset) == 1)
                    {
                        if (!_nodes[node.Y, checkX].IsWalkable || !_nodes[checkY, node.X].IsWalkable) continue;
                    }
                    neighbors.Add(_nodes[checkY, checkX]);
                }
            }
        }
        return neighbors;
    }

    internal Node? FindNearestWalkableNode(Node startNode, Node targetNode)
    {
        int maxRadius = Math.Max(Width, Height);
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

                    if (checkX >= 0 && checkX < Width && checkY >= 0 && checkY < Height)
                    {
                        Node candidateNode = _nodes[checkY, checkX];
                        if (candidateNode.IsWalkable && candidateNode != startNode)
                        {
                            candidates.Add(candidateNode);
                        }
                    }
                }
            }

            if (candidates.Any())
            {
                return candidates.OrderBy(n => PathfindingUtils.GetDistance(startNode, n)).First();
            }
        }
        return null;
    }
}
