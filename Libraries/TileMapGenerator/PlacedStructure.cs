namespace TileMapGenerator;

public class PlacedStructure
{
    public Structure Structure { get; }
    public int X { get; }
    public int Y { get; }

    public PlacedStructure(Structure structure, int x, int y)
    {
        Structure = structure;
        X = x;
        Y = y;
    }
}
