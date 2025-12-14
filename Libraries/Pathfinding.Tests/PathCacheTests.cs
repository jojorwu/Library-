using NUnit.Framework;
using Pathfinding;
using System.Threading.Tasks;

namespace Pathfinding.Tests;

public class PathCacheTests
{
    private const int W = 1; // Walkable
    private const int O = int.MaxValue; // Obstacle

    [TearDown]
    public void TearDown()
    {
        PathCache.Clear();
    }

    [Test]
    public async Task FindPath_WithSameGrid_ReturnsCachedPath()
    {
        var grid = new int[,] { { W, W, W } };
        var pathfinder = new AStarPathfinder();

        var result1 = await pathfinder.FindPath(grid, 0, 0, 2, 0);
        Assert.That(result1.Nodes.Count, Is.EqualTo(3));

        // Create a second, identical grid. The result should come from the cache.
        var grid2 = new int[,] { { W, W, W } };
        var result2 = await pathfinder.FindPath(grid2, 0, 0, 2, 0);

        // A simple way to check if it's the cached instance is by reference.
        Assert.That(ReferenceEquals(result1, result2), Is.True);
    }

    [Test]
    public async Task FindPath_WithDifferentGrid_RecalculatesPath()
    {
        var grid1 = new int[,] { { W, W, W } };
        var pathfinder = new AStarPathfinder();

        var result1 = await pathfinder.FindPath(grid1, 0, 0, 2, 0);
        Assert.That(result1.Nodes.Count, Is.EqualTo(3));

        // Create a different grid. This should force a recalculation.
        var grid2 = new int[,] { { W, O, W } };
        var result2 = await pathfinder.FindPath(grid2, 0, 0, 2, 0);

        Assert.That(result2.Nodes, Is.Empty);
        Assert.That(ReferenceEquals(result1, result2), Is.False);
    }

    [Test]
    public async Task FindPath_WithCacheCleared_RecalculatesPath()
    {
        var grid = new int[,] { { W, W, W } };
        var pathfinder = new AStarPathfinder();

        var result1 = await pathfinder.FindPath(grid, 0, 0, 2, 0);
        Assert.That(result1.Nodes.Count, Is.EqualTo(3));

        PathCache.Clear();
        var result2 = await pathfinder.FindPath(grid, 0, 0, 2, 0);

        Assert.That(result2.Nodes.Count, Is.EqualTo(3));
        Assert.That(ReferenceEquals(result1, result2), Is.False);
    }

    [Test]
    public async Task FindPath_WithUseCacheFalse_BypassesCache()
    {
        var grid = new int[,] { { W, W, W } };
        var pathfinder = new AStarPathfinder();

        var result1 = await pathfinder.FindPath(grid, 0, 0, 2, 0);
        Assert.That(result1.Nodes.Count, Is.EqualTo(3));

        // This call should not use the cache and should not add to it.
        var result2 = await pathfinder.FindPath(grid, 0, 0, 2, 0, useCache: false);

        Assert.That(result2.Nodes.Count, Is.EqualTo(3));
        Assert.That(ReferenceEquals(result1, result2), Is.False);
    }
}
