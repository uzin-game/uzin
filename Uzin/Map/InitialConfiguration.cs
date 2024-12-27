namespace Uzin.Map;

using Tiles;

public class InitialConfiguration
{
    private readonly int width;
    private readonly int height;

    private readonly BaseTile[,] grid;

    public InitialConfiguration(int width, int height)
    {
        this.width = width;
        this.height = height;

        grid = new BaseTile[width, height];

        BuildConfiguration();
        GenerateCompatibilityLists();
    }

    public void BuildConfiguration()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x < 2 || x >= width - 2 || y < 2 || y >= height - 2)
                {
                    grid[x, y] = new GroundTile();
                }
                else
                {
                    grid[x, y] = new WaterTile();
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] is not WaterTile)
                {
                    continue;
                }

                if (grid[x - 1, y] is GroundTile)
                {
                    grid[x, y] = new HybridTile();
                }

                if (grid[x + 1, y] is GroundTile)
                {
                    grid[x, y] = new HybridTile();
                }

                if (grid[x, y - 1] is GroundTile)
                {
                    grid[x, y] = new HybridTile();
                }

                if (grid[x, y + 1] is GroundTile)
                {
                    grid[x, y] = new HybridTile();
                }
            }
        }
    }

    public void GenerateCompatibilityLists()
    {
        ResetNeighbors(
            GroundTile.UpNeighbors,
            GroundTile.RightNeighbors,
            GroundTile.DownNeighbors,
            GroundTile.LeftNeighbors,
            HybridTile.UpNeighbors,
            HybridTile.RightNeighbors,
            HybridTile.DownNeighbors,
            HybridTile.LeftNeighbors,
            WaterTile.UpNeighbors,
            WaterTile.RightNeighbors,
            WaterTile.DownNeighbors,
            WaterTile.LeftNeighbors
        );

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                BaseTile currentTile = grid[x, y];

                if (y + 1 < height)
                {
                    AddToStaticNeighbors(currentTile, Direction.Up, grid[x, y + 1]);
                }

                if (x + 1 < width)
                {
                    AddToStaticNeighbors(currentTile, Direction.Right, grid[x + 1, y]);
                }

                if (y - 1 >= 0)
                {
                    AddToStaticNeighbors(currentTile, Direction.Down, grid[x, y - 1]);
                }

                if (x - 1 >= 0)
                {
                    AddToStaticNeighbors(currentTile, Direction.Left, grid[x - 1, y]);
                }
            }
        }
    }

    private void ResetNeighbors(params List<BaseTile>[] lists)
    {
        foreach (var list in lists)
        {
            list.Clear();
        }
    }

    private void AddToStaticNeighbors(BaseTile tile, Direction direction, BaseTile neighbor)
    {
        var neighbors = GetTileNeighbors(tile);
        var neighborList = neighbors[direction];

        bool typePresent = false;

        foreach (var neighbour in neighborList)
        {
            if (neighbour.GetType() == neighbor.GetType())
            {
                typePresent = true;
                break;
            }
        }

        if (!typePresent)
        {
            neighborList.Add(neighbor);
        }
    }

    private Dictionary<Direction, List<BaseTile>> GetTileNeighbors(BaseTile tile)
    {
        switch (tile)
        {
            case GroundTile:
                return new Dictionary<Direction, List<BaseTile>>
                {
                    { Direction.Up, GroundTile.UpNeighbors },
                    { Direction.Right, GroundTile.RightNeighbors },
                    { Direction.Down, GroundTile.DownNeighbors },
                    { Direction.Left, GroundTile.LeftNeighbors }
                };

            case WaterTile:
                return new Dictionary<Direction, List<BaseTile>>
                {
                    { Direction.Up, WaterTile.UpNeighbors },
                    { Direction.Right, WaterTile.RightNeighbors },
                    { Direction.Down, WaterTile.DownNeighbors },
                    { Direction.Left, WaterTile.LeftNeighbors }
                };

            case HybridTile:
                return new Dictionary<Direction, List<BaseTile>>
                {
                    { Direction.Up, HybridTile.UpNeighbors },
                    { Direction.Right, HybridTile.RightNeighbors },
                    { Direction.Down, HybridTile.DownNeighbors },
                    { Direction.Left, HybridTile.LeftNeighbors }
                };

            default:
                throw new ArgumentException("Invalid tile type");
        }
    }

    public void PrintGridToConsole()
    {
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                var tile = grid[x, y];

                switch (tile)
                {
                    case GroundTile:
                        Console.BackgroundColor = ConsoleColor.Magenta;
                        break;

                    case WaterTile:
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                        break;

                    case HybridTile:
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        break;
                }

                Console.Write($"{tile.TileName}\t");

                Console.ResetColor();
            }

            Console.WriteLine();
        }
    }

    private enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
}