using NUnit.Framework;
using Pathfinding;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding.Tests;

public class AStarPathfinderTests
{
    private const int W = 1; // Walkable
    private const int O = int.MaxValue; // Obstacle

    [Test]
    public void FindPath_WithSimplePath_ReturnsCorrectPath()
    {
        var grid = new int[,]
        {
            { W, W, W },
            { W, W, W },
            { W, W, W }
        };
        var pathfinder = new AStarPathfinder();
        var path = pathfinder.FindPath(grid, 0, 0, 2, 2);

        var expectedPath = new List<(int, int)> { (0, 0), (1, 1), (2, 2) };
        var actualPath = path.Select(p => (p.X, p.Y)).ToList();

        Assert.That(actualPath, Is.EqualTo(expectedPath));
    }

    [Test]
    public void FindPath_WithObstacles_ReturnsCorrectPath()
    {
        var grid = new int[,]
        {
            { W, W, W, W, W },
            { W, O, O, O, W },
            { W, W, W, W, W },
            { W, O, O, O, W },
            { O, W, W, W, W } // Block one of the optimal paths
        };
        var pathfinder = new AStarPathfinder();
        var path = pathfinder.FindPath(grid, 0, 0, 4, 4);

        var expectedPath = new List<(int, int)>
        {
            (0, 0), (0, 1), (0, 2), (1, 2), (2, 2), (3, 2), (4, 2), (4, 3), (4, 4)
        };
        var actualPath = path.Select(p => (p.X, p.Y)).ToList();

        Assert.That(actualPath, Is.EqualTo(expectedPath));
    }

    [Test]
    public void FindPath_WithNoPath_ReturnsEmptyList()
    {
        var grid = new int[,]
        {
            { W, O, W },
            { W, O, W },
            { W, O, W }
        };
        var pathfinder = new AStarPathfinder();
        var path = pathfinder.FindPath(grid, 0, 0, 2, 0);
        Assert.That(path, Is.Empty);
    }

    [Test]
    public void FindPath_WithStartAndEndSame_ReturnsPathWithOneNode()
    {
        var grid = new int[,]
        {
            { W, W, W },
            { W, W, W },
            { W, W, W }
        };
        var pathfinder = new AStarPathfinder();
        var path = pathfinder.FindPath(grid, 1, 1, 1, 1);
        Assert.That(path.Count, Is.EqualTo(1));
        Assert.That(path.Single().X, Is.EqualTo(1));
        Assert.That(path.Single().Y, Is.EqualTo(1));
    }

    [TestCase(-1, 0, 1, 1)]
    [TestCase(0, -1, 1, 1)]
    [TestCase(3, 0, 1, 1)]
    [TestCase(0, 3, 1, 1)]
    [TestCase(0, 0, -1, 1)]
    [TestCase(0, 0, 1, -1)]
    [TestCase(0, 0, 3, 1)]
    [TestCase(0, 0, 1, 3)]
    public void FindPath_WithOutOfBoundsCoordinates_ReturnsEmptyList(int startX, int startY, int endX, int endY)
    {
        var grid = new int[,]
        {
            { W, W, W },
            { W, W, W },
            { W, W, W }
        };
        var pathfinder = new AStarPathfinder();
        var path = pathfinder.FindPath(grid, startX, startY, endX, endY);
        Assert.That(path, Is.Empty);
    }

    [Test]
    public void FindPath_WithUnwalkableStart_ReturnsEmptyList()
    {
        var grid = new int[,]
        {
            { O, W, W },
            { W, W, W },
            { W, W, W }
        };
        var pathfinder = new AStarPathfinder();
        var path = pathfinder.FindPath(grid, 0, 0, 2, 2);
        Assert.That(path, Is.Empty);
    }

    [Test]
    public void FindPath_WithUnwalkableEnd_ReturnsEmptyList()
    {
        var grid = new int[,]
        {
            { W, W, W },
            { W, W, W },
            { W, W, O }
        };
        var pathfinder = new AStarPathfinder();
        var path = pathfinder.FindPath(grid, 0, 0, 2, 2);
        Assert.That(path, Is.Empty);
    }

    [Test]
    public void FindPath_PrefersCheaperPathOverShorterPath()
    {
        // The direct path (0,0 -> 1,0 -> 2,0) is shorter but goes through a high-cost tile (100).
        // The longer path around the high-cost tile is cheaper.
        var grid = new int[,]
        {
            { W, 100, W },
            { W, W,   W },
        };
        var pathfinder = new AStarPathfinder();
        var path = pathfinder.FindPath(grid, 0, 0, 2, 0);

        var expectedPath = new List<(int, int)> { (0, 0), (1, 1), (2, 0) };
        var actualPath = path.Select(p => (p.X, p.Y)).ToList();

        Assert.That(actualPath, Is.EqualTo(expectedPath));
    }
}
