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

        //könyvtárak létrehozása a számok és betűk átkonvertálásához

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

        // A térkép fejlécének kiírása (az oszlopok számai és az üres mező bal felső sarokban)
        static void PrintHeader()
        {
            //Console színének a megváltoztatása
            ForegroundColor = ConsoleColor.DarkYellow;
            //Üres mező
            Write("[ ]");
            //Számok 1-től 10-ig []-ben
            for (int i = 1; i < 11; i++)
                Write("[" + i + "]");
        }

        //A map kiírása
        public void PrintMap(int[,] map)
        {
            //Meghívjuk a Header függvényt, hogy az is meglegyen
            PrintHeader();
            WriteLine();
            //Létrehozzuk a row character változót, a map sorainak sorszámozásához betűkkel. (A, B, C, D.....)
            char row = 'A';

            //Ezzel a for-al az oszlopokon megyünk végig
            for (int x = 0; x < map.GetLength(0); x++)
            {
                //Ezzzel pedig a sorokon
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    //A sor 0. elemének a helyére beírjuk a már említett sorszámozást
                    if (y == 0)
                    {
                        ForegroundColor = ConsoleColor.DarkYellow;
                        Write("[" + row + "]");
                        row++;
                    }
                    //Ha lehelyeztünk egy hajót, akkor kirajzoljuk azt
                    if (map[x,y] == 1)
                    {
                        ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("[O]");
                    }
                    //Ha nincs a mezőn hajó, akkor hullámot rajzolunk oda
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

            // Ha y = y akkor sor
            // Ha x = x akkor oszlop

            //Ki írjuk a "coordinates" tömb tagjait változókba, hogy könnyebb legyen később kezelni azokat és
            // ne kavarodjuk bele. (f = from, t = to)
            int fx = coordinates[0];
            int fy = coordinates[1];
            int tx = coordinates[2];
            int ty = coordinates[3];

            //Bool változó létrehozása, arra, hogyha már van a mezőn hajó lehelyezve, azt jelezze
            bool exists = false;

            //Megvizsgáljuk, hogy van-e még olyan hajó amit leakar helyezni a player 
            if (ships[ship-1] > 0)
            {
                //Megnézzük, hogy a két bekért érték (a from és a to) X értéke egyezik, ha igen akkor
                //egy oszlopban lesz a hajó
                if (fx == tx)
                {
                    //Végig megyünk a lehelyezni kívánt hajó koordinátáin, hogy nem keresztezi-e egy másik hajó
                    for (int i = fy; i <= ty; i++)
                    {
                        //Ha a Map mátrix fx. és i. elemén az érték 1, a bool változót true-ra módosítjuk,
                        //ezzel jelezve, hogy ott már van hajó
                        if (map[fx, i] == 1)
                        {
                            exists = true;
                        }
                    }

                    //Csak akkor rakjuk le a hajót, ha ott nincs már egy hajó lehelyezve vagy nem kereszetezi egy másik hajót
                    if (!exists)
                    {
                        //A beírt két érték között (pl. xy=2;1 és 2;5) végig megyünk és átváltjuk
                        //a mátrixban a nullákat egyre
                        for (int i = fy; i <= ty; i++)
                        {
                            //A Map mátrix fx.(x tengely) és i.(y tengely) helyét átírjuk 1-re
                            map[fx, i] = 1;
                        }
                    }
                    //Ha egyik lehetőség se volt, az azt jelenti, hogy rossz a beírt koordináta, ezért újra bekérjük
                    else
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine("Balfasz");
                        Question(ships, map);
                    }                   
                }
                //Megnézzük, hogy a két bekért érték Y értéke egyezik-e, ha igen akkor egy sorban lesz a hajó
                //A többi ugyanaz, mint az előbb, csak az x-et és y-ont megcseréltük
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
                //Ha sikeresen lefutott a fenti kódsor, akkor kivonunk a "Ships" tömb jelenlegi eleméből 1-et,
                //ezzzel nullára csökkentve annak értékét, tehát már lehelyeztük azt a hajó típust
                ships[ship - 1]--;
            }
            else
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Balfasz");
                Question(ships, map);
            }
        }

        //Létrehozzunk egy változót a lehelyezni kívánt hajó típusának bekérdezésére. Erre azért van szükség, mert így nem jön létre egy bug amit tapasztaltunk,
        //mert így nem a "Coordinates" függvényt kell meghívnunk, ha rossz a bekért adat, és nem akad össze a már futó "Coordinates" függvénnyel.
        public void Question(int[] ships, int[,] map)
        {
            //Várunk fél másodpercet a kiírt szöveg törlésével, majd töröljük, és meghívjukk a "PrintMap" függvényt,
            //ezzel frissítve a térképet a lehelyezett hajókkal
            Thread.Sleep(500);
            Clear();
            PrintMap(map);

            
            ForegroundColor = ConsoleColor.Cyan;
            WriteLine("Add meg a hajó nevét! (Carrier(1), BattleShip(2), Destroyer(3), Submarine(4), PatrolBoat(5) )");
            ForegroundColor = ConsoleColor.Gray;

            //"ship" változóként mentjük a bekért értéket
            int ship = Int32.Parse(ReadLine());

            //Leellenőrizzük, hogy hibás-e a megadott érték
            if (ship < 1 || ship > 5)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Balfasz");
                Question(ships, map);
            }
            //Leellenőrizzük, hogy a lehelyezni kívánt hajót lehelyeztük-e már
            else if (ships[ship - 1] == 0)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Balfasz");
                Question(ships, map);
            }
            //Ha helyes a beírt adat, akkor meghívjuk a "Coordinates" függvényt, ezzel tovább haladva a bekérdezéssel
            else
            {
                Coordinates(ships, map, ship);
            }
        }

        //Lértehozzuk a "Coordinates" függvényt, a koordináták bekérdezésére és a hajóhosszúság ellenőrzésére
        public void Coordinates(int[] ships, int[,] map, int ship)
        {
            //Carrier = 5
            //Battleship = 4
            //Destroyer = 3
            //Submarine = 3
            //PatrolBoat = 2

            ForegroundColor = ConsoleColor.Cyan;
            WriteLine("Add meg, a hajó kezdőponti és végponti koordinátáit!" + Environment.NewLine
                + "(Fontos hogy ELŐSSZÖR A KEZDŐ koordinátát adjuk meg, UTÁNA AZ UTOLSÓ koordinátát!!!)");
            ForegroundColor = ConsoleColor.Gray;

            //Bekérjük a két értéket, majd átkonvertáljuk amit kell int típussá és szétbontjuk a stringet "karakterekre", az az számokra
            string from = ReadLine();
            //Hasznájuk a már korábban létrehozott könyvtárat, hogy a betűket számmá alakítsuk
            int fromX = mapCharToIntDict[from[0]];
            int fromY = Int32.Parse(from[1].ToString()) - 1;
            string to = ReadLine();
            int toX = mapCharToIntDict[to[0]];
            int toY = Int32.Parse(to[1].ToString()) - 1;

            //A létrejött int változókat egy tömbbe rakjuk, hogy könnyen tudjuk használni a place függvényben is
            int[] coordinates = {fromX, fromY, toX, toY };

            //Ezeknél az if-eknél megnézzük, hogy melyik hajót választotta a kedves áldozat
            if (ship == 1)
            {
                //Ezt már leírtam fentebb!
                //Miért nem figyelsz arra, hogy mit olvasol?!
                if (fromX == toX)
                {
                    //Ezeknél az if-eknél ellenőrizzük, hogy megfelelő hosszúságot adott-e meg a hajók lehelyezésénél
                    if (fromY - toY == 4 || fromY - toY == -4)
                    {
                        //Meghívjuk a "Place" függvényt és nyugtázzuk a sikeres a lehelyezést egy "Nice"-al
                        Place(ships, map, ship, coordinates);
                        ForegroundColor = ConsoleColor.Green; 
                        WriteLine("Nice");
                    }
                    //Már ezt is leírtam fentebb.
                    //Hányszor mondjam még el?
                    else
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine("Balfasz");
                        Question(ships, map);
                    }
                }
                //Ez ugyan az, mint az előző csak fordítva, meg amúgyis leírtam már egyszer, ha nem tűnt volna fel.
                else if (fromY == toY)
                {
                    if (fromX - toX == 4 || fromX - toX == -4)
                    {
                        Place(ships, map, ship, coordinates);
                        ForegroundColor = ConsoleColor.Green; 
                        WriteLine("Nice");
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
            //Innentől csak ismételgetjük a fentit, minden hajótípusnál, a hajóhosszúságot megváltoztatva
            else if (ship == 2)
            {
                if (fromX == toX)
                {
                    if (fromY - toY == 3 || fromY - toY == -3)
                    {
                        Place(ships, map, ship, coordinates);
                        ForegroundColor = ConsoleColor.Green; 
                        WriteLine("Nice");
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
                        ForegroundColor = ConsoleColor.Green;
                        WriteLine("Nice");
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
                        ForegroundColor = ConsoleColor.Green; 
                        WriteLine("Nice");
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
                        ForegroundColor = ConsoleColor.Green; 
                        WriteLine("Nice");
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
                        ForegroundColor = ConsoleColor.Green; 
                        WriteLine("Nice");
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
                        ForegroundColor = ConsoleColor.Green;
                        WriteLine("Nice");
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

    //Végre jön a játék meghívása
    class Program
    {
        static void Main(string[] args)
        {
            //Átrakjuk a konzol megjelenítését UTF-8-ra, hogy látszódjanak a hosszú magánhangzók, ha már ekkora gyász nyelv ez a magyar.
            Console.OutputEncoding = Encoding.UTF8;

            //Létre hozzuk a már sokat emlegetett Map mátrixot (vagy mit xd)
            int[,] Map = new int[10, 10];
            //Ebben a tömbben tároljuk a hajótípusokat, abban a sorrendben, ahogy kiírtuk a konzolra a választásnál
            //(Carrier(1), BattleShip(2), Destroyer(3), Submarine(4), PatrolBoat(5)
            int[] Ships = { 1, 1, 1, 1, 1 };

            //A Torpedo osztályt "game"-ként "hozzuk" lét re
            Torpedo game = new Torpedo();

            //Meghívjuk a "PrintMap" függvényt, ezzel kirajzolva a map-ot
            game.PrintMap(Map);

            //Meghívjuk 5-ször a "Question" függvényt, ezzel elindítva a bekérdezést és a játékot is
            for (int i = 0; i < 5; i++)
            {
                game.Question(Ships, Map);
            }

            //Valszeg ideiglenes megoldás az utolsó lehelyezett hajó megtekintéséhez.
            game.PrintMap(Map);
        }        
    }
}


//Bugok:
//Nem lehet a 10-es oszlopban lehelyezni hajókat