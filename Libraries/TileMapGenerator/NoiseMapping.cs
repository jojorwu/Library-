namespace TileMapGenerator;

/// <summary>
/// Maps a Perlin noise threshold to a specific tile definition.
/// </summary>
public class NoiseMapping
{
    /// <summary>
    /// The noise value threshold.
    /// </summary>
    public float Threshold { get; }
    /// <summary>
    /// The tile definition to use when the noise value is below the threshold.
    /// </summary>
    public TileDefinition Tile { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoiseMapping"/> class.
    /// </summary>
    /// <param name="threshold">The noise value threshold.</param>
    /// <param name="tile">The tile definition.</param>
    public NoiseMapping(float threshold, TileDefinition tile)
    {
        Threshold = threshold;
        Tile = tile;
    }
}
