namespace Uzin.Tiles;

public class HybridTile : BaseTile
{
    public static List<BaseTile> UpNeighbors { get; } = new();
    public static List<BaseTile> RightNeighbors { get; } = new();
    public static List<BaseTile> DownNeighbors { get; } = new();
    public static List<BaseTile> LeftNeighbors { get; } = new();

    public HybridTile() : base("Hybrid")
    {
    }
}