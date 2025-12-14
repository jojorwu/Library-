using NUnit.Framework;
using Pathfinding;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pathfinding.Tests;

public class AStarPathfinderTests
{
    private const int W = 1; // Walkable
    private const int O = int.MaxValue; // Obstacle

    [TearDown]
    public void TearDown()
    {
        PathCache.Clear();
    }

    [Test]
    public async Task FindPath_WithSimplePath_ReturnsCorrectPathAndCost()
    {
        var grid = new int[,]
        {
            { W, W, W },
            { W, W, W },
            { W, W, W }
        };
        var pathfinder = new AStarPathfinder();
        var result = await pathfinder.FindPath(grid, 0, 0, 2, 2);

        var expectedPath = new List<(int, int)> { (0, 0), (1, 1), (2, 2) };
        var actualPath = result.Nodes.Select(p => (p.X, p.Y)).ToList();

        Assert.That(actualPath, Is.EqualTo(expectedPath));
        Assert.That(result.TotalCost, Is.EqualTo(30));
    }

    [Test]
    public async Task FindPath_WithObstacles_ReturnsCorrectPathAndCost()
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
        var result = await pathfinder.FindPath(grid, 0, 0, 4, 4);

        var expectedPath = new List<(int, int)>
        {
            (0, 0), (0, 1), (0, 2), (1, 2), (2, 2), (3, 2), (4, 2), (4, 3), (4, 4)
        };
        var actualPath = result.Nodes.Select(p => (p.X, p.Y)).ToList();

        Assert.That(actualPath, Is.EqualTo(expectedPath));
        Assert.That(result.TotalCost, Is.EqualTo(88));
    }

    [Test]
    public async Task FindPath_WithNoPath_ReturnsEmptyResult()
    {
        var grid = new int[,]
        {
            { W, O, W },
            { W, O, W },
            { W, O, W }
        };
        var pathfinder = new AStarPathfinder();
        var result = await pathfinder.FindPath(grid, 0, 0, 2, 0);
        Assert.That(result.Nodes, Is.Empty);
        Assert.That(result.TotalCost, Is.EqualTo(0));
    }

    [Test]
    public async Task FindPath_WithStartAndEndSame_ReturnsPathWithOneNodeAndZeroCost()
    {
        var grid = new int[,]
        {
            { W, W, W },
            { W, W, W },
            { W, W, W }
        };
        var pathfinder = new AStarPathfinder();
        var result = await pathfinder.FindPath(grid, 1, 1, 1, 1);
        Assert.That(result.Nodes.Count, Is.EqualTo(1));
        Assert.That(result.Nodes.Single().X, Is.EqualTo(1));
        Assert.That(result.Nodes.Single().Y, Is.EqualTo(1));
        Assert.That(result.TotalCost, Is.EqualTo(0));
    }

    [TestCase(-1, 0, 1, 1)]
    [TestCase(0, -1, 1, 1)]
    [TestCase(3, 0, 1, 1)]
    [TestCase(0, 3, 1, 1)]
    [TestCase(0, 0, -1, 1)]
    [TestCase(0, 0, 1, -1)]
    [TestCase(0, 0, 3, 1)]
    [TestCase(0, 0, 1, 3)]
    public async Task FindPath_WithOutOfBoundsCoordinates_ReturnsEmptyResult(int startX, int startY, int endX, int endY)
    {
        var grid = new int[,]
        {
            { W, W, W },
            { W, W, W },
            { W, W, W }
        };
        var pathfinder = new AStarPathfinder();
        var result = await pathfinder.FindPath(grid, startX, startY, endX, endY);
        Assert.That(result.Nodes, Is.Empty);
        Assert.That(result.TotalCost, Is.EqualTo(0));
    }

    [Test]
    public async Task FindPath_WithUnwalkableStart_ReturnsEmptyResult()
    {
        var grid = new int[,]
        {
            { O, W, W },
            { W, W, W },
            { W, W, W }
        };
        var pathfinder = new AStarPathfinder();
        var result = await pathfinder.FindPath(grid, 0, 0, 2, 2);
        Assert.That(result.Nodes, Is.Empty);
        Assert.That(result.TotalCost, Is.EqualTo(0));
    }

    [Test]
    public async Task FindPath_WithUnwalkableEnd_ReturnsEmptyResult()
    {
        var grid = new int[,]
        {
            { W, W, W },
            { W, W, W },
            { W, W, O }
        };
        var pathfinder = new AStarPathfinder();
        var result = await pathfinder.FindPath(grid, 0, 0, 2, 2);
        Assert.That(result.Nodes, Is.Empty);
        Assert.That(result.TotalCost, Is.EqualTo(0));
    }

    [Test]
    public async Task FindPath_PrefersCheaperPathOverShorterPath_ReturnsCorrectPathAndCost()
    {
        var grid = new int[,]
        {
            { W, 100, W },
            { W, W,   W },
        };
        var pathfinder = new AStarPathfinder();
        var result = await pathfinder.FindPath(grid, 0, 0, 2, 0);

        var expectedPath = new List<(int, int)> { (0, 0), (1, 1), (2, 0) };
        var actualPath = result.Nodes.Select(p => (p.X, p.Y)).ToList();

        Assert.That(actualPath, Is.EqualTo(expectedPath));
        Assert.That(result.TotalCost, Is.EqualTo(30));
    }

    [Test]
    public async Task FindPath_FindClosestIfBlocked_FindsClosestWalkableNode()
    {
        var grid = new int[,]
        {
            { W, W, W },
            { W, O, W },
            { W, W, W }
        };
        var pathfinder = new AStarPathfinder();
        var result = await pathfinder.FindPath(grid, 0, 0, 1, 1, new PathfindingOptions { FindClosestIfBlocked = true });

        var expectedPath = new List<(int, int)> { (0, 0), (1, 0) };
        var actualPath = result.Nodes.Select(p => (p.X, p.Y)).ToList();

        Assert.That(actualPath, Is.EqualTo(expectedPath));
    }

    [Test]
    public async Task FindPath_FindClosestIfBlocked_FailsIfNoWalkableNodeFound()
    {
        var grid = new int[,]
        {
            { W, O, W },
            { O, O, O },
            { W, O, W }
        };
        var pathfinder = new AStarPathfinder();
        var result = await pathfinder.FindPath(grid, 0, 0, 1, 1, new PathfindingOptions { FindClosestIfBlocked = true });

        Assert.That(result.Nodes, Is.Empty);
    }
}
