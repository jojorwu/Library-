using System.Collections.Generic;
using System.Linq;

namespace TileMapGenerator;

public class MapGenerator
{
    public int[,] Generate(int width, int height, float scale, List<NoiseMapping> mappings)
    {
        var map = new int[width, height];
        var sortedMappings = mappings.OrderBy(m => m.Threshold).ToList();

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var noise = PerlinNoise.Generate(x / scale, y / scale);
                var tile = sortedMappings.First(m => noise <= m.Threshold).Tile;
                map[x, y] = tile.Id;
            }
        }

        return map;
    }
}
