namespace Uzin.Tiles;

public class WaterTile : BaseTile
{
    public static List<BaseTile> UpNeighbors { get; } = new();
    public static List<BaseTile> RightNeighbors { get; } = new();
    public static List<BaseTile> DownNeighbors { get; } = new();
    public static List<BaseTile> LeftNeighbors { get; } = new();

    public WaterTile() : base("Water")
    {
    }
}