using System;
using System.Collections.Generic;
using System.Linq;

namespace TileMapGenerator;

/// <summary>
/// Generates a procedural tile map using Perlin noise.
/// </summary>
public class MapGenerator
{
    /// <summary>
    /// Generates a tile map.
    /// </summary>
    /// <param name="width">The width of the map.</param>
    /// <param name="height">The height of the map.</param>
    /// <param name="scale">The scale of the Perlin noise. Higher values result in more zoomed-in terrain.</param>
    /// <param name="seed">The seed for the random number generator, ensuring deterministic output.</param>
    /// <param name="mappings">A list of noise thresholds mapped to tile definitions.</param>
    /// <param name="randomStructures">A list of structures to be placed at random locations.</param>
    /// <param name="placedStructures">A list of structures to be placed at specific locations.</param>
    /// <returns>A 2D integer array representing the generated tile map.</returns>
    public int[,] Generate(
        int width, int height, float scale, int seed,
        List<NoiseMapping> mappings,
        List<Structure> randomStructures,
        List<PlacedStructure> placedStructures)
    {
        var map = new int[width, height];
        var sortedMappings = mappings.OrderBy(m => m.Threshold).ToList();

        PerlinNoise.Initialize(seed);

        // 1. Generate base terrain
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var noise = PerlinNoise.Generate(x / scale, y / scale);
                var tile = sortedMappings.First(m => noise <= m.Threshold).Tile;
                map[x, y] = tile.Id;
            }
        }

        // 2. Place random structures
        var random = new Random(seed);
        foreach (var structure in randomStructures)
        {
            var x = random.Next(width - structure.Tiles.GetLength(0));
            var y = random.Next(height - structure.Tiles.GetLength(1));
            PlaceStructure(map, structure, x, y);
        }

        // 3. Place fixed structures
        foreach (var placedStructure in placedStructures)
        {
            PlaceStructure(map, placedStructure.Structure, placedStructure.X, placedStructure.Y);
        }

        return map;
    }

    private void PlaceStructure(int[,] map, Structure structure, int x, int y)
    {
        var width = structure.Tiles.GetLength(0);
        var height = structure.Tiles.GetLength(1);
        for (var j = 0; j < height; j++)
        {
            for (var i = 0; i < width; i++)
            {
                if (x + i < map.GetLength(0) && y + j < map.GetLength(1))
                {
                    map[x + i, y + j] = structure.Tiles[i, j];
                }
            }
        }
    }
}
