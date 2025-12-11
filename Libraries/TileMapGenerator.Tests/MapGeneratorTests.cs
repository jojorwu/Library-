using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TileMapGenerator;

namespace TileMapGenerator.Tests;

public class MapGeneratorTests
{
    [Test]
    public void Generate_WithValidInputs_ReturnsMapWithCorrectDimensions()
    {
        var generator = new MapGenerator();
        var mappings = new List<NoiseMapping>
        {
            new(0.5f, new TileDefinition(1)),
            new(1.0f, new TileDefinition(2))
        };
        var map = generator.Generate(10, 20, 10.0f, mappings);
        Assert.That(map.GetLength(0), Is.EqualTo(10));
        Assert.That(map.GetLength(1), Is.EqualTo(20));
    }

    [Test]
    public void Generate_WithValidInputs_ReturnsMapWithCorrectTileIds()
    {
        var generator = new MapGenerator();
        var mappings = new List<NoiseMapping>
        {
            new(0.5f, new TileDefinition(1)),
            new(1.0f, new TileDefinition(2))
        };
        var map = generator.Generate(10, 10, 10.0f, mappings);
        var uniqueIds = map.Cast<int>().Distinct().ToList();

        Assert.That(uniqueIds, Is.SubsetOf(new[] { 1, 2 }));
    }
}
