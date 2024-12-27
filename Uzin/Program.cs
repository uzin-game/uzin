namespace Uzin;

using Map;
using Tiles;

class Program
{
    public static void Main(string[] args)
    {
        int width = 10;
        int height = 10;

        InitialConfiguration example = new InitialConfiguration(width, height);

        Console.WriteLine("Voisins 'Down' pour HybridTile:");

        foreach (var neighbor in HybridTile.DownNeighbors)
        {
            Console.WriteLine($"- {neighbor.TileName}");
        }

        Console.WriteLine();

        example.PrintGridToConsole();
    }
}