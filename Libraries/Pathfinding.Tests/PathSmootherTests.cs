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
    public async Task SmoothPath_WithSimpleJaggedPath_ReturnsStraightPath()
    {
        var grid = new int[,]
        {
            { W, W, W, W, W },
            { W, W, W, W, W },
            { W, W, W, W, W },
        };

        var pathfinder = new AStarPathfinder();
        // Get a jagged path first, then smooth it
        var jaggedPath = await pathfinder.FindPath(grid, 0, 0, 4, 2);
        var smoothedPath = PathSmoother.SmoothPath(grid, jaggedPath);

        var expectedPath = new List<(int, int)> { (0, 0), (4, 2) };
        var actualPath = smoothedPath.Select(p => (p.X, p.Y)).ToList();

        Assert.That(actualPath, Is.EqualTo(expectedPath));
    }

    [Test]
    public async Task SmoothPath_WithPathAroundObstacle_DoesNotCutCorner()
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
        var jaggedPath = await pathfinder.FindPath(grid, 1, 2, 3, 2);
        var smoothedPath = PathSmoother.SmoothPath(grid, jaggedPath);

        // The correct smoothed path should navigate around the corners of the obstacle
        var expectedPath = new List<(int, int)> { (1, 2), (1, 0), (3, 0), (3, 2) };
        var actualPath = smoothedPath.Select(p => (p.X, p.Y)).ToList();

        Assert.That(actualPath, Is.EqualTo(expectedPath));
    }

    [Test]
    public async Task FindPath_WithSmoothingEnabled_ReturnsSmoothedPath()
    {
        var grid = new int[,]
        {
            { W, W, W, W, W },
            { W, W, W, W, W },
            { W, W, W, W, W },
        };

        var pathfinder = new AStarPathfinder();
        var smoothedPath = await pathfinder.FindPath(grid, 0, 0, 4, 2, true);

        var expectedPath = new List<(int, int)> { (0, 0), (4, 2) };
        var actualPath = smoothedPath.Select(p => (p.X, p.Y)).ToList();

        Assert.That(actualPath, Is.EqualTo(expectedPath));
    }
}
