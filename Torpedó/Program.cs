using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace Torpedó
{
    class Torpedo
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
            {'J', 9},
            {'a', 0},
            {'b', 1},
            {'c', 2},
            {'d', 3},
            {'e', 4},
            {'f', 5},
            {'g', 6},
            {'h', 7},
            {'i', 8},
            {'j', 9}
        };


        static void PrintHeader()
        {
            ForegroundColor = ConsoleColor.DarkYellow;
            Write("[ ]");
            for (int i = 1; i < 11; i++)
                Write("[" + i + "]");
        }

        public void PrintMap(int[,] map)
        {
            PrintHeader();
            WriteLine();
            char row = 'A';

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (y == 0)
                    {
                        ForegroundColor = ConsoleColor.DarkYellow;
                        Write("[" + row + "]");
                        row++;
                    }
                    if (map[x,y] == 1)
                    {
                        ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("[O]");
                    }
                    else if (map[x, y] == 0)
                    {
                        ForegroundColor = ConsoleColor.Blue;
                        Console.Write("[~]");
                    }
                }

                Console.WriteLine();
            }
        }

        public void Place(int[] ships, int[,] map, int ship, int[] coordinates)
        {

            // ha y = y akkor sor
            // ha x = x akkor oszlop

            int fx = coordinates[0];
            int fy = coordinates[1];
            int tx = coordinates[2];
            int ty = coordinates[3];

            bool exists = false;

            if (ships[ship-1] > 0)
            {          
                if (fx == tx)
                {
                    for (int i = fy; i <= ty; i++)
                    {
                        if (map[fx, i] == 1)
                        {
                            exists = true;
                        }
                    }

                    if (!exists)
                    {
                        for (int i = fy; i <= ty; i++)
                        {
                            map[fx, i] = 1;
                        }
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine("Balfasz");
                        Question(ships, map);
                    }                   
                }
                else if (fy == ty)
                {
                    for (int i = fx; i <= tx; i++)
                    {
                        if (map[i, fy] == 1)
                        {
                            exists = true;
                        }
                    }

                    if (!exists)
                    {
                        for (int i = fx; i <= tx; i++)
                        {
                            map[i, fy] = 1;
                        }
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine("Balfasz");
                        Question(ships, map);
                    }
                }
                ships[ship - 1]--;
            }
            else
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Balfasz");
                Question(ships, map);
            }
        }

        public void Question(int[] ships, int[,] map)
        {
            Thread.Sleep(500);
            Clear();
            PrintMap(map);

            ForegroundColor = ConsoleColor.Cyan;
            WriteLine("Add meg a hajó nevét! (Carrier(1), BattleShip(2), Destroyer(3), Submarine(4), PatrolBoat(5) )");
            ForegroundColor = ConsoleColor.Gray;

            int ship = Int32.Parse(ReadLine());

            if (ship < 1 || ship > 5)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Balfasz");
                Question(ships, map);
            }
            else if (ships[ship - 1] == 0)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Balfasz");
                Question(ships, map);
            }
            else
            {
                Place_Command(ships, map, ship);
            }
        }

        public void Place_Command(int[] ships, int[,] map, int ship)
        {
            //Carrier = 5
            //Battleship = 4
            //Destroyer = 3
            //Submarine = 3
            //PatrolBoat = 2
            ForegroundColor = ConsoleColor.Cyan;
            WriteLine("Add meg, hogy mettől, meddig akarod lerakni!");
            ForegroundColor = ConsoleColor.Gray;

            string from = ReadLine();
            int fromX = mapCharToIntDict[from[0]];
            int fromY = Int32.Parse(from[1].ToString()) - 1;
            string to = ReadLine();
            int toX = mapCharToIntDict[to[0]];
            int toY = Int32.Parse(to[1].ToString()) - 1;

            int[] coordinates = {fromX, fromY, toX, toY };

            ForegroundColor = ConsoleColor.Green;

            if (ship == 1)
            {
                if (fromX == toX)
                {
                    if (fromY - toY == 4 || fromY - toY == -4)
                    {
                        Place(ships, map, ship, coordinates);
                        WriteLine("Nice");
                        //Be kell rakni a mapbe
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine("Balfasz");
                        Question(ships, map);
                    }
                }
                else if (fromY == toY)
                {
                    if (fromX - toX == 4 || fromX - toX == -4)
                    {
                        Place(ships, map, ship, coordinates);
                        WriteLine("Nice");
                        //Be kell rakni a mapbe
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine("Balfasz");
                        Question(ships, map);
                    }
                }
                else
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("Balfasz");
                    Question(ships, map);
                }
            }
            else if (ship == 2)
            {
                if (fromX == toX)
                {
                    if (fromY - toY == 3 || fromY - toY == -3)
                    {
                        Place(ships, map, ship, coordinates);
                        WriteLine("Nice");
                        //Be kell rakni a mapbe
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine("Balfasz");
                        Question(ships, map);
                    }
                }
                else if (fromY == toY)
                {
                    if (fromX - toX == 3 || fromX - toX == -3)
                    {
                        Place(ships, map, ship, coordinates);
                        WriteLine("Nice");
                        //Be kell rakni a mapbe
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine("Balfasz");
                        Question(ships, map);
                    }
                }
                else
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("Balfasz");
                    Question(ships, map);
                }
            }
            else if (ship == 3 || ship == 4)
            {
                if (fromX == toX)
                {
                    if (fromY - toY == 2 || fromY - toY == -2)
                    {
                        Place(ships, map, ship, coordinates);
                        WriteLine("Nice");
                        //Be kell rakni a mapbe
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine("Balfasz");
                        Question(ships, map);
                    }
                }
                else if (fromY == toY)
                {
                    if (fromX - toX == 2 || fromX - toX == -2)
                    {
                        Place(ships, map, ship, coordinates);
                        WriteLine("Nice");
                        //Be kell rakni a mapbe
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine("Balfasz");
                        Question(ships, map);
                    }
                }
                else
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("Balfasz");
                    Question(ships, map);
                }
            }
            else if (ship == 5)
            {
                if (fromX == toX)
                {
                    if (fromY - toY == 1 || fromY - toY == -1)
                    {
                        Place(ships, map, ship, coordinates);
                        WriteLine("Nice");
                        //Be kell rakni a mapbe
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine("Balfasz");
                        Question(ships, map);
                    }
                }
                else if (fromY == toY)
                {
                    if (fromX - toX == 1 || fromX - toX == -1)
                    {
                        Place(ships, map, ship, coordinates);
                        WriteLine("Nice");
                        //Be kell rakni a mapbe
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine("Balfasz");
                        Question(ships, map);
                    }
                }
                else
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("Balfasz");
                    Question(ships, map);
                }
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            int[,] Map = new int[10, 10];
            int[] Ships = { 1, 1, 1, 1, 1 };
            Torpedo game = new Torpedo();
            game.PrintMap(Map);

            for (int i = 0; i < 5; i++)
            {
                game.Question(Ships, Map);
            }

            game.PrintMap(Map);
        }

        
    }
}
