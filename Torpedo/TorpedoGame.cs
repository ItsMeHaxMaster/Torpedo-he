namespace Torpedo;

public class TorpedoGame
{
    static Dictionary<int, string> mapDict = new Dictionary<int, string>()
    {
        {0, "O"},
        {1, "X"}
    };

    static Dictionary<int, char> mapIntToCharDict = new Dictionary<int, char>()
    {
        {0, 'A'},
        {1, 'B'},
        {2, 'C'},
        {3, 'D'},
        {4, 'E'},
        {5, 'F'},
        {6, 'G'},
        {7, 'H'},
        {8, 'I'},
        {9, 'J'}
    };

    static Dictionary<char, int> mapCharToIntDict = new Dictionary<char, int>()
    {
        {'A', 0},
        {'B', 1},
        {'C', 2},
        {'D', 3},
        {'E', 4},
        {'F', 5},
        {'G', 6},
        {'H', 7},
        {'I', 8},
        {'J', 9}
    };

    public void PrintMap(int[,] map)
    {
        Console.Write("    ");

        for(int i = 0; i < map.GetLength(1); i++)
        {
            Console.Write((i + 1) + " ");
        }

        Console.WriteLine();
        Console.Write("  \u250c");

        for (int i = 0; i <= map.GetLength(1) * 2; i++)
        {
            Console.Write("\u2500");
        }

        Console.WriteLine();

        for (int y = 0; y < map.GetLength(0); y++)
        {
            Console.Write(mapIntToCharDict[y] + " \u2502 ");

            for (int x = 0; x < map.GetLength(1); x++)
            {
                Console.Write(mapDict[map[y, x]] + " ");
            }

            Console.WriteLine();
        }
    }
}