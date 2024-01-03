namespace Torpedo;

public class TorpedoGame {
    static Dictionary<int, string> displayMap = new Dictionary<int, string>() {
        {-1, " "},
        {0, "~"},
        {1, "+"},
    };

    static Dictionary<int, char> coordChar = new Dictionary<int, char>() {
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

    static Dictionary<int, int> shipSizes = new Dictionary<int, int>() {
        {1, 5},
        {2, 4},
        {3, 3},
        {4, 3},
        {5, 2}
    };

    public void PrintMap(int[,] map) {
        for (int y = 0; y < map.GetLength(0); y++) {
            for (int x = 0; x < map.GetLength(1); x++) {
                if (displayMap[map[y, x]] == "+") {
                    Console.ForegroundColor = ConsoleColor.Red;
                } else {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                Console.Write("[" + displayMap[map[y, x]] + "]");

                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine();
        }
    }

    public bool CanPlace(int[,] map, int[] coords, int shipType) {
        int fromX = coords[0];
        int fromY = coords[1];

        int toX = coords[2];
        int toY = coords[3];

        if (fromX == toX) {
            if (toY - fromY != shipSizes[shipType])
                return false;

            for (int i = fromY; i <= toY; i++)
                if (map[i, fromX] != 0)
                    return false;

            return true;
        }

        if (fromY == toY) {
            if (toX - fromX != shipSizes[shipType])
                return false;

            for (int i = fromX; i <= toX; i++)
                if (map[fromY, i] != 0)
                    return false;

            return true;
        }

        return false;
    }

    public void Place(int[,] map, int[] coords, int shipType) {
        int fromX = coords[0];
        int fromY = coords[1];

        int toX = coords[2];
        int toY = coords[3];

        if (fromX == toX) {
            for (int i = fromY; i < toY; i++)
                map[i, fromX] = 1;
        }

        if (fromY == toY) {
            for (int i = fromX; i < toX; i++)
                map[fromY, i] = 1;
        }
    }

    public void AiGenerate(int[,] map) {
        Random randInt = new Random();

        for (int i = 0; i < 5; i++) {
            bool isHorizontal = randInt.Next(0, 100) < 50;

            Console.WriteLine(shipSizes[i + 1]);

            bool cant = true;

            while (cant) {
                if (isHorizontal) {
                    int columnRand = randInt.Next(0, 10);
                    int rowRand    = randInt.Next(0, 10 - shipSizes[i + 1]);

                    int fromX = rowRand;
                    int fromY = columnRand;

                    int toX = rowRand + shipSizes[i + 1];
                    int toY = columnRand;

                    if (this.CanPlace(map, [ fromX, fromY, toX, toY ], i + 1)) {
                        Place(map, [ fromX, fromY, toX, toY ], i + 1);

                        cant = false;
                    } else {
                        Console.WriteLine("cannot");
                    }
                } else {
                    int rowRand    = randInt.Next(0, 10);
                    int columnRand = randInt.Next(0, 10 - shipSizes[i + 1]);

                    int fromX = rowRand;
                    int fromY = columnRand;

                    int toX = rowRand;
                    int toY = columnRand + shipSizes[i + 1];

                    if (this.CanPlace(map, [ fromX, fromY, toX, toY ], i + 1)) {
                        Place(map, [ fromX, fromY, toX, toY ], i + 1);

                        cant = false;
                    } else {
                        Console.WriteLine("cannot");
                    }
                }
            }
        }
    }
}