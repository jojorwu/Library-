namespace TileMapGenerator;

/// <summary>
/// Represents a structure or pattern of tiles.
/// </summary>
public class Structure
{
    /// <summary>
    /// A 2D array of tile IDs representing the structure.
    /// </summary>
    public int[,] Tiles { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Structure"/> class.
    /// </summary>
    /// <param name="tiles">A 2D array of tile IDs.</param>
    public Structure(int[,] tiles)
    {
        Tiles = tiles;
    }
}
