using Spectre.Console;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
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
        public void PrintHeader()
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
                    else if (map[x, y] == -1)
                    {
                        AnsiConsole.Write(new Markup("[black][[~]][/]"));
                    }
                    else if (map[x, y] == -2)
                    {
                        AnsiConsole.Write(new Markup("[maroon][[*]][/]"));
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
                    else if (aimap[x, y] == -1)
                    {
                        AnsiConsole.Write(new Markup("[black][[~]][/]"));
                    }
                    else if (aimap[x, y] == -2)
                    {
                        AnsiConsole.Write(new Markup("[maroon][[*]][/]"));
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

            FriendlyShipsCoords.Add(fx + ";" + fy + ";" + tx + ";" + ty);

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
            //Várunk 1 másodpercet a kiírt szöveg törlésével, vagy várunk egy gomb megnyomására, majd töröljük a konzolt, és meghívjukk a "PrintMap" függvényt,
            //ezzel frissítve a térképet a lehelyezett hajókkal
            if (manual)
                ReadKey();
            else if (manual == false)
                Thread.Sleep(1000);
            Clear();
            PrintMap(map, aimap);


            AnsiConsole.Write(new Markup("[cyan3]Add meg a hajó sorszámát! (1. Repülőgép-hordozó(5), 2. Csatahajó(4), 3. Romboló(3), 4. Tengeralattjáró(3), 5. Járőrhajó(2) )[/]"));
            WriteLine(" ");
            ForegroundColor = ConsoleColor.White;

            try
            {
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
            catch
            {
                AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                Question(ships, map, aimap);
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

            bool bajvanmore = true;
            int fromX = 0;
            string a = "a";
            int fromY = 0;
            int toX = 0;
            int toY = 0;
            string b = "b";


            while (bajvanmore)
            {
                //Bekérjük a két értéket, majd átkonvertáljuk amit kell int típussá és szétbontjuk a stringet "karakterekre", az az számokra
                string from = ReadLine().ToUpper();
            
                if (from.Length! < 2 || from.Length! > 4)
                {
                    AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                }

                if (!chars.Contains(from[0]))
                {
                    AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                }

                fromX = mapCharToIntDict[from[0]];
                a = from.Remove(0, 1);

                try
                {
                    if (Int32.Parse(a.ToString()) > 10 || Int32.Parse(a.ToString()) < 1)
                    {
                        AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                    }
                }
                catch
                {
                    AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                }

                fromY = Int32.Parse(a.ToString()) - 1;

                //----------------------------------------------------------------------

                string to = ReadLine().ToUpper();

                if (from.Length! < 2 || from.Length! > 4)
                {
                    AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                }

                if (!chars.Contains(from[0]))
                {
                    AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                }

                toX = mapCharToIntDict[from[0]];
                b = to.Remove(0, 1);


                try
                {
                    if (Int32.Parse(b.ToString()) > 10 || Int32.Parse(b.ToString()) < 1)
                    {
                        AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                    }
                }
                catch
                {
                    AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                }

                toY = Int32.Parse(b) - 1;

                bajvanmore = false;
            }


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

        public void Shoot(int[,] map, int[,] aimap, int[] enemyships)
        {
            if (manual)
                ReadKey();
            else if (manual == false)
                Thread.Sleep(1000);
            Clear();
            PrintMap(map, aimap);

            AnsiConsole.Write(new Markup("[cyan3]Löveg betöltve! Adja meg a cél kordinátákat![/]"));
            WriteLine(" ");
            ForegroundColor = ConsoleColor.White;

            string target = ReadLine().ToUpper();


            if (target.Length !< 2 || target.Length !> 4)
            {
                AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                Shoot(map, aimap, enemyships);
                return;
            }
            
            if (!chars.Contains(target[0]))
            {
                AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                Shoot(map, aimap, enemyships);
            }

            int targetX = mapCharToIntDict[target[0]];
            string a = target.Remove(0, 1);

            try
            {
                if (Int32.Parse(a.ToString()) > 10 || Int32.Parse(a.ToString()) < 1)
                {
                    AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                    Shoot(map, aimap, enemyships);
                }
            }
            catch
            {
                AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                Shoot(map, aimap, enemyships);
            }

            int targetY = Int32.Parse(a.ToString()) - 1;

            bool shot = false;
            
            if (aimap[targetX, targetY] == -1 || aimap[targetX, targetY] == -2)
            {
                shot = true;
            }

           
            if (!shot)
            {
                if(aimap[targetX, targetY] == 1)
                {
                    aimap[targetX, targetY] = -2;
                    AnsiConsole.Write(new Markup("[maroon]Találat![/]"));
                    Sink(aimap, enemyships);
                }
                else
                {
                    aimap[targetX, targetY] = -1;
                    AnsiConsole.Write(new Markup("[grey58]A francba! Nincs találat![/]"));
                }
            }
            else
            {
                AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                Shoot(map, aimap, enemyships);
            }

        }


        public void Sink(int[,] aimap, int[] enemyships)
        {
            //fromX = [0]
            //fromY = [1]
            //toX = [2]
            //toY = [3]

            string[][] ships = new string[5][];

            ships[0] = EnemyShipsCoords[0].ToString().Split(';');
            ships[1] = EnemyShipsCoords[1].ToString().Split(';');
            ships[2] = EnemyShipsCoords[2].ToString().Split(';');
            ships[3] = EnemyShipsCoords[3].ToString().Split(';');
            ships[4] = EnemyShipsCoords[4].ToString().Split(';');


            int[][] intShips = new int[5][];

            for (int i = 0; i < ships.Length; i++)
            {
                int[] thisInt = new int[4];

                for (int j = 0; j < ships[i].Length; j++)
                {
                    thisInt[j] = Int32.Parse(ships[i][j]);
                }

                intShips[i] = thisInt;
            }

            int[] Carrier = intShips[0];
            int[] BattleShip = intShips[1];
            int[] Destroyer = intShips[2];
            int[] Submarine = intShips[3];
            int[] PatrolBoat = intShips[4];

            int Car = 0;
            int Bat = 0;
            int Des = 0;
            int Sub = 0;
            int Pat = 0;


                            //*

            if (!sinkc)
            {
                if (Carrier[0] == Carrier[2])
                {
                    for (int i = Carrier[1]; i <= Carrier[3]; i++)
                        if (aimap[i, Carrier[0]] == -2)
                            Car++;
                }
                else if (Carrier[1] == Carrier[3])
                {
                    for (int i = Carrier[0]; i <= Carrier[2]; i++)
                        if (aimap[Carrier[1], i] == -2)
                            Car++;
                }
            }

            if (!sinkb)
            {
                if (BattleShip[0] == BattleShip[2])
                {
                    for (int i = BattleShip[1]; i <= BattleShip[3]; i++)
                        if (aimap[i, BattleShip[0]] == -2)
                            Bat++;
                }
                else if (BattleShip[1] == BattleShip[3])
                {
                    for (int i = BattleShip[0]; i <= BattleShip[2]; i++)
                        if (aimap[BattleShip[1], i] == -2)
                            Bat++;
                }
            }

            if (!sinkd)
            {
                if (Destroyer[0] == Destroyer[2])
                {
                    for (int i = Destroyer[1]; i <= Destroyer[3]; i++)
                        if (aimap[i, Destroyer[0]] == -2)
                            Des++;
                }
                else if (Destroyer[1] == Destroyer[3])
                {
                    for (int i = Destroyer[0]; i <= Destroyer[2]; i++)
                        if (aimap[Destroyer[1], i] == -2)
                            Des++;
                }
            }

            if (!sinks)
            {
                if (Submarine[0] == Submarine[2])
                {
                    for (int i = Submarine[1]; i <= Submarine[3]; i++)
                        if (aimap[i, Submarine[0]] == -2)
                            Sub++;
                }
                else if (Submarine[1] == Submarine[3])
                {
                    for (int i = Submarine[0]; i <= Submarine[2]; i++)
                        if (aimap[Submarine[1], i] == -2)
                            Sub++;
                }
            }

            if (!sinkp)
            {
                if (PatrolBoat[0] == PatrolBoat[2])
                {
                    for (int i = PatrolBoat[1]; i <= PatrolBoat[3]; i++)
                        if (aimap[i, PatrolBoat[0]] == -2)
                            Pat++;
                }
                else if (PatrolBoat[1] == PatrolBoat[3])
                {
                    for (int i = PatrolBoat[0]; i <= PatrolBoat[2]; i++)
                        if (aimap[PatrolBoat[1], i] == -2)
                            Pat++;
                }
            }


            if (Car == 5)
            {
                AnsiConsole.Write(new Markup("[greenyellow]Az ellenség Repülőgép-hordozója elsüllyedt![/]"));
                sinkc = true;
                enemyships[0] = 0;
            }
           
            if (Bat == 4)
            {
                AnsiConsole.Write(new Markup("[greenyellow]Az ellenség Csatahajója elsüllyedt![/]"));
                sinkb = true;
                enemyships[1] = 0;
            }

            if (Des == 3)
            {
                AnsiConsole.Write(new Markup("[greenyellow]Az ellenség Rombolója elsüllyedt![/]"));
                sinkd = true;
                enemyships[2] = 0;
            }

            if (Sub == 3)
            {
                AnsiConsole.Write(new Markup("[greenyellow]Az ellenség Tengeralattjárója elsüllyedt![/]"));
                sinks = true;
                enemyships[3]= 0;
            }

            if (Pat == 2)
            {
                AnsiConsole.Write(new Markup("[greenyellow]Az ellenség Járőrhajója elsüllyedt![/]"));
                sinkp = true;
                enemyships[4] = 0;
            }

        }

        private bool didShootGoodBefore = false;
        private int[] lastGoodCoords = new int[2];

        private int tries = 0;

        public int[] AiMoveToNext(int[,] map) {
            Random rnd = new Random();

            int side = rnd.Next(0, 3);

            int shootRow = lastGoodCoords[0];
            int shootCol = lastGoodCoords[1];

            bool isRowLeftOccupied = false;
            bool isRowRightOccupied = false;

            bool isColLeftOccupied = false;
            bool isColRightOccupied = false;

            bool isOccupiedOnAllSides =
                isRowLeftOccupied &&
                isRowRightOccupied &&
                isColLeftOccupied &&
                isColRightOccupied;

            while (map[shootRow, shootCol] == -2 && !isOccupiedOnAllSides) {
                side = rnd.Next(0, 3);

                isOccupiedOnAllSides =
                    isRowLeftOccupied &&
                    isRowRightOccupied &&
                    isColLeftOccupied &&
                    isColRightOccupied;

                shootRow = lastGoodCoords[0];
                shootCol = lastGoodCoords[1];

                switch (side) {
                    case 0:
                        if (shootCol == 0) {
                            isColLeftOccupied = true;

                            continue;
                        }

                        shootCol--;

                        if (map[shootRow, shootCol] == -2) {
                            isColLeftOccupied = true;
                        }

                        break;
                    case 1:
                        if (shootCol == 9) {
                            isColRightOccupied = true;

                            continue;
                        }

                        shootCol++;

                        if (map[shootRow, shootCol] == -2) {
                            isColRightOccupied = true;
                        }

                        break;
                    case 2:
                        if (shootRow == 0) {
                            isRowLeftOccupied = true;

                            continue;
                        }

                        shootRow--;

                        if (map[shootRow, shootCol] == -2) {
                            isRowLeftOccupied = true;
                        }

                        break;
                    case 3:
                        if (shootRow == 9) {
                            isRowRightOccupied = true;

                            continue;
                        }

                        shootRow++;

                        if (map[shootRow, shootCol] == -2) {
                            isRowRightOccupied = true;
                        }

                        break;
                }
            }

            if (isOccupiedOnAllSides) {
                shootRow = -1;
                shootCol = -1;

                didShootGoodBefore = false;
            }

            return new [] { shootRow, shootCol };
        }

        public void AI_Shoot(int[,] map, int[,] aimap, int[] friendlyships)
        {
            if (manual)
                ReadKey();
            else if (manual == false)
                Thread.Sleep(1000);
            Clear();
            PrintMap(map, aimap);

            Random rnd = new Random();

            //Első random kulcs - értékek létreholzása
            int shootRow = rnd.Next(0, 10);
            int shootCol = rnd.Next(0, 10);

            if (didShootGoodBefore) {
                tries = 0;

                int[] aiii = AiMoveToNext(map);

                if (aiii[0] != -1) {
                    shootRow = aiii[0];
                    shootCol = aiii[1];
                }
            }

            //Ha már létezik egy elem a szótárban akkor a következő random
            //elemet nem adjuk hozzá, ha még nem létezik akkor hozzáadjuk
            while (map[shootRow,shootCol] < 0)
            {
                //Random kulcs - érték pár létrehozása addig, amíg nem hozunk létre egy olyat ahova még nem lőttünk.
                shootRow = rnd.Next(0, 10);
                shootCol = rnd.Next(0, 10);
            }

            if (map[shootRow, shootCol] == 1) {
                map[shootRow, shootCol] = -2;

                lastGoodCoords[0] = shootRow;
                lastGoodCoords[1] = shootCol;

                didShootGoodBefore = true;

                AnsiConsole.Write(new Markup("[maroon]Az ellenség eltalálta az egyik hajódat![/]"));

                AI_Sink(map, friendlyships);
            }
            else
            {
                map[shootRow, shootCol] = -1;
                AnsiConsole.Write(new Markup("[grey58]Az ellenség lövése nem talált![/]"));
            }
        }

        public void AI_Sink(int[,] aimap, int[] friendlyships)
        {
            //fromX = [0]
            //fromY = [1]
            //toX = [2]
            //toY = [3]

            string[][] ships = new string[5][];

            ships[0] = FriendlyShipsCoords[0].ToString().Split(';');
            ships[1] = FriendlyShipsCoords[1].ToString().Split(';');
            ships[2] = FriendlyShipsCoords[2].ToString().Split(';');
            ships[3] = FriendlyShipsCoords[3].ToString().Split(';');
            ships[4] = FriendlyShipsCoords[4].ToString().Split(';');

            int[][] intShips = new int[5][];

            for (int i = 0; i < ships.Length; i++)
            {
                int[] thisInt = new int[4];

                for (int j = 0; j < ships[i].Length; j++)
                {
                    thisInt[j] = Int32.Parse(ships[i][j]);
                }

                intShips[i] = thisInt;
            }

            int[] Carrier = intShips[0];
            int[] BattleShip = intShips[1];
            int[] Destroyer = intShips[2];
            int[] Submarine = intShips[3];
            int[] PatrolBoat = intShips[4];

            int AICar = 0;
            int AIBat = 0;
            int AIDes = 0;
            int AISub = 0;
            int AIPat = 0;

                            //*

            if (!sinkc)
            {
                if (Carrier[0] == Carrier[2])
                {
                    for (int i = Carrier[1]; i <= Carrier[3]; i++)
                        if (aimap[i, Carrier[0]] == -2)
                            AICar++;
                }
                else if (Carrier[1] == Carrier[3])
                {
                    for (int i = Carrier[0]; i <= Carrier[2]; i++)
                        if (aimap[Carrier[1], i] == -2)
                            AICar++;
                }
            }

            if (!sinkb)
            {
                if (BattleShip[0] == BattleShip[2])
                {
                    for (int i = BattleShip[1]; i <= BattleShip[3]; i++)
                        if (aimap[i, BattleShip[0]] == -2)
                            AIBat++;
                }
                else if (BattleShip[1] == BattleShip[3])
                {
                    for (int i = BattleShip[0]; i <= BattleShip[2]; i++)
                        if (aimap[BattleShip[1], i] == -2)
                            AIBat++;
                }
            }

            if (!sinkd)
            {
                if (Destroyer[0] == Destroyer[2])
                {
                    for (int i = Destroyer[1]; i <= Destroyer[3]; i++)
                        if (aimap[i, Destroyer[0]] == -2)
                            AIDes++;
                }
                else if (Destroyer[1] == Destroyer[3])
                {
                    for (int i = Destroyer[0]; i <= Destroyer[2]; i++)
                        if (aimap[Destroyer[1], i] == -2)
                            AIDes++;
                }
            }

            if (!sinks)
            {
                if (Submarine[0] == Submarine[2])
                {
                    for (int i = Submarine[1]; i <= Submarine[3]; i++)
                        if (aimap[i, Submarine[0]] == -2)
                            AISub++;
                }
                else if (Submarine[1] == Submarine[3])
                {
                    for (int i = Submarine[0]; i <= Submarine[2]; i++)
                        if (aimap[Submarine[1], i] == -2)
                            AISub++;
                }
            }

            if (!sinkp)
            {
                if (PatrolBoat[0] == PatrolBoat[2])
                {
                    for (int i = PatrolBoat[1]; i <= PatrolBoat[3]; i++)
                        if (aimap[i, PatrolBoat[0]] == -2)
                            AIPat++;
                }
                else if (PatrolBoat[1] == PatrolBoat[3])
                {
                    for (int i = PatrolBoat[0]; i <= PatrolBoat[2]; i++)
                        if (aimap[PatrolBoat[1], i] == -2)
                            AIPat++;
                }
            }


            if (AICar == 5)
            {
                AnsiConsole.Write(new Markup("[greenyellow]A Repülőgép-hordozód elsüllyedt![/]"));
                aisinkc = true;
                friendlyships[0] = 0;
            }

            if (AIBat == 4)
            {
                AnsiConsole.Write(new Markup("[greenyellow]A Csatahajód elsüllyedt![/]"));
                aisinkb = true;
                friendlyships[1] = 0;
            }

            if (AIDes == 3)
            {
                AnsiConsole.Write(new Markup("[greenyellow]A Rombolód elsüllyedt![/]"));
                aisinkd = true;
                friendlyships[2] = 0;
            }

            if (AISub == 3)
            {
                AnsiConsole.Write(new Markup("[greenyellow]A Tengeralattjáród elsüllyedt![/]"));
                aisinks = true;
                friendlyships[3] = 0;
            }

            if (AIPat == 2)
            {
                AnsiConsole.Write(new Markup("[greenyellow]A Járőrhajód elsüllyedt![/]"));
                aisinkp = true;
                friendlyships[4] = 0;
            }

        }


        //--------------------------------------------------------------------------------------------------------------------------------------
        //Beni kódja:

        public void Ai_Random()
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

        //static void Main(string[] args)
        //{
        //    //Szemléltetés / szimuláció: feltételezzük, hogy a következő lépést a gép végzi.
        //    //Tehát a függvény csak addig fut le amíg ez az érték igaz.
        //    //Ez az érték akkor lehet false, ha a játékosé a következő lépés.
        //    bool nextStep = true;
        //}
        public Dictionary<char, int> GenerateEnemyShoot(bool nextStap)
        {

            //Szótár létrehozása az ellenfél lövésének
            Dictionary<char, int> enemyShoots = new Dictionary<char, int>();

            if (nextStap == true)
            {
                Random rnd = new Random();

                //Első random kulcs - értékek létreholzása
                char shootChar = (char)('A' + rnd.Next(10));
                int shootNum = rnd.Next(1, 11);

                //Random értékek hozzáadása a szótárhoz
                enemyShoots.Add(shootChar, shootNum);

                //Vizsgálat benne van e a kulcs - érték pár a szótárban
                bool canaddChar = enemyShoots.ContainsKey(shootChar);
                bool canaddNum = enemyShoots.ContainsValue(shootNum);

                //Ha már létezik egy elem a szótárban akkor a következő random
                //elemet nem adjuk hozzá, ha még nem létezik akkor hozzáadjuk
                while (canaddChar && canaddNum != true)
                {

                    //Random kulcs - érték pár létrehozása addig, amíg nem bírjuk hozzáadni az előzőt.
                    shootChar = (char)('A' + rnd.Next(10));
                    shootNum = rnd.Next(1, 11);

                    //Random értékek hozzáadása a szótárhoz
                    enemyShoots.Add(shootChar, shootNum);

                    //Vizsgálat benne van e a kulcs - érték pár a szótárban
                    canaddChar = enemyShoots.ContainsKey(shootChar);
                    canaddNum = enemyShoots.ContainsValue(shootNum);
                }

            }
            else
            {
                Console.WriteLine("A játékos következik!");
            }
            //Szótár a függvény visszatérése!!
            return enemyShoots;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------
        //Gábor kódja:

        public void AIGenerate(int[,] aimap)
        {
            Random randInt = new Random();

            for (int i = 0; i < 5; i++)
            {
                bool isHorizontal = randInt.Next(0, 100) > 50;

                //Console.WriteLine(shipSizes[i + 1]);

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

                        if (this.CanPlace(aimap, [fromX, fromY, toX, toY], i + 1))
                        {
                            AI_Place(aimap, [fromX, fromY, toX, toY], i + 1);
                            cant = false;
                        }
                        else
                        {
                            //Console.WriteLine("cannot");
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

                        if (this.CanPlace(aimap, [fromX, fromY, toX, toY], i + 1))
                        {
                            AI_Place(aimap, [fromX, fromY, toX, toY], i + 1);
                            cant = false;
                        }
                        else
                        {
                            //Console.WriteLine("cannot");
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

        public void AI_Place(int[,] aimap, int[] coords, int shipType)
        {
            int fromX = coords[0];
            int fromY = coords[1];

            int toX = coords[2];
            int toY = coords[3];

            if (selfAI)
                FriendlyShipsCoords.Add(fromX + ";" + fromY + ";" + toX + ";" + toY);
            else
                EnemyShipsCoords.Add(fromX + ";" + fromY + ";" + toX + ";" + toY);

            if (fromX == toX)
            {
                for (int i = fromY; i < toY; i++)
                    aimap[i, fromX] = 1;
            }

            if (fromY == toY)
            {
                for (int i = fromX; i < toX; i++)
                    aimap[fromY, i] = 1;
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------      

        ArrayList EnemyShipsCoords = new ArrayList();
        ArrayList FriendlyShipsCoords = new ArrayList();

        bool sinkc = false;
        bool sinkb = false;
        bool sinkd = false;
        bool sinks = false;
        bool sinkp = false;

        bool aisinkc = false;
        bool aisinkb = false;
        bool aisinkd = false;
        bool aisinks = false;
        bool aisinkp = false;

        bool selfAI = false;
        bool manual = false;
        char[] chars = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };

        public void Menu(int[] ships, int[,] map, int[,] aimap)
        {
            Thread.Sleep(500);
            Clear();
            PrintMap(map, aimap);

            AnsiConsole.Write(new Markup("[cyan3]Ha azt szeretnéd hogy 1 másodperc múlva eltűnjenek az 'értesítések' a képernyőről nyomd meg az 1-et." + Environment.NewLine
                + "Ha azt szeretnéd, hogy csak akkor tűnjenek el, ha megnyomsz egy gombot, nyomd meg a 2-t.[/]"));
            WriteLine(" ");
            ForegroundColor = ConsoleColor.White;

            string szöveg = ReadLine();
            int gyász = Int32.Parse(szöveg.ToString());

            if (gyász == 1)
            {
                manual = false;
            }
            else if (gyász == 2)
            {
                manual = true;
            }
            else
            {
                AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                Menu(ships, map, aimap);
            }

            AnsiConsole.Write(new Markup("[cyan3]Ha te szeretnéd lehelyezni a hajóidat, akkor írj be egy 1-et." + Environment.NewLine
                + "Ha véletlenszerű lehelyezést szeretnél, írj be egy 2-t.[/]"));
            WriteLine(" ");
            ForegroundColor = ConsoleColor.White;

            string lehelyezés = ReadLine();
            int rák = Int32.Parse(lehelyezés.ToString());

            if (rák == 1)
            {
                //Meghívjuk 5-ször a "Question" függvényt, ezzel elindítva a bekérdezést és a játékot is
                for (int i = 0; i < 5; i++)
                {
                    Question(ships, map, aimap);
                }
            }
            else if (rák == 2)
            {
                selfAI = true;
                AIGenerate(map);
            }
            else
            {
                AnsiConsole.Write(new Markup("[red1]Balfasz[/]"));
                Menu(ships, map, aimap);
            }
        }
    }

    //Végre jön a játék meghívása
    class Program
    {
        static void Main(string[] args)
        {

            bool run = true;

            //Átrakjuk a konzol megjelenítését UTF-8-ra, hogy látszódjanak a hosszú magánhangzók, ha már ekkora gyász nyelv ez a magyar.
            Console.OutputEncoding = Encoding.UTF8;

            //Létre hozzuk a már sokat emlegetett Map mátrixot (vagy mit xd)
            int[,] Map = new int[10, 10];

            int[,] AI_Map = new int[10, 10];

            //Ebben a tömbben tároljuk a hajótípusokat, abban a sorrendben, ahogy kiírtuk a konzolra a választásnál
            //(Carrier(1), BattleShip(2), Destroyer(3), Submarine(4), PatrolBoat(5)
            int[] EnemyShips = { 1, 1, 1, 1, 1 };
            int[] FriendlyShips = { 1, 1, 1, 1, 1 };

            //A Torpedo osztályt "game"-ként "hozzuk" létre
            Torpedo game = new Torpedo();

            //Meghívjuk a "PrintMap" függvényt, ezzel kirajzolva a map-ot
            game.PrintMap(Map, AI_Map);

            game.AIGenerate(AI_Map);

            game.Menu(EnemyShips, Map, AI_Map);

            while (run)
            {
                game.Shoot(Map, AI_Map, EnemyShips);
                if (EnemyShips[0] == 0 && EnemyShips[1] == 0 && EnemyShips[2] == 0 && EnemyShips[3] == 0 && EnemyShips[4] == 0)
                {
                    run = false;
                    Clear();
                    game.PrintMap(Map, AI_Map);
                    AnsiConsole.Write(new Markup("[green1]Sikeresen elsüllyeszted az ellenség összes hajóját, ezzel megnyerve a csatát![/]"));
                }
                if (!run)
                {
                    WriteLine(" ");
                    AnsiConsole.Write(new Markup("[cyan3]Szeretnél még egyet játszani? (I/N)[/]"));
                    WriteLine(" ");
                    ForegroundColor = ConsoleColor.White;
                    string yes = ReadLine();
                    if (yes == "I" || yes == "i")
                    {
                        run = true;
                        AnsiConsole.Write(new Markup("[cyan3]Akkor kezdődjön az új csata![/]"));
                        Thread.Sleep(2000);
                        Clear();
                        for (int x = 0; x < Map.GetLength(0); x++)
                        {
                            for (int y = 0; y < Map.GetLength(1); y++)
                            {
                                Map[x, y] = 0;
                            }
                        }
                        for (int x = 0; x < AI_Map.GetLength(0); x++)
                        {
                            for (int y = 0; y < AI_Map.GetLength(1); y++)
                            {
                                AI_Map[x, y] = 0;
                            }
                        }
                        game.PrintMap(Map, AI_Map);
                        game.AIGenerate(AI_Map);
                        game.Menu(EnemyShips, Map, AI_Map);
                    }
                    else if (yes == "N" || yes == "n")
                    {
                        Clear();
                        AnsiConsole.Write(new Markup("[cyan3]Reméljük hamar viszont látunk![/]"));
                        Environment.Exit(0);
                    }
                }
                else
                {
                    game.AI_Shoot(Map, AI_Map, FriendlyShips);
                    if (FriendlyShips[0] == 0 && FriendlyShips[1] == 0 && FriendlyShips[2] == 0 && FriendlyShips[3] == 0 && FriendlyShips[4] == 0)
                    {
                        run = false;
                        Clear();
                        game.PrintMap(Map, AI_Map);
                        AnsiConsole.Write(new Markup("[red1]Sajnos az ellenség elsüllyesztette az összes hajódat, ezzel elvesztetted a csatát![/]"));
                    }
                    if (!run)
                    {
                        WriteLine(" ");
                        AnsiConsole.Write(new Markup("[cyan3]Szeretnél még egyet játszani? (I/N)[/]"));
                        WriteLine(" ");
                        ForegroundColor = ConsoleColor.White;
                        string yes = ReadLine();
                        if (yes == "I" || yes == "i")
                        {
                            run = true;
                            AnsiConsole.Write(new Markup("[cyan3]Akkor kezdődjön az új csata![/]"));
                            Thread.Sleep(2000);
                            Clear();
                            for (int x = 0; x < Map.GetLength(0); x++)
                            {
                                for (int y = 0; y < Map.GetLength(1); y++)
                                {
                                    Map[x, y] = 0;
                                }
                            }
                            for (int x = 0; x < AI_Map.GetLength(0); x++)
                            {
                                for (int y = 0; y < AI_Map.GetLength(1); y++)
                                {
                                    AI_Map[x, y] = 0;
                                }
                            }
                            game.PrintMap(Map, AI_Map);
                            game.AIGenerate(AI_Map);
                            game.Menu(EnemyShips, Map, AI_Map);
                        }
                        else if (yes == "N" || yes == "n")
                        {
                            Clear();
                            AnsiConsole.Write(new Markup("[cyan3]Reméljük hamar viszont látunk![/]"));
                            Environment.Exit(0);
                        }
                    }
                }
            }
        }
    }
}


//Bugok:
//Nem lehet a 10-es oszlopban lehelyezni hajókat ✓
//Bugos a Sink