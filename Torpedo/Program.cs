using Spectre.Console;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;


namespace Torpedo
{
    class Torpedo
    {

        //Könyvtárak létrehozása a számok és betűk átkonvertálásához

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

        static Dictionary<int, int> shipSizes = new Dictionary<int, int>()
        {
            {1, 5},
            {2, 4},
            {3, 3},
            {4, 3},
            {5, 2}
        };

        // A térkép fejlécének kiírása (az oszlopok számai és az üres mező bal felső sarokban)
        static void PrintHeader()
        {

            //Bal felül lévő ikon
            AnsiConsole.Write(new Markup("[gold3_1]⚓ [/]"));
            //Számok 1-től 10-ig []-ben
            for (int i = 1; i < 11; i++)
                //Custom color a kiíráshoz
                AnsiConsole.Write(new Markup("[gold3_1][[" + i + "]][/]"));
            Write("     ");
            AnsiConsole.Write(new Markup("[gold3_1]💢 [/]"));
            for (int i = 1; i < 11; i++)
                AnsiConsole.Write(new Markup("[gold3_1][[" + i + "]][/]"));
        }

        //A map kiírása
        public void PrintMap(int[,] map, int[,] aimap)
        {

            bool visible = true;

            //Meghívjuk a Header függvényt, hogy az is meglegyen
            PrintHeader();
            WriteLine();
            //Létrehozzuk a row character változót, a map sorainak sorszámozásához betűkkel. (A, B, C, D.....)
            char row1 = 'A';
            char row2 = 'A';

            //Ezzel a for-al az oszlopokon megyünk végig
            for (int x = 0; x < map.GetLength(0); x++)
            {
                //Ezzzel pedig a sorokon
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    //A sor 0. elemének a helyére beírjuk a már említett sorszámozást
                    if (y == 0)
                    {                       
                        AnsiConsole.Write(new Markup("[gold3_1][[" + row1 + "]][/]"));
                        row1++;
                    }
                    //Ha lehelyeztünk egy hajót, akkor kirajzoljuk azt
                    if (map[x, y] == 1)
                    {
                        AnsiConsole.Write(new Markup("[green3][[O]][/]"));
                    }
                    //Ha nincs a mezőn hajó, akkor hullámot rajzolunk oda
                    else if (map[x, y] == 0)
                    {
                        AnsiConsole.Write(new Markup("[navy][[~]][/]"));
                    }
                }

                for (int y = 0; y < aimap.GetLength(1); y++)
                {
                    //A sor 0. elemének a helyére beírjuk a már említett sorszámozást
                    if (y == 0)
                    {
                        Write("      ");
                        AnsiConsole.Write(new Markup("[gold3_1][[" + row2 + "]][/]"));
                        row2++;
                    }
                    //Ha lehelyeztünk egy hajót, akkor kirajzoljuk azt
                    if (aimap[x, y] == 1)
                    {
                        if (visible)
                        {
                            AnsiConsole.Write(new Markup("[red][[X]][/]"));
                        }
                        else
                        {
                            AnsiConsole.Write(new Markup("[navy][[~]][/]"));
                        }
                    }
                    //Ha nincs a mezőn hajó, akkor hullámot rajzolunk oda
                    else if (aimap[x, y] == 0)
                    {
                        AnsiConsole.Write(new Markup("[navy][[~]][/]"));
                    }
                }

                Console.WriteLine();
            }
        }

        public void Place(int[] ships, int[,] map, int[,] aimap, int ship, int[] coordinates)
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
            if (ships[ship - 1] > 0)
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
                        AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                        Question(ships, map, aimap);
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
                        
                        AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                        Question(ships, map, aimap);
                    }
                }
                //Ha sikeresen lefutott a fenti kódsor, akkor kivonunk a "Ships" tömb jelenlegi eleméből 1-et,
                //ezzzel nullára csökkentve annak értékét, tehát már lehelyeztük azt a hajó típust
                ships[ship - 1]--;
            }
            else
            {
                
                AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                Question(ships, map, aimap);
            }
        }

        //Létrehozzunk egy változót a lehelyezni kívánt hajó típusának bekérdezésére. Erre azért van szükség, mert így nem jön létre egy bug amit tapasztaltunk,
        //mert így nem a "Coordinates" függvényt kell meghívnunk, ha rossz a bekért adat, és nem akad össze a már futó "Coordinates" függvénnyel.
        public void Question(int[] ships, int[,] map, int[,] aimap)
        {
            //Várunk fél másodpercet a kiírt szöveg törlésével, majd töröljük, és meghívjukk a "PrintMap" függvényt,
            //ezzel frissítve a térképet a lehelyezett hajókkal
            Thread.Sleep(500);
            Clear();
            PrintMap(map,aimap);


           AnsiConsole.Write(new Markup("[cyan3]Add meg a hajó sorszámát! (1. Carrier(5), 2. BattleShip(4), 3. Destroyer(3), 4. Submarine(3), 5. PatrolBoat(2) )[/]"));
            WriteLine(" ");
            ForegroundColor = ConsoleColor.White;

            //"ship" változóként mentjük a bekért értéket
            int ship = Int32.Parse(ReadLine());       

            //Leellenőrizzük, hogy hibás-e a megadott érték
            if (ship < 1 || ship > 5)
            {
                
                AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                Question(ships, map, aimap);
            }
            //Leellenőrizzük, hogy a lehelyezni kívánt hajót lehelyeztük-e már
            else if (ships[ship - 1] == 0)
            {
                
                AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                Question(ships, map, aimap);
            }
            //Ha helyes a beírt adat, akkor meghívjuk a "Coordinates" függvényt, ezzel tovább haladva a bekérdezéssel
            else
            {
                Coordinates(ships, map, aimap, ship);
            }
        }

        //Lértehozzuk a "Coordinates" függvényt, a koordináták bekérdezésére és a hajóhosszúság ellenőrzésére
        public void Coordinates(int[] ships, int[,] map, int[,] aimap, int ship)
        {
            //Carrier = 5
            //Battleship = 4
            //Destroyer = 3
            //Submarine = 3
            //PatrolBoat = 2

            AnsiConsole.Write(new Markup("[cyan3]Add meg, a hajó kezdőponti és végponti koordinátáit!" + Environment.NewLine
                + "(Fontos hogy ELŐSZÖR A KEZDŐ koordinátát adjuk meg, UTÁNA AZ UTOLSÓ koordinátát!!!)[/]"));
            WriteLine(" ");
            ForegroundColor = ConsoleColor.White;

            //Bekérjük a két értéket, majd átkonvertáljuk amit kell int típussá és szétbontjuk a stringet "karakterekre", az az számokra
            string from = ReadLine();
            //Hasznájuk a már korábban létrehozott könyvtárat, hogy a betűket számmá alakítsuk
            int fromX = mapCharToIntDict[from[0]];
            string a = from.Replace(from[0], ' ');
            int fromY = Int32.Parse(a) - 1;
            string to = ReadLine();
            int toX = mapCharToIntDict[to[0]];
            string b = to.Replace(to[0], ' ');
            int toY = Int32.Parse(b) - 1;

            //A létrejött int változókat egy tömbbe rakjuk, hogy könnyen tudjuk használni a place függvényben is
            int[] coordinates = { fromX, fromY, toX, toY };

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
                        Place(ships, map, aimap, ship, coordinates);                        
                        AnsiConsole.Write(new Markup("[green1]Nice[/]"));
                    }
                    //Már ezt is leírtam fentebb.
                    //Hányszor mondjam még el?
                    else
                    {                        
                        AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                        Question(ships, map, aimap);
                    }
                }
                //Ez ugyan az, mint az előző csak fordítva, meg amúgyis leírtam már egyszer, ha nem tűnt volna fel.
                else if (fromY == toY)
                {
                    if (fromX - toX == 4 || fromX - toX == -4)
                    {
                        Place(ships, map, aimap, ship, coordinates);                        
                        AnsiConsole.Write(new Markup("[green1]Nice[/]"));
                    }
                    else
                    {                        
                        AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                        Question(ships, map, aimap);
                    }
                }
                else
                {                    
                    AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                    Question(ships, map, aimap);
                }
            }
            //Innentől csak ismételgetjük a fentit, minden hajótípusnál, a hajóhosszúságot megváltoztatva
            else if (ship == 2)
            {
                if (fromX == toX)
                {
                    if (fromY - toY == 3 || fromY - toY == -3)
                    {
                        Place(ships, map, aimap, ship, coordinates);                        
                        AnsiConsole.Write(new Markup("[green1]Nice[/]"));
                    }
                    else
                    {                        
                        AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                        Question(ships, map, aimap);
                    }
                }
                else if (fromY == toY)
                {
                    if (fromX - toX == 3 || fromX - toX == -3)
                    {
                        Place(ships, map, aimap, ship, coordinates);                        
                        AnsiConsole.Write(new Markup("[green1]Nice[/]"));
                    }
                    else
                    {                        
                        AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                        Question(ships, map, aimap);
                    }
                }
                else
                {                    
                    AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                    Question(ships, map, aimap);
                }
            }
            else if (ship == 3 || ship == 4)
            {
                if (fromX == toX)
                {
                    if (fromY - toY == 2 || fromY - toY == -2)
                    {
                        Place(ships, map, aimap, ship, coordinates);                        
                        AnsiConsole.Write(new Markup("[green1]Nice[/]"));
                    }
                    else
                    {                        
                        AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                        Question(ships, map, aimap);
                    }
                }
                else if (fromY == toY)
                {
                    if (fromX - toX == 2 || fromX - toX == -2)
                    {
                        Place(ships, map, aimap, ship, coordinates);                        
                        AnsiConsole.Write(new Markup("[green1]Nice[/]"));
                    }
                    else
                    {                        
                        AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                        Question(ships, map, aimap);
                    }
                }
                else
                {                    
                    AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                    Question(ships, map, aimap);
                }
            }
            else if (ship == 5)
            {
                if (fromX == toX)
                {
                    if (fromY - toY == 1 || fromY - toY == -1)
                    {
                        Place(ships, map, aimap, ship, coordinates);                        
                        AnsiConsole.Write(new Markup("[green1]Nice[/]"));
                    }
                    else
                    {                        
                        AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                        Question(ships, map, aimap);
                    }
                }
                else if (fromY == toY)
                {
                    if (fromX - toX == 1 || fromX - toX == -1)
                    {
                        Place(ships, map, aimap, ship, coordinates);                        
                        AnsiConsole.Write(new Markup("[green1]Nice[/]"));
                    }
                    else
                    {                        
                        AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                        Question(ships, map, aimap);
                    }
                }
                else
                {                    
                    AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                    Question(ships, map, aimap);
                }
            }
        }

//--------------------------------------------------------------------------------------------------------------------------------------
//Beni kódja:

        static void Ai_Random()
        {
            Random randInt = new Random();
            string chars = "ABCDEFGHIJ";

            for (int i = 0; i < 5; i++)
            {
                int vertical = randInt.Next(0, 1); //Ha 0 vízszintes, ha 1 függőleges
                int randomNum = randInt.Next(1, 11);
                char randomChar = chars[randInt.Next(1, 11)];

                int[] ships = { 5, 4, 3, 3, 2 };
                int ship_L = ships[randInt.Next(0, 4)]; //ship_L = ship_Lenght

                //From értékek: betű, szám - fromChar, fromNum
                //To értékek betű, szám - toChar, toNum

                if (vertical == 0)
                {
                    int fromNum = randomNum;
                    char fromChar = randomChar;

                    int toNum = randomNum;
                    if (10 - chars[(int)randomChar] > chars[(int)ship_L])
                    {
                        int toChar_i = chars[(int)randomChar] + chars[(int)ship_L];
                        char toChar = Convert.ToChar(toChar_i);
                    }
                    else
                    {
                        int toChar_i = chars[(int)randomChar] - chars[(int)ship_L];
                        char toChar = Convert.ToChar(toChar_i);
                    }
                }
                else
                {
                    int fromNum = randomNum;
                    char fromChar = randomChar;

                    char toChar = randomChar;
                    if (10 - randomNum > ship_L)
                    {
                        int toNum = randomNum + ship_L;
                    }
                    else
                    {
                        int toNum = randomNum - ship_L;
                    }
                }
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------
        //Gábor kódja:

        public void AIGenerate(int[,] map)
        {
            Random randInt = new Random();

            for (int i = 0; i < 5; i++)
            {
                bool isHorizontal = randInt.Next(0, 100) > 50;

                Console.WriteLine(shipSizes[i + 1]);

                bool cant = true;

                while (cant)
                {
                    if (isHorizontal)
                    {
                        int columnRand = randInt.Next(0, 10);
                        int rowRand = randInt.Next(0, 10 - shipSizes[i + 1]);

                        int fromX = rowRand;
                        int fromY = columnRand;

                        int toX = rowRand + shipSizes[i + 1];
                        int toY = columnRand;

                        if (this.CanPlace(map, [fromX, fromY, toX, toY], i + 1))
                        {
                            AI_Place(map, [fromX, fromY, toX, toY], i + 1);
                            cant = false;
                        }
                        else
                        {
                            Console.WriteLine("cannot");
                        }
                    }
                    else
                    {
                        int rowRand = randInt.Next(0, 10);
                        int columnRand = randInt.Next(0, 10 - shipSizes[i + 1]);

                        int fromX = rowRand;
                        int fromY = columnRand;

                        int toX = rowRand;
                        int toY = columnRand + shipSizes[i + 1];

                        if (this.CanPlace(map, [fromX, fromY, toX, toY], i + 1))
                        {
                            AI_Place(map, [fromX, fromY, toX, toY], i + 1);
                            cant = false;
                        }
                        else
                        {
                            Console.WriteLine("cannot");
                        }
                    }
                }
            }
        }

        public bool CanPlace(int[,] map, int[] coords, int shipType)
        {
            int fromX = coords[0];
            int fromY = coords[1];

            int toX = coords[2];
            int toY = coords[3];

            if (fromX == toX)
            {
                if (toY - fromY != shipSizes[shipType])
                    return false;

                for (int i = fromY; i <= toY; i++)
                    if (map[i, fromX] != 0)
                        return false;

                return true;
            }

            if (fromY == toY)
            {
                if (toX - fromX != shipSizes[shipType])
                    return false;

                for (int i = fromX; i <= toX; i++)
                    if (map[fromY, i] != 0)
                        return false;

                return true;
            }

            return false;
        }

        public void AI_Place(int[,] map, int[] coords, int shipType)
        {
            int fromX = coords[0];
            int fromY = coords[1];

            int toX = coords[2];
            int toY = coords[3];

            if (fromX == toX)
            {
                for (int i = fromY; i < toY; i++)
                    map[i, fromX] = 1;
            }

            if (fromY == toY)
            {
                for (int i = fromX; i < toX; i++)
                    map[fromY, i] = 1;
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------      

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

            int[,] AI_Map = new int[10, 10];

            //Ebben a tömbben tároljuk a hajótípusokat, abban a sorrendben, ahogy kiírtuk a konzolra a választásnál
            //(Carrier(1), BattleShip(2), Destroyer(3), Submarine(4), PatrolBoat(5)
            int[] Ships = { 1, 1, 1, 1, 1 };

            //A Torpedo osztályt "game"-ként "hozzuk" létre
            Torpedo game = new Torpedo();

            //Meghívjuk a "PrintMap" függvényt, ezzel kirajzolva a map-ot
            game.PrintMap(Map, AI_Map);

            game.AIGenerate(AI_Map);

            //Meghívjuk 5-ször a "Question" függvényt, ezzel elindítva a bekérdezést és a játékot is
            for (int i = 0; i < 5; i++)
            {
                game.Question(Ships, Map, AI_Map);
            }


            //Valszeg ideiglenes megoldás az utolsó lehelyezett hajó megtekintéséhez.
            game.PrintMap(Map, AI_Map);
        }
    }
}


//Bugok:
//Nem lehet a 10-es oszlopban lehelyezni hajókat ✓