using NUnit.Framework;
using Pathfinding;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pathfinding.Tests;

public class PathSmootherTests
{
    private const int W = 1; // Walkable
    private const int O = int.MaxValue; // Obstacle

    [Test]
    public async Task SmoothPath_WithSimpleJaggedPath_ReturnsStraightPathAndCorrectCost()
    {
        var grid = new int[,]
        {
            { W, W, W, W, W },
            { W, W, W, W, W },
            { W, W, W, W, W },
        };

        var pathfinder = new AStarPathfinder();
        var jaggedPathResult = await pathfinder.FindPath(grid, 0, 0, 4, 2);
        var smoothedResult = PathSmoother.SmoothPath(grid, jaggedPathResult);

        var expectedPath = new List<(int, int)> { (0, 0), (4, 2) };
        var actualPath = smoothedResult.Nodes.Select(p => (p.X, p.Y)).ToList();

        Assert.That(actualPath, Is.EqualTo(expectedPath));
        Assert.That(smoothedResult.TotalCost, Is.EqualTo(49));
    }

    [Test]
    public async Task SmoothPath_WithPathAroundObstacle_DoesNotCutCornerAndHasCorrectCost()
    {
        var grid = new int[,]
        {
            { W, W, W, W, W },
            { W, W, O, W, W },
            { W, W, O, W, W },
            { W, W, O, W, W },
            { W, W, W, W, W },
        };

        var pathfinder = new AStarPathfinder();
        var jaggedPathResult = await pathfinder.FindPath(grid, 1, 2, 3, 2);
        var smoothedResult = PathSmoother.SmoothPath(grid, jaggedPathResult);

        var expectedPath = new List<(int, int)> { (1, 2), (1, 0), (3, 0), (3, 2) };
        var actualPath = smoothedResult.Nodes.Select(p => (p.X, p.Y)).ToList();

        Assert.That(actualPath, Is.EqualTo(expectedPath));
        Assert.That(smoothedResult.TotalCost, Is.EqualTo(63));
    }

    [Test]
    public async Task FindPath_WithSmoothingEnabled_ReturnsSmoothedPathAndCorrectCost()
    {
        var grid = new int[,]
        {
            { W, W, W, W, W },
            { W, W, W, W, W },
            { W, W, W, W, W },
        };

        var pathfinder = new AStarPathfinder();
        var smoothedResult = await pathfinder.FindPath(grid, 0, 0, 4, 2, true);

        var expectedPath = new List<(int, int)> { (0, 0), (4, 2) };
        var actualPath = smoothedResult.Nodes.Select(p => (p.X, p.Y)).ToList();

        Assert.That(actualPath, Is.EqualTo(expectedPath));
        Assert.That(smoothedResult.TotalCost, Is.EqualTo(49));
    }
}
