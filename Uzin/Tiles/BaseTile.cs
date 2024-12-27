namespace Uzin.Tiles;

public abstract class BaseTile
{
    public string TileName { get; }

    protected BaseTile(string tileName)
    {
        TileName = tileName;
    }
}