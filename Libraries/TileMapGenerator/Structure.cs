namespace TileMapGenerator;

public class Structure
{
    public int[,] Tiles { get; }

    public Structure(int[,] tiles)
    {
        Tiles = tiles;
    }
}
