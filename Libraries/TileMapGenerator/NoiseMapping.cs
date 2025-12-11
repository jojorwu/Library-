namespace TileMapGenerator;

public class NoiseMapping
{
    public float Threshold { get; }
    public TileDefinition Tile { get; }

    public NoiseMapping(float threshold, TileDefinition tile)
    {
        Threshold = threshold;
        Tile = tile;
    }
}
