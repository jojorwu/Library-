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
        var map = generator.Generate(10, 20, 10.0f, 0, mappings, new List<Structure>(), new List<PlacedStructure>());
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
        var map = generator.Generate(10, 10, 10.0f, 0, mappings, new List<Structure>(), new List<PlacedStructure>());
        var uniqueIds = map.Cast<int>().Distinct().ToList();

        Assert.That(uniqueIds, Is.SubsetOf(new[] { 1, 2 }));
    }

    [Test]
    public void Generate_WithPlacedStructure_PlacesStructureAtCorrectCoordinates()
    {
        var generator = new MapGenerator();
        var mappings = new List<NoiseMapping> { new(1.0f, new TileDefinition(1)) };
        var structure = new Structure(new[,] { { 9, 9 }, { 9, 9 } });
        var placedStructures = new List<PlacedStructure> { new(structure, 2, 3) };

        var map = generator.Generate(10, 10, 1.0f, 0, mappings, new List<Structure>(), placedStructures);

        Assert.That(map[2, 3], Is.EqualTo(9));
        Assert.That(map[3, 3], Is.EqualTo(9));
        Assert.That(map[2, 4], Is.EqualTo(9));
        Assert.That(map[3, 4], Is.EqualTo(9));
        Assert.That(map[0, 0], Is.EqualTo(1));
    }

    [Test]
    public void Generate_WithRandomStructure_PlacesStructureOnMap()
    {
        var generator = new MapGenerator();
        var mappings = new List<NoiseMapping> { new(1.0f, new TileDefinition(1)) };
        var structure = new Structure(new[,] { { 9 } });
        var randomStructures = new List<Structure> { structure };

        var map = generator.Generate(10, 10, 1.0f, 0, mappings, randomStructures, new List<PlacedStructure>());

        var containsStructure = map.Cast<int>().Any(id => id == 9);
        Assert.That(containsStructure, Is.True);
    }

    [Test]
    public void Generate_WithSameSeed_ProducesIdenticalMaps()
    {
        var generator = new MapGenerator();
        var mappings = new List<NoiseMapping> { new(1.0f, new TileDefinition(1)) };
        var structure = new Structure(new[,] { { 9 } });
        var randomStructures = new List<Structure> { structure };

        var map1 = generator.Generate(20, 20, 5.0f, 12345, mappings, randomStructures, new List<PlacedStructure>());
        var map2 = generator.Generate(20, 20, 5.0f, 12345, mappings, randomStructures, new List<PlacedStructure>());

        Assert.That(map1, Is.EqualTo(map2));
    }

    [Test]
    public void Generate_WithDifferentSeeds_ProducesDifferentMaps()
    {
        var generator = new MapGenerator();
        var mappings = new List<NoiseMapping> { new(1.0f, new TileDefinition(1)) };
        var structure = new Structure(new[,] { { 9 } });
        var randomStructures = new List<Structure> { structure };

        var map1 = generator.Generate(20, 20, 5.0f, 12345, mappings, randomStructures, new List<PlacedStructure>());
        var map2 = generator.Generate(20, 20, 5.0f, 54321, mappings, randomStructures, new List<PlacedStructure>());

        Assert.That(map1, Is.Not.EqualTo(map2));
    }
}
