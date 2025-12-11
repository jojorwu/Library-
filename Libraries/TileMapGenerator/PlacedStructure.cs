namespace TileMapGenerator;

/// <summary>
/// Represents a structure that is placed at a specific location on the map.
/// </summary>
public class PlacedStructure
{
    /// <summary>
    /// The structure to place.
    /// </summary>
    public Structure Structure { get; }
    /// <summary>
    /// The x-coordinate of the top-left corner of the structure.
    /// </summary>
    public int X { get; }
    /// <summary>
    /// The y-coordinate of the top-left corner of the structure.
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlacedStructure"/> class.
    /// </summary>
    /// <param name="structure">The structure to place.</param>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    public PlacedStructure(Structure structure, int x, int y)
    {
        Structure = structure;
        X = x;
        Y = y;
    }
}
