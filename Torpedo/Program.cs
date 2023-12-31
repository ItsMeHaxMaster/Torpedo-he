﻿using Spectre.Console;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using static System.Console;

namespace Torpedo
{
    class Torpedo
    {

        //Könyvtárak létrehozása a számok és betűk átkonvertálásához + Gábor kódjához

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

        // A térkép fejlécének kiírása (az oszlopok számai és az ikonok a bal felső sarokban)
        public void PrintHeader()
        {

            //Bal felül lévő ikon
            AnsiConsole.Write(new Markup("[blue1]⚓ [/]"));
            //Számok 1-től 10-ig []-ben
            for (int i = 1; i < 11; i++)
                //Ez az AnsiConsole egy plugin, amit mi most arra használtunk,
                //hogy "szebbekk" legyenek a színek a console-on, de rengeteg mindenre lehet haszálni
                AnsiConsole.Write(new Markup("[gold3_1][[" + i + "]][/]"));
            Console.Write("                         ");
            AnsiConsole.Write(new Markup("[red1]💢 [/]"));
            for (int i = 1; i < 11; i++)
                AnsiConsole.Write(new Markup("[gold3_1][[" + i + "]][/]"));
        }

        //A map kiírása
        public void PrintMap(int[,] map, int[,] aimap, ref int Win, ref int Lose)
        {

            //Itt tudjuk megjeleníteni az ellenség hajóit ha kell a teszteléshez
            bool visible = true;

            //Meghívjuk a Header függvényt, hogy az is ki legyen írva
            PrintHeader();
            WriteLine();
            //Létrehozzuk a row1 és row2 character változókat
            //a map sorainak sorszámozásához betűkkel. (A, B, C, D.....)
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
                        //Hozzáadunk egyet a row1-hez, hogy tovább lépjünk az ABC betűin
                        //('A' + row1++ -> 'B')
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
                    //Ha olyan mezőre lőttunk, ahol nem volt semmi
                    //feketére változtatjuk a hullámot, ezzel eltüntetve azt
                    else if (map[x, y] == -1)
                    {
                        AnsiConsole.Write(new Markup("[black][[~]][/]"));
                    }
                    //Ha olyan mezőre lőttunk, ahol egy hajó volt
                    //akkor egy piros "robbanás"-ra (azaz csillagra) változtatjuk a hullámot
                    else if (map[x, y] == -2)
                    {
                        AnsiConsole.Write(new Markup("[maroon][[*]][/]"));
                    }

                    //Kiírjuk a hajóinknak a státuszát, azaz elsüllyedt-e vagy sem
                    PrintStats(x, y);

                }

                for (int y = 0; y < aimap.GetLength(1); y++)
                {
                    //A sor 0. elemének a helyére beírjuk a már említett sorszámozást
                    if (y == 0)
                    {
                        //Ha olyan soron vagyunk ahol van kiírva statisztika,
                        //nem írunk "space"-eket, ezzel "egybe tartva" a map-ot
                        if (x>0 && x<7)
                        {                            
                            AnsiConsole.Write(new Markup("[gold3_1][[" + row2 + "]][/]"));
                            row2++;
                        }
                        else
                        {
                            Console.Write("                          ");
                            AnsiConsole.Write(new Markup("[gold3_1][[" + row2 + "]][/]"));
                            row2++;
                        }
                    }
                    //Innentúl ugyanaz a séma, csak az AIMap-on
                    if (aimap[x, y] == 1)
                    {
                        //Ha szeretnénk látni az ellenfél hajóit, kirajzoljuk azokat,
                        //ha nem, akkor nem
                        if (visible)
                        {
                            AnsiConsole.Write(new Markup("[red][[X]][/]"));
                        }
                        else
                        {
                            AnsiConsole.Write(new Markup("[navy][[~]][/]"));
                        }
                    }
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

                    //Kiírjuk az ellenfél hajóinak a statisztikáját
                    PrintAIStats(x, y);                    
                }
                
                Console.WriteLine();
            }

            //Kiírjuk, hogy hányszor vesztettünk/nyertünk
            PrintWL(Win, Lose);
        }

        //A Win-ek és Lose-ok kiírása
        public void PrintWL(int Win, int Lose)
        {
            WriteLine();
            AnsiConsole.Write(new Markup("[gold1]🥇Wins: [/]" + Win));
            WriteLine();
            AnsiConsole.Write(new Markup("[maroon]💥Losses: [/]" + Lose));
            WriteLine();

            float winratio = float.Parse(Win.ToString()) / (float.Parse(Win.ToString()) + float.Parse(Lose.ToString())) * 100;

            if (Win == 0 && Lose == 0)
            {
                AnsiConsole.Write(new Markup("[gray58]⚔️Win ratio: Nincs adat[/]"));
            }
            else if (winratio <= 100 && winratio >= 90)
            {
                AnsiConsole.Write(new Markup("[green1]⚔️Win ratio: " + winratio + " %[/]"));
            }
            else if (winratio <= 89 && winratio >= 75)
            {
                AnsiConsole.Write(new Markup("[green3]⚔️Win ratio: " + winratio + " %[/]"));
            }
            else if (winratio <= 74 && winratio >= 50)
            {
                AnsiConsole.Write(new Markup("[darkorange]⚔️Win ratio: " + winratio + " %[/]"));
            }
            else if (winratio <= 49 && winratio >= 25)
            {
                AnsiConsole.Write(new Markup("[red1]⚔️Win ratio: " + winratio + " %[/]"));
            }
            else if (winratio <= 24)
            {
                AnsiConsole.Write(new Markup("[darkred_1]⚔️Win ratio: " + winratio + " %[/]"));
            }

            WriteLine();
            WriteLine();
        }

        //A saját hajóink statisztikáinak a kiírása (elsüllyedt-e vagy sem)
        public void PrintStats(int x, int y)
        {
            //Megvizsgáljuk hogy a jó sorban vagyunk-e, ha igen akkor kiírjuk, amit ki akarunk
            //(Ezt a továbbiakban nem fogom leírni, minden if-nél)
            if (x == 1 && y == 9)
            {
                AnsiConsole.Write(new Markup("[white] A hajóid:                [/]"));
            }


            if (x == 2 && y == 9)
            {
                //Ha a kiírni kívánt hajó elsüllyedt, akkor pirossal írjuk ki,
                //ha nem, akkor zölddel
                //Ugyanez érvényes az összes többire is
                if (FriendlySinkedC == true)
                {
                    //Kiírjuk a hajótípust, a hosszuságát és utána "space"-eket teszünk, hogy "ne csússzon szét" a map
                    AnsiConsole.Write(new Markup("[maroon] Repülőgép-hordozó [[5]]    [/]"));
                }
                else
                {
                    AnsiConsole.Write(new Markup("[green3_1] Repülőgép-hordozó [[5]]    [/]"));
                }
            }

            if (x == 3 && y == 9)
            {
                if (FriendlySinkedB)
                {
                    AnsiConsole.Write(new Markup("[maroon] Csatahajó [[4]]            [/]"));
                }
                else
                {
                    AnsiConsole.Write(new Markup("[green3_1] Csatahajó [[4]]            [/]"));
                }
            }

            if (x == 4 && y == 9)
            {

                if (FriendlySinkedD)
                {
                    AnsiConsole.Write(new Markup("[maroon] Romboló [[3]]              [/]"));
                }
                else
                {
                    AnsiConsole.Write(new Markup("[green3_1] Romboló [[3]]              [/]"));
                }
            }

            if (x == 5 && y == 9)
            {

                if (FriendlySinkedS)
                {
                    AnsiConsole.Write(new Markup("[maroon] Tengeralattjáró [[3]]      [/]"));
                }
                else
                {
                    AnsiConsole.Write(new Markup("[green3_1] Tengeralattjáró [[3]]      [/]"));
                }
            }

            if (x == 6 && y == 9)
            {

                if (FriendlySinkedP)
                {
                    AnsiConsole.Write(new Markup("[maroon] Járőrhajó [[2]]            [/]"));
                }
                else
                {
                    AnsiConsole.Write(new Markup("[green3_1] Járőrhajó [[2]]            [/]"));
                }

            }
        }

        //Az ellenfél statisztikáinak a kiírása
        public void PrintAIStats(int x, int y)
        {
            //Ugyanaz, mint az előbb, csak mások a szövegek és már nem kell "space"-eket írnunk
            if (x == 1 && y == 9)
            {
                AnsiConsole.Write(new Markup("[white] Az ellenség hajói:[/]"));
            }


            if (x == 2 && y == 9)
            {
                if (EnemySinkedC == true)
                {
                    AnsiConsole.Write(new Markup("[maroon] Repülőgép-hordozó [[5]][/]"));
                }
                else
                {
                    AnsiConsole.Write(new Markup("[green3_1] Repülőgép-hordozó [[5]][/]"));
                }
            }

            if (x == 3 && y == 9)
            {
                if (EnemySinkedB)
                {
                    AnsiConsole.Write(new Markup("[maroon] Csatahajó [[4]][/]"));
                }
                else
                {
                    AnsiConsole.Write(new Markup("[green3_1] Csatahajó [[4]][/]"));
                }
            }

            if (x == 4 && y == 9)
            {

                if (EnemySinkedD)
                {
                    AnsiConsole.Write(new Markup("[maroon] Romboló [[3]][/]"));
                }
                else
                {
                    AnsiConsole.Write(new Markup("[green3_1] Romboló [[3]][/]"));
                }
            }

            if (x == 5 && y == 9)
            {

                if (EnemySinkedS)
                {
                    AnsiConsole.Write(new Markup("[maroon] Tengeralattjáró [[3]][/]"));
                }
                else
                {
                    AnsiConsole.Write(new Markup("[green3_1] Tengeralattjáró [[3]][/]"));
                }
            }

            if (x == 6 && y == 9)
            {

                if (EnemySinkedP)
                {
                    AnsiConsole.Write(new Markup("[maroon] Járőrhajó [[2]][/]"));
                }
                else
                {
                    AnsiConsole.Write(new Markup("[green3_1] Járőrhajó [[2]][/]"));
                }

            }
        }

        //Ebben megnézzük, hogy letudja-e tenni a játékos oda a hajót ahova akarta,
        //ha letudja, akkor a mátrixban is "lerakjuk"
        public void Place(int[] ships, int[,] map, int[,] aimap, int ship, int[] coordinates, int Win, int Lose)
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

            //Megvizsgáljuk, hogy van-e még olyan hajó amit le akar helyezni a player 
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
                        AnsiConsole.Write(new Markup("[red1]Rossz a beírt koordináta![/]"));
                        Question(ships, map, aimap, Win, Lose);
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
                        AnsiConsole.Write(new Markup("[red1]Rossz a beírt koordináta![/]"));
                        Question(ships, map, aimap, Win, Lose);
                    }
                }
                //Ha sikeresen lefutott a fenti kódsor, akkor kivonunk a "Ships" tömb jelenlegi eleméből 1-et,
                //ezzzel nullára csökkentve annak értékét, tehát már lehelyeztük azt a hajó típust
                ships[ship - 1]--;
            }
            else
            {
                AnsiConsole.Write(new Markup("[red1]Nincs több ilyen hajótípusod![/]"));
                Question(ships, map, aimap, Win, Lose);
            }
        }

        //Létrehozzunk egy függvényt a lehelyezni kívánt hajó típusának bekérdezésére.
        //Erre azért van szükség, mert így nem jön létre egy bug amit tapasztaltunk,
        //így nem a "Coordinates" függvényt kell meghívnunk, ha rossz a bekért adat,
        //és nem akad össze a már futó "Coordinates" függvénnyel.
        public void Question(int[] ships, int[,] map, int[,] aimap, int Win, int Lose)
        {
            //Várunk 1 másodpercet a kiírt szöveg törlésével,
            //vagy várunk egy gomb megnyomására, ha azt választotta a felhasználó,
            //majd töröljük a konzolt, és meghívjukk a "PrintMap" függvényt,
            //ezzel frissítve a térképet a lehelyezett hajókkal
            if (manual)
                ReadKey();
            else if (manual == false)
                Thread.Sleep(sleep);
            Clear();
            PrintMap(map, aimap, ref Win, ref Lose);


            AnsiConsole.Write(new Markup("[cyan3]Add meg a hajó sorszámát! (1. Repülőgép-hordozó(5), " +
                "2. Csatahajó(4), 3. Romboló(3), 4. Tengeralattjáró(3), 5. Járőrhajó(2) )[/]"));
            WriteLine();
            //Módosítjuk a ConsoleColor-t fehérre,
            //hogy a játékos által beírt szöveg fehér legyen, így "jobban" fog kinézni a játék
            ForegroundColor = ConsoleColor.White;

            //Ez a try catch azért kell, hogy ne crasheljen a játék,
            //ha rosszul adjuk meg a bekért értéket.
            try
            {
                //"ship" változóként mentjük a bekért értéket
                int ship = Int32.Parse(ReadLine());
                answer = ship;

                //Leellenőrizzük, hogy hibás-e a megadott érték, ha az,
                //akkor újra meghívjuk a függvényt, ezzel errőlről kezdve az egészet
                if (ship < 1 || ship > 5)
                {
                    AnsiConsole.Write(new Markup("[red1]Nem létezik ilyen hajó![/]"));
                    Question(ships, map, aimap, Win, Lose);
                }
                //Leellenőrizzük, hogy a lehelyezni kívánt hajót lehelyeztük-e már,
                //ha igen újra meghívjuk a függvényt
                else if (ships[ship - 1] == 0)
                {
                    AnsiConsole.Write(new Markup("[red1]Már lehelyeztük![/]"));
                    Question(ships, map, aimap, Win, Lose);
                }
                //Ha helyes a beírt adat, akkor meghívjuk a "Coordinates" függvényt,
                //ezzel tovább haladva a bekérdezéssel
                else
                {
                    Coordinates(ships, map, aimap, ship, Win, Lose);
                }
            }
            //Ha rosszul adjuk meg a bekért értéket (pl. asdadsadas),
            //akkor újra meghívjuk a függvényt
            catch
            {
                AnsiConsole.Write(new Markup("[red1]Biztos jól adtad meg?[/]"));
                Question(ships, map, aimap, Win, Lose);
            }

        }

        //Lértehozzuk a "Coordinates" függvényt,
        //a koordináták bekérdezésére és a hajóhosszúság ellenőrzésére
        public void Coordinates(int[] ships, int[,] map, int[,] aimap, int ship, int Win, int Lose)
        {
            //Carrier = 5
            //Battleship = 4
            //Destroyer = 3
            //Submarine = 3
            //PatrolBoat = 2

            AnsiConsole.Write(new Markup("[cyan3]Add meg, a hajó kezdőponti és végponti koordinátáit!" + Environment.NewLine
                + "(Fontos hogy ELŐSZÖR A KEZDŐ koordinátát adjuk meg, UTÁNA AZ UTOLSÓ koordinátát!!!)[/]"));
            WriteLine();
            ForegroundColor = ConsoleColor.White;

            //Létrehozzuk előre azokat a változókat amiket használni fogunk,
            //mivel, ha a "while"-on belül hoznánk létre, akkor a "while"-on
            //kívül nem tudnánk használni őket
            int fromX = 0;
            string a = "a";
            int fromY = 0;
            int toX = 0;
            int toY = 0;
            string b = "b";

            //Amíg ez true, addig folyamatosan kéri be a választ, ha rosszul adnánk be
            bool bajvanmore = true;
            bool bajvanmore2 = true;
            bool bajvanmore3 = true;

            //Ez a ciklus arra van, hogyha rosszul adnánk meg a bekért értéket (pl. aaaa), ne crasheljen be a játék
            while (bajvanmore)
            {
                while (bajvanmore2)
                {
                    try
                    {
                        //Bekérjük az első értéket, amit nagybetűvé con cvertálunk
                        string from = ReadLine();
                        answer2 = from;
                        from = from.ToUpper();

                        //Ezekkel az "if"-ekkel ellenőrizzük, hogy jól írtuk-e be a bekért értéket

                        //Megnézzük a hosszúságát, ezzel kizárva egy lehetséges hibát
                        if (from.Length! < 2 || from.Length! > 4)
                        {
                            AnsiConsole.Write(new Markup("[red1]Helytelen érték![/]"));
                        }
                        //Ezután megnézzük, hogy jó betűt adott-e meg (A és J között)
                        else if (!chars.Contains(from[0]))
                        {
                            AnsiConsole.Write(new Markup("[red1]Helytelen érték![/]"));
                        }

                        //Ha jól adtuk meg, akkor az első elemet (azaz a betűt (pl. A1 -> A = [0] 1 = [1]))
                        //a könyvtár segítségével "átkonvertáljuk" számmá (pl. A = 0)
                        fromX = mapCharToIntDict[from[0]];
                        //Ez után eltávolítjuk a stringből a betűt, hogy áttudjuk konvertálni int-é a string-et
                        a = from.Remove(0, 1);

                        //Ez a try catch azért kell, hogy ne crasheljen a játék,
                        //ha rosszul adjuk meg a bekért értéket.
                        try
                        {
                            //Ha túl nagy számot adunk meg, akkor újra kezdjük
                            if (Int32.Parse(a.ToString()) > 10 || Int32.Parse(a.ToString()) < 1)
                            {
                                AnsiConsole.Write(new Markup("[red1]Helytelen érték![/]"));
                            }
                        }
                        catch
                        {
                            AnsiConsole.Write(new Markup("[red1]Helytelen érték![/]"));
                        }

                        //Átkonvertáljuk a bekért stringet int-é és kivonunk belőle egyet,
                        //mivel mi 0 és 9 között dolgozunk
                        fromY = Int32.Parse(a.ToString()) - 1;

                        bajvanmore2 = false;
                    }
                    catch
                    {
                        if (manual)
                            ReadKey();
                        else if (manual == false)
                            Thread.Sleep(sleep);
                        Clear();
                        PrintMap(map, aimap, ref Win, ref Lose);
                        AnsiConsole.Write(new Markup("[cyan3]Add meg a hajó sorszámát! (1. Repülőgép-hordozó(5), " +
                        "2. Csatahajó(4), 3. Romboló(3), 4. Tengeralattjáró(3), 5. Járőrhajó(2) )[/]"));
                        ForegroundColor = ConsoleColor.White;
                        WriteLine();
                        WriteLine(answer);
                        AnsiConsole.Write(new Markup("[cyan3]Add meg, a hajó kezdőponti és végponti koordinátáit!" + Environment.NewLine
                        + "(Fontos hogy ELŐSZÖR A KEZDŐ koordinátát adjuk meg, UTÁNA AZ UTOLSÓ koordinátát!!!)[/]"));
                        WriteLine();
                        ForegroundColor = ConsoleColor.White;
                    }

                }

                //----------------------------------------------------------------------

                while (bajvanmore3)
                {
                    try
                    {
                        //Bekérjük a második értéket és megismételjük a fenti dolgokat
                        string to = ReadLine().ToUpper();

                        if (to.Length! < 2 || to.Length! > 4)
                        {
                            AnsiConsole.Write(new Markup("[red1]Helytelen érték![/]"));
                        }
                        else if (!chars.Contains(to[0]))
                        {
                            AnsiConsole.Write(new Markup("[red1]Helytelen érték![/]"));
                        }

                        toX = mapCharToIntDict[to[0]];
                        b = to.Remove(0, 1);

                        try
                        {
                            if (Int32.Parse(b.ToString()) > 10 || Int32.Parse(b.ToString()) < 1)
                            {
                                AnsiConsole.Write(new Markup("[red1]Helytelen érték![/]"));
                            }
                        }
                        catch
                        {
                            AnsiConsole.Write(new Markup("[red1]Helytelen érték![/]"));
                        }

                        toY = Int32.Parse(b) - 1;

                        bajvanmore3 = false;
                    }
                    catch
                    {
                        if (manual)
                            ReadKey();
                        else if (manual == false)
                            Thread.Sleep(sleep);
                        Clear();
                        PrintMap(map, aimap, ref Win, ref Lose);
                        AnsiConsole.Write(new Markup("[cyan3]Add meg a hajó sorszámát! (1. Repülőgép-hordozó(5), " +
                        "2. Csatahajó(4), 3. Romboló(3), 4. Tengeralattjáró(3), 5. Járőrhajó(2) )[/]"));
                        ForegroundColor = ConsoleColor.White;
                        WriteLine();
                        WriteLine(answer);
                        AnsiConsole.Write(new Markup("[cyan3]Add meg, a hajó kezdőponti és végponti koordinátáit!" + Environment.NewLine
                        + "(Fontos hogy ELŐSZÖR A KEZDŐ koordinátát adjuk meg, UTÁNA AZ UTOLSÓ koordinátát!!!)[/]"));
                        WriteLine();
                        WriteLine(answer2);
                    }                    
                }

                //Ha mindenhol jól adtunk meg mident, akkor ezzel kilépünk a ciklusból
                bajvanmore = false;
            }

            //A létrejött int változókat egy tömbbe rakjuk, hogy könnyen tudjuk használni a "Place" függvényben is
            int[] coordinates = { fromX, fromY, toX, toY };

            //Ezeknél az if-eknél megnézzük, hogy melyik hajót választotta a kedves áldozat
            if (ship == 1)
            {
                //Ezt már leírtam fentebb a "Place" függvényben! 
                //Miért nem figyelsz arra, hogy mit olvasol?!
                if (fromX == toX)
                {
                    //Ezeknél az if-eknél ellenőrizzük, hogy megfelelő hosszúságot adott-e meg a hajók lehelyezésénél
                    if (fromY - toY == 4 || fromY - toY == -4)
                    {                        
                        //Meghívjuk a "Place" függvényt és nyugtázzuk a sikeres a lehelyezést
                        Place(ships, map, aimap, ship, coordinates, Win, Lose);
                        AnsiConsole.Write(new Markup("[green1]Sikeres a lehelyezés![/]"));
                    }
                    //Már ezt is leírtam fentebb.
                    //Hányszor mondjam még el?
                    else
                    {
                        AnsiConsole.Write(new Markup("[red1]Nem jól adtad meg a hajó hosszát![/]"));
                        Question(ships, map, aimap, Win, Lose);
                    }
                }
                //Ez ugyan az, mint az előző csak fordítva, meg amúgyis leírtam már egyszer, ha nem tűnt volna fel.
                else if (fromY == toY)
                {
                    if (fromX - toX == 4 || fromX - toX == -4)
                    {
                        Place(ships, map, aimap, ship, coordinates, Win, Lose);
                        AnsiConsole.Write(new Markup("[green1]Sikeres lehelyezés![/]"));
                    }
                    else
                    {
                        AnsiConsole.Write(new Markup("[red1]Nem jól adtad meg a hajó hosszát![/]"));
                        Question(ships, map, aimap, Win, Lose);
                    }
                }
                else
                {
                    AnsiConsole.Write(new Markup("[red1]Nem jól adtad meg a hajó hosszát![/]"));
                    Question(ships, map, aimap, Win, Lose);
                }
            }
            //Innentől csak ismételgetjük a fentit, minden hajótípusnál, a hajóhosszúságot megváltoztatva
            else if (ship == 2)
            {
                if (fromX == toX)
                {
                    if (fromY - toY == 3 || fromY - toY == -3)
                    {
                        Place(ships, map, aimap, ship, coordinates, Win, Lose);
                        AnsiConsole.Write(new Markup("[green1]Sikeres lehelyezés![/]"));
                    }
                    else
                    {
                        AnsiConsole.Write(new Markup("[red1]Nem jól adtad meg a hajó hosszát![/]"));
                        Question(ships, map, aimap, Win, Lose);
                    }
                }
                else if (fromY == toY)
                {
                    if (fromX - toX == 3 || fromX - toX == -3)
                    {
                        Place(ships, map, aimap, ship, coordinates, Win, Lose);
                        AnsiConsole.Write(new Markup("[green1]Sikeres lehelyezés![/]"));
                    }
                    else
                    {
                        AnsiConsole.Write(new Markup("[red1]Nem jól adtad meg a hajó hosszát![/]"));
                        Question(ships, map, aimap, Win, Lose);
                    }
                }
                else
                {
                    AnsiConsole.Write(new Markup("[red1]Nem jól adtad meg a hajó hosszát![/]"));
                    Question(ships, map, aimap, Win, Lose);
                }
            }
            else if (ship == 3 || ship == 4)
            {
                if (fromX == toX)
                {
                    if (fromY - toY == 2 || fromY - toY == -2)
                    {
                        Place(ships, map, aimap, ship, coordinates, Win, Lose);
                        AnsiConsole.Write(new Markup("[green1]Sikeres lehelyezés![/]"));
                    }
                    else
                    {
                        AnsiConsole.Write(new Markup("[red1]Nem jól adtad meg a hajó hosszát![/]"));
                        Question(ships, map, aimap, Win, Lose);
                    }
                }
                else if (fromY == toY)
                {
                    if (fromX - toX == 2 || fromX - toX == -2)
                    {
                        Place(ships, map, aimap, ship, coordinates, Win, Lose);
                        AnsiConsole.Write(new Markup("[green1]Sikeres lehelyezés![/]"));
                    }
                    else
                    {
                        AnsiConsole.Write(new Markup("[red1]Nem jól adtad meg a hajó hosszát![/]"));
                        Question(ships, map, aimap, Win, Lose);
                    }
                }
                else
                {
                    AnsiConsole.Write(new Markup("[red1]Nem jól adtad meg a hajó hosszát![/]"));
                    Question(ships, map, aimap, Win, Lose);
                }
            }
            else if (ship == 5)
            {
                if (fromX == toX)
                {
                    if (fromY - toY == 1 || fromY - toY == -1)
                    {
                        Place(ships, map, aimap, ship, coordinates, Win, Lose);
                        AnsiConsole.Write(new Markup("[green1]Sikeres lehelyezés![/]"));
                    }
                    else
                    {
                        AnsiConsole.Write(new Markup("[red1]Nem jól adtad meg a hajó hosszát![/]"));
                        Question(ships, map, aimap, Win, Lose);
                    }
                }
                else if (fromY == toY)
                {
                    if (fromX - toX == 1 || fromX - toX == -1)
                    {
                        Place(ships, map, aimap, ship, coordinates, Win, Lose);
                        AnsiConsole.Write(new Markup("[green1]Sikeres lehelyezés![/]"));
                    }
                    else
                    {
                        AnsiConsole.Write(new Markup("[red1]Nem jól adtad meg a hajó hosszát![/]"));
                        Question(ships, map, aimap, Win, Lose);
                    }
                }
                else
                {
                    AnsiConsole.Write(new Markup("[red1]Nem jól adtad meg a hajó hosszát![/]"));
                    Question(ships, map, aimap, Win, Lose);
                }
            }
        }

        public void Shoot(int[,] map, int[,] aimap, int[] enemyships, int Win, int Lose)
        {

            if (first)
            {
                Thread.Sleep(500);
                first = false;
            }
            else
            {
                if (manual)
                    ReadKey();
                else if (manual == false)
                    Thread.Sleep(sleep);
            }
            Clear();
            PrintMap(map, aimap, ref Win, ref Lose);

            AnsiConsole.Write(new Markup("[cyan3]Löveg betöltve! Adja meg a cél kordinátákat![/]"));
            WriteLine();
            ForegroundColor = ConsoleColor.White;

            //Bekérjük a koordinátát
            string target = ReadLine().ToUpper();

            //Megnézzük, hogy jól adta-e meg a felhasználó a koordinátát
            //Majdnem minden ugyanaz mint az előbb
            if (target.Length !< 2 || target.Length !> 4)
            {
                AnsiConsole.Write(new Markup("[red1]Nem érvényes koordináta![/]"));
                Shoot(map, aimap, enemyships, Win, Lose);
                return;
            }
            
            if (!chars.Contains(target[0]))
            {
                AnsiConsole.Write(new Markup("[red1]Nem érvényes koordináta![/]"));
                Shoot(map, aimap, enemyships, Win, Lose);
            }

            //Ha igen akkor megint szétbontjuk, úgy mint előzőleg
            int targetX = mapCharToIntDict[target[0]];
            string a = target.Remove(0, 1);

            //Megint ugyanaz, mint az előbb
            try
            {
                if (Int32.Parse(a.ToString()) > 10 || Int32.Parse(a.ToString()) < 1)
                {
                    AnsiConsole.Write(new Markup("[red1]Nem érvényes koordináta![/]"));
                    Shoot(map, aimap, enemyships, Win, Lose);
                }
            }
            catch
            {
                AnsiConsole.Write(new Markup("[red1]Nem érvényes koordináta![/]"));
                Shoot(map, aimap, enemyships, Win, Lose);
            }

            //Ide már csak akkor érünk el, ha jól adtuk meg a koordinátát

            //Szintén ugyanaz
            int targetY = Int32.Parse(a.ToString()) - 1;

            //Ha ez true lesz, akkor oda már lőttünk
            bool shot = false;
            
            //Itt nézzük meg, hogy már lőttünk-e az adott koordinátára
            if (aimap[targetX, targetY] == -1 || aimap[targetX, targetY] == -2)
            {
                shot = true;
            }

            //Ha nem lőttünk még a koordinátára belépünk az if-be
            if (!shot)
            {
                //Ha az adott koordinátán van hajó, akkor ebbe lépünk be
                if(aimap[targetX, targetY] == 1)
                {
                    //A mátrixon átváltoztajuk a koordinátán lévő értéket "-2"-re
                    //Ezzel jelölve, hogy eltaláltunk egy hajót
                    aimap[targetX, targetY] = -2;
                    //Meghívjuk a "Sink" függvényt, ezzel ellenőrizve,
                    //hogy elsüllyedt-e egy hajó
                    Sink(aimap, enemyships, Win, Lose);
                    //Frissítjük a Map-ot, hogy kirajzoljuk a változásokat
                    Clear();
                    PrintMap(map, aimap, ref Win, ref Lose);
                    AnsiConsole.Write(new Markup("[maroon]Találat![/]"));
                    //Itt pedig kiírjuk, hogyha egy hajó elsüllyedt
                    if (Car == 5)
                    {
                        WriteLine();
                        AnsiConsole.Write(new Markup("[greenyellow]Az ellenség Repülőgép-hordozója elsüllyedt![/]"));
                    }

                    if (Bat == 4)
                    {
                        WriteLine();
                        AnsiConsole.Write(new Markup("[greenyellow]Az ellenség Csatahajója elsüllyedt![/]"));
                    }

                    if (Des == 3)
                    {
                        WriteLine();
                        AnsiConsole.Write(new Markup("[greenyellow]Az ellenség Rombolója elsüllyedt![/]"));
                    }

                    if (Sub == 3)
                    {
                        WriteLine();
                        AnsiConsole.Write(new Markup("[greenyellow]Az ellenség Tengeralattjárója elsüllyedt![/]"));
                    }

                    if (Pat == 2)
                    {
                        WriteLine();
                        AnsiConsole.Write(new Markup("[greenyellow]Az ellenség Járőrhajója elsüllyedt![/]"));
                    }
                }
                //Ha nincs hajó, akkor pedig ebbe lépünk be
                else
                {
                    //Mivel nem talált a lövés, ezért ezt "-1"-el jelöljük a mátrixban
                    aimap[targetX, targetY] = -1;
                    //Frissítjük a Map-ot, hogy kirajzoljuk a változásokat
                    Clear();
                    PrintMap(map, aimap, ref Win, ref Lose);
                    AnsiConsole.Write(new Markup("[grey58]A francba! Nincs találat![/]"));
                }
            }
            //Ha a "shot" true, akkor újra meghívjuk a fügvényt
            else
            {
                AnsiConsole.Write(new Markup("[red1]Nem érvényes koordináta![/]"));
                Shoot(map, aimap, enemyships, Win, Lose);
            }

        }

        //Létrehozunk egy függvényt, amiben vizsgáljuk, hogy elsüllyedt-e egy hajó
        public void Sink(int[,] aimap, int[] enemyships, int Win, int Lose)
        {
            //fromX = [0]
            //fromY = [1]
            //toX = [2]
            //toY = [3]

            //------------------------------------------------------------
            //Gábor része

            // Létrehozzuk a hajók koordinátáit tartalmazó tömböt
            string[][] ships = new string[5][];

            // berakjuk a koordinátákat egy tömbbe
            ships[0] = EnemyShipsCoords[0].ToString().Split(';');
            ships[1] = EnemyShipsCoords[1].ToString().Split(';');
            ships[2] = EnemyShipsCoords[2].ToString().Split(';');
            ships[3] = EnemyShipsCoords[3].ToString().Split(';');
            ships[4] = EnemyShipsCoords[4].ToString().Split(';');

            int[][] intShips = new int[5][];

            // átmegyünk az összes tömbon
            for (int i = 0; i < ships.Length; i++)
            {
                // létrehozunk egy ideiglenes tömböt
                int[] thisInt = new int[4];

                // átmágyünk az összes elemen és int-é alakítjuk
                for (int j = 0; j < ships[i].Length; j++)
                {
                    thisInt[j] = Int32.Parse(ships[i][j]);
                }

                // hozzáadjuk az ideiglenes tomböt a tömböket tartalmazö tömbhöz
                intShips[i] = thisInt;
            }

            // Kiszedjük a tömböket külön változókba
            int[] Carrier = intShips[0];
            int[] BattleShip = intShips[1];
            int[] Destroyer = intShips[2];
            int[] Submarine = intShips[3];
            int[] PatrolBoat = intShips[4];

            //------------------------------------------------------------

            //Ezekkel számoljuk, hogy hány találat érte az adott hajót
            Car = 0;
            Bat = 0;
            Des = 0;
            Sub = 0;
            Pat = 0;

            //Csak akkor lépünk be, ha még nincs elsüllyedve a hajó
            if (!EnemySinkedC)
            {
                //Ezek a hajó koordinátái, amivel vizsgáljuk,
                //hogy sorban vagy oszlopban van lehelyezve
                if (Carrier[0] == Carrier[2])
                {
                    //Ezekkel a "for"-okkal végig megyünk a hajón
                    //(azaz a két végpontja között lépkedünk) 
                    for (int i = Carrier[1]; i <= Carrier[3]; i++)
                        //Amikor találunk a mátrixban egy "-2"-t akkor
                        //hozzáadunk a hajó számlálójához egyet
                        if (aimap[i, Carrier[0]] == -2)
                            Car++;
                }
                //Ugyanaz csak most állítva nézzük
                else if (Carrier[1] == Carrier[3])
                {
                    for (int i = Carrier[0]; i <= Carrier[2]; i++)
                        if (aimap[Carrier[1], i] == -2)
                            Car++;
                }
            }

            //Innentől ezt ismételgetjük minden hajónál
            if (!EnemySinkedB)
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

            if (!EnemySinkedD)
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

            if (!EnemySinkedS)
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

            if (!EnemySinkedP)
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

            //Ha eléri a "számláló" a hajó hosszúságát, akkor elsüllyedt
            //Ezt egy bool változóban is jellezzük, illetve az "EnemyShips" tömbben
            //azt az értéket ahol tároljuk a hajót 1-ről 0-ra csökkentjük
            if (Car == 5)
            {
                EnemySinkedC = true;
                enemyships[0] = 0;
            }
           
            if (Bat == 4)
            {
                EnemySinkedB = true;
                enemyships[1] = 0;
            }

            if (Des == 3)
            {
                EnemySinkedD = true;
                enemyships[2] = 0;
            }

            if (Sub == 3)
            {
                EnemySinkedS = true;
                enemyships[3]= 0;
            }

            if (Pat == 2)
            {
                EnemySinkedP = true;
                enemyships[4] = 0;
            }
        }

//--------------------------------------------------------------------------------------------------------------------------------------
//Gábor kódja:

        // jobb ai
        private bool didShootGoodBefore = false;
        private int[] lastGoodCoords = new int[2];

        private int tries = 0;

        public int[] AiMoveToNext(int[,] map) {
            Random rnd = new Random();

            int side = rnd.Next(0, 3);

            // Eltároljuk a legutolsó jó koordinátákat
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

            // Megkeressük az első jó koordinátát
            while (map[shootRow, shootCol] < 0 && !isOccupiedOnAllSides) {
                // Kiválasztunk egy oldalt
                // 0 - bal
                // 1 - jobb
                // 2 - fel
                // 3 - le
                side = rnd.Next(0, 3);

                // megvizsgáljuk, hogy az összes oldalról van-e foglalt mező
                isOccupiedOnAllSides =
                    isRowLeftOccupied &&
                    isRowRightOccupied &&
                    isColLeftOccupied &&
                    isColRightOccupied;

                // Visszaállítjuk a koordinátákat
                shootRow = lastGoodCoords[0];
                shootCol = lastGoodCoords[1];

                // Megnézzük, hogy az oldal foglalt-e, ha nem akkor arra irányitjuk a kooridnátákat
                switch (side) {
                    case 0:
                        if (shootCol == 0) {
                            isColLeftOccupied = true;

                            continue;
                        }

                        shootCol--;

                        if (map[shootRow, shootCol] < 0) {
                            isColLeftOccupied = true;

                            shootCol++;
                        }

                        break;
                    case 1:
                        if (shootCol == 9) {
                            isColRightOccupied = true;

                            continue;
                        }

                        shootCol++;

                        if (map[shootRow, shootCol] < 0) {
                            isColRightOccupied = true;

                            shootCol--;
                        }

                        break;
                    case 2:
                        if (shootRow == 0) {
                            isRowLeftOccupied = true;

                            continue;
                        }

                        shootRow--;

                        if (map[shootRow, shootCol] < 0) {
                            isRowLeftOccupied = true;

                            shootRow++;
                        }

                        break;
                    case 3:
                        if (shootRow == 9) {
                            isRowRightOccupied = true;

                            continue;
                        }

                        shootRow++;

                        if (map[shootRow, shootCol] < 0) {
                            isRowRightOccupied = true;

                            shootRow--;
                        }

                        break;
                }
            }

            // Megizsgáljuk hogy az összes oldalról van-e foglalt mező
            // ha igen akkor -1,-1-et adunk vissza
            if (isOccupiedOnAllSides) {
                shootRow = -1;
                shootCol = -1;

                didShootGoodBefore = false;
            }

            return new [] { shootRow, shootCol };
        }

        public void AI_Shoot(int[,] map, int[,] aimap, int[] friendlyships, int Win, int Lose)
        {
            if (manual)
                ReadKey();
            else if (manual == false)
                Thread.Sleep(sleep);
            Clear();
            PrintMap(map, aimap, ref Win, ref Lose);

            Random rnd = new Random();

            //Eredetileg Dict-ben voltak a dolgok tárolva ezért olvashatóak az alábbi kommentek!

            //Első random kulcs - értékek létreholzása
            int shootRow = rnd.Next(0, 10);
            int shootCol = rnd.Next(0, 10);

            // Megnézzuk hogy volt-e találatunk előzőleg
            if (didShootGoodBefore) {
                tries = 0;

                int[] aiii = AiMoveToNext(map);

                // megnézzük hogy a visszaadott koordináták nem -1,-1-e
                // ha nem azok akkor a visszaadott koordinátákat használjuk
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
                
//--------------------------------------------------------------------------------------------------------------------------------------

                //Meghívjuk az "AI_Sink" függvényt, hogy ellenőrizzük, hogy elsüllyedt-e egy hajónk
                AI_Sink(map, friendlyships);

                //Frissítjuk a map-ot
                Clear();
                PrintMap(map, aimap, ref Win, ref Lose);

                AnsiConsole.Write(new Markup("[maroon]Az ellenség eltalálta az egyik hajódat![/]"));
                //Kiírjuk ha elsüllyedt egy hajónk
                if (AICar == 5)
                {
                    WriteLine();
                    AnsiConsole.Write(new Markup("[greenyellow]A Repülőgép-hordozód elsüllyedt![/]"));
                }

                if (AIBat == 4)
                {
                    WriteLine();
                    AnsiConsole.Write(new Markup("[greenyellow]A Csatahajód elsüllyedt![/]"));
                }

                if (AIDes == 3)
                {
                    WriteLine();
                    AnsiConsole.Write(new Markup("[greenyellow]A Rombolód elsüllyedt![/]"));
                }

                if (AISub == 3)
                {
                    WriteLine();
                    AnsiConsole.Write(new Markup("[greenyellow]A Tengeralattjáród elsüllyedt![/]"));
                }

                if (AIPat == 2)
                {
                    WriteLine();
                    AnsiConsole.Write(new Markup("[greenyellow]A Járőrhajód elsüllyedt![/]"));
                }
            }
            else
            {
                map[shootRow, shootCol] = -1;

                Clear();
                PrintMap(map, aimap, ref Win, ref Lose);

                AnsiConsole.Write(new Markup("[grey58]Az ellenség lövése nem talált![/]"));
            }
        }

        //Ugyanaz mint a "Sink" függvény, csak az ellenfél hajói helyett
        //a saját hajóinkat vizsgáljuk
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

            AICar = 0;
            AIBat = 0;
            AIDes = 0;
            AISub = 0;
            AIPat = 0;

            if (!FriendlySinkedC)
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

            if (!FriendlySinkedB)
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

            if (!FriendlySinkedD)
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

            if (!FriendlySinkedS)
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

            if (!FriendlySinkedP)
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
                FriendlySinkedC = true;
                friendlyships[0] = 0;
            }

            if (AIBat == 4)
            {
                FriendlySinkedB = true;
                friendlyships[1] = 0;
            }

            if (AIDes == 3)
            {
                FriendlySinkedD = true;
                friendlyships[2] = 0;
            }

            if (AISub == 3)
            {
                FriendlySinkedS = true;
                friendlyships[3] = 0;
            }

            if (AIPat == 2)
            {
                FriendlySinkedP = true;
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

                //Elágazás fő feladata: random értékek helyre rakása, hogy ne tudjunk "kicsordulni" a map-ről
                //Ha a random érték és a 10 között van akkora hely, amekkora a random kiválasztott hajótípusnak kell,
                //akkor a hajó típus hosszát hozzá adjuk a random értékhez
                //Ha nincs elegendő hely akkor kivonjuk a hajó hosszát a értékből
                if (vertical == 0)
                {
                    int fromNum = randomNum;
                    char fromChar = randomChar;

                    //Hosszúság vizsgálat
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

                    //Hosszúság vizsgálat
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

            // Lerakjuk az öt hajót
            for (int i = 0; i < 5; i++)
            {
                // Legeneráljuk, hogy vízszintesen vagy függőlegesen legyen-e
                bool isHorizontal = randInt.Next(0, 100) > 50;

                //Console.WriteLine(shipSizes[i + 1]);

                bool cant = true;

                // addig probálkozunk míg `cant` nem lesz `false`
                while (cant)
                {
                    if (isHorizontal)
                    {
                        // generáluunk egy random oszlopot és sort
                        int columnRand = randInt.Next(0, 10);
                        int rowRand = randInt.Next(0, 10 - shipSizes[i + 1]);

                        // megadjuk a hajó kooridnátáit
                        int fromX = rowRand;
                        int fromY = columnRand;

                        int toX = rowRand + shipSizes[i + 1];
                        int toY = columnRand;

                        // Ellenőrzi, hogy el lehet-e helyezni a hajót a megadott helyen
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
                        // generáluunk egy random oszlopot és sort
                        int rowRand = randInt.Next(0, 10);
                        int columnRand = randInt.Next(0, 10 - shipSizes[i + 1]);

                        // megadjuk a hajó kooridnátáit
                        int fromX = rowRand;
                        int fromY = columnRand;

                        int toX = rowRand;
                        int toY = columnRand + shipSizes[i + 1];

                        // Ellenőrzi, hogy el lehet-e helyezni a hajót a megadott helyen
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

            // Megnézzük hogy a koordináták egy sorban vannak-e
            if (fromX == toX)
            {
                // Megnézzük hogy a letteni kivánt hajó hossza megfelel-e a hajó típusának
                if (toY - fromY != shipSizes[shipType])
                    return false;

                // Megnézzük hogy a koordináták között van-e foglalt mező
                for (int i = fromY; i <= toY; i++)
                    if (map[i, fromX] != 0)
                        return false;

                return true;
            }

            // Megnézzük hogy a koordináták egy oszlopban vannak-e
            if (fromY == toY)
            {
                // Megnézzük hogy a letteni kivánt hajó hossza megfelel-e a hajó típusának
                if (toX - fromX != shipSizes[shipType])
                    return false;

                // Megnézzük hogy a koordináták között van-e foglalt mező
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

            // Lehelyezzük a hajót
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

        //Ezeket a listákat, változókat, és tömböt több függvényben is használjuk,
        //ezért hozzuk létre itt őket, hogy bárhol lehessen használni
        //a "Torpedo" osztályon belül

        ArrayList EnemyShipsCoords = new ArrayList();
        ArrayList FriendlyShipsCoords = new ArrayList();

        bool EnemySinkedC = false;
        bool EnemySinkedB = false;
        bool EnemySinkedD = false;
        bool EnemySinkedS = false;
        bool EnemySinkedP = false;

        bool FriendlySinkedC = false;
        bool FriendlySinkedB = false;
        bool FriendlySinkedD = false;
        bool FriendlySinkedS = false;
        bool FriendlySinkedP = false;

        int Car = 0;
        int Bat = 0;
        int Des = 0;
        int Sub = 0;
        int Pat = 0;

        int AICar = 0;
        int AIBat = 0;
        int AIDes = 0;
        int AISub = 0;
        int AIPat = 0;

        bool selfAI = false;
        bool manual = false;

        char[] chars = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };

        int sleep = 0;

        int answer = 0;
        string answer2 = null;
        string answer3 = "a";

        bool first = true;

        //Létrehozunk egy Menu függvényt,
        //a játék elején lévő kérdések kiíráshoz és használáshoz
        public void Menu(int[] ships, int[,] map, int[,] aimap, int Win, int Lose)
        {
            Thread.Sleep(500);
            Clear();
            PrintMap(map, aimap, ref Win, ref Lose);

            bool nocrash = true;
            bool nocrash2 = true;

            AnsiConsole.Write(new Markup("[cyan3]Ha te szeretnéd be írni, hogy hány másodperc múlva tűnjenek el az 'értesítések' a képernyőről nyomd meg az 1-et." + Environment.NewLine
            + "Ha azt szeretnéd, hogy csak akkor tűnjenek el, ha megnyomsz egy gombot, nyomd meg a 2-t.[/]"));
            WriteLine();
            ForegroundColor = ConsoleColor.White;

            while (nocrash)
            {
                try
                {

                    //Bekérjük a számot
                    string szoveg = ReadLine();
                    answer3 = szoveg;

                    if (szoveg.Length! < 1 || szoveg.Length! > 1)
                    {
                        AnsiConsole.Write(new Markup("[red1]Rossz érték![/]"));
                    }

                    //Ezt gondolom nem kell magyarázni
                    if (Int32.Parse(szoveg.ToString()) == 1)
                    {
                        manual = false;
                        AnsiConsole.Write(new Markup("[cyan3]Add meg, hogy hány másodperc múlva tűnjenek el az értesítések. (pl. 1 vagy 1.5)[/]"));
                        WriteLine();
                        string answer = ReadLine();
                        float asd = float.Parse(answer.ToString());
                        asd = asd * 1000;

                        sleep = Int32.Parse(asd.ToString());

                        nocrash = false;
                    }
                    else if (Int32.Parse(szoveg.ToString()) == 2)
                    {
                        manual = true;

                        nocrash = false;
                    }
                    else
                    {
                        WriteLine("c" + szoveg);
                        AnsiConsole.Write(new Markup("[red1]Rossz érték![/]"));
                        //Menu(ships, map, aimap, Win, Lose);
                    }
                }
                catch
                {
                    Thread.Sleep(500);
                    Clear();
                    PrintMap(map, aimap, ref Win, ref Lose);
                    AnsiConsole.Write(new Markup("[cyan3]Ha te szeretnéd be írni, hogy hány másodperc múlva tűnjenek el az 'értesítések' a képernyőről nyomd meg az 1-et." + Environment.NewLine
                    + "Ha azt szeretnéd, hogy csak akkor tűnjenek el, ha megnyomsz egy gombot, nyomd meg a 2-t.[/]"));
                    WriteLine();
                    ForegroundColor = ConsoleColor.White;
                }
            }

            AnsiConsole.Write(new Markup("[cyan3]Ha te szeretnéd lehelyezni a hajóidat, akkor írj be egy 1-et." + Environment.NewLine
                    + "Ha véletlenszerű lehelyezést szeretnél, írj be egy 2-t.[/]"));
            WriteLine();
            ForegroundColor = ConsoleColor.White;

            while (nocrash2)
            {
                try
                {
                    //Ugyanaz mint az előbb
                    string lehelyezés = ReadLine();

                    if (lehelyezés.Length! < 1 || lehelyezés.Length! > 1)
                    {
                        AnsiConsole.Write(new Markup("[red1]Rossz érték![/]"));
                    }

                    if (Int32.Parse(lehelyezés.ToString()) == 1)
                    {
                        //Meghívjuk 5-ször a "Question" függvényt, ezzel elindítva a bekérdezést
                        for (int i = 0; i < 5; i++)
                        {
                            Question(ships, map, aimap, Win, Lose);
                        }
                    }
                    else if (Int32.Parse(lehelyezés.ToString()) == 2)
                    {
                        //Az AIGenerate függvényt használjuk, csak a saját map-el,
                        //ezzel megspórolva egy függvényt
                        selfAI = true;
                        AIGenerate(map);
                    }
                    else
                    {
                        AnsiConsole.Write(new Markup("[red1]Rossz érték![/]"));
                        //Menu(ships, map, aimap, Win, Lose);
                    }
                    nocrash2 = false;                                       
                }
                catch
                {
                    Thread.Sleep(500);
                    Clear();
                    PrintMap(map, aimap, ref Win, ref Lose);
                    AnsiConsole.Write(new Markup("[cyan3]Ha te szeretnéd be írni, hogy hány másodperc múlva tűnjenek el az 'értesítések' a képernyőről nyomd meg az 1-et." + Environment.NewLine
                    + "Ha azt szeretnéd, hogy csak akkor tűnjenek el, ha megnyomsz egy gombot, nyomd meg a 2-t.[/]"));
                    WriteLine();
                    ForegroundColor = ConsoleColor.White;
                    WriteLine(answer3);
                    AnsiConsole.Write(new Markup("[cyan3]Ha te szeretnéd lehelyezni a hajóidat, akkor írj be egy 1-et." + Environment.NewLine
                    + "Ha véletlenszerű lehelyezést szeretnél, írj be egy 2-t.[/]"));
                    WriteLine();
                }
            }
        }        

        //Ez a függvény beolvassa a mentésünket,
        //Ha nincs mentésünk létrehozz egyet
        public void Read(ref int winCount, ref int loseCount) 
        {
            try
            {
                string line = null;

                StreamWriter sw = null;

                //Ha a file nem létezik létrehozzuk azt egy 0;0 eredménnyel
                if (!File.Exists("../../save.txt"))
                {
                    sw = new StreamWriter("../../save.txt");
                    sw.WriteLine(0 + ";" + 0);
                    sw.Close();
                }

                //Ha létezik a file, ha nem, az alábbiak akkor is lefutnak
                StreamReader sr = new StreamReader("../../save.txt");
                line = sr.ReadLine();

                while (line != null)
                {
                    string[] counts = line.Split(';');

                    //Csak akkor változtatjuk meg az eredményeket, ha azok kisebbek a mentetnél
                    //Ezzel elkerülve azt, hogy mindig 0 legyen az eredmény, ha nincs már mentésünk
                    if (winCount < int.Parse(counts[0]))
                    {
                        winCount = int.Parse(counts[0]);
                    }
                    if (loseCount < int.Parse(counts[1]))
                    {
                        loseCount = int.Parse(counts[1]);
                    }                   
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception e) { Console.WriteLine(e); }
        }

        //Ezzel a függvénnyel pedig "mentjük" az eredményünket, azaz kiírjuk egy file-ba
        public void Write(ref int winCount, ref int loseCount)
        {

            StreamWriter sw = null;

            //Eredmények fileba írása mentés gyanánt
            try
            {
                sw = new StreamWriter("../../save.txt");
                sw.WriteLine(winCount + ";" + loseCount);
                sw.Close();
            }
            catch (Exception e) { Console.WriteLine(e); }
        }
    }

    //Végre jön a játék meghívása
    class Program
    {
        static void Main(string[] args)
        {
            //Amíg ez true, addig fut a játék
            bool run = true;

            //Ez a kettő változó nyeri ki az eredményeket, közvetlen nem kell a számlálóhoz!
            int WinCount = 0;
            int LoseCount = 0;

            //Átrakjuk a konzol megjelenítését UTF-8-ra, hogy látszódjanak a hosszú magánhangzók, ha már ekkora gyász nyelv ez a magyar.
            Console.OutputEncoding = Encoding.UTF8;

            //Létre hozzuk a már sokat emlegetett Map mátrixot (vagy mit xd)
            int[,] Map = new int[10, 10];

            //Illetve az ellenfél Map mátrixát is
            int[,] AI_Map = new int[10, 10];

            //Ebben a tömbben tároljuk a saját hajóink típusait, abban a sorrendben, ahogy kiírtuk a konzolra a választásnál
            //(Carrier(1), BattleShip(2), Destroyer(3), Submarine(4), PatrolBoat(5)
            int[] EnemyShips = { 1, 1, 1, 1, 1 };
            //Ebben pedig az ellenfél hajóit
            int[] FriendlyShips = { 1, 1, 1, 1, 1 };

            //A Torpedo osztályt "game"-ként "hozzuk" létre
            Torpedo game = new Torpedo();

            //File műveletek példányosítása
            game.Read(ref WinCount, ref LoseCount);
            game.Write(ref WinCount, ref LoseCount);

            //Meghívjuk a "PrintMap" és "AIGenerate" függvényeket,
            //ezzel kirajzolva a map-ot és lehelyezve az ellenfél hajóit
            game.AIGenerate(AI_Map);
            game.PrintMap(Map, AI_Map, ref WinCount, ref LoseCount);

            //Menu beizzítása a hozzá való argumentumokkal
            game.Menu(EnemyShips, Map, AI_Map, WinCount, LoseCount);

            //Ameddig csatában vagyunk addig a run = true
            while (run)
            {
                //Lövés funkció player oldal
                game.Shoot(Map, AI_Map, EnemyShips, WinCount, LoseCount);

                //Megvizsgáljuk mennyi hajó maradt, majd ennek következtében,
                //ha nem maradt egy ellenséges hajó sem, CW(Siker üzenet) + kilépünk a ciklusból
                if (EnemyShips[0] == 0 && EnemyShips[1] == 0 && EnemyShips[2] == 0 && EnemyShips[3] == 0 && EnemyShips[4] == 0)
                {                    
                    run = false;
                    WinCount++;
                    game.Write(ref WinCount, ref LoseCount);
                    Clear();
                    game.PrintMap(Map, AI_Map, ref WinCount, ref LoseCount);
                    AnsiConsole.Write(new Markup("[green1]🏆Sikeresen elsüllyeszted az ellenség összes hajóját, ezzel megnyerted a csatát!🏆[/]"));
                }

                //Szeretne-e még játszani a player?
                if (!run)
                {
                    WriteLine();
                    AnsiConsole.Write(new Markup("[cyan3]Szeretnél még egyet játszani? (I/N)[/]"));
                    WriteLine();
                    ForegroundColor = ConsoleColor.White;
                    string yes = ReadLine();
                    if (yes == "I" || yes == "i")
                    {

                        //Ha szeretne játszani még, akkor a run = true lesz, ezzel elindítva a csatát
                        run = true;
                        AnsiConsole.Write(new Markup("[cyan3]Akkor kezdődjön az új csata![/]"));
                        Thread.Sleep(1000);
                        Clear();

                        //A mátrixon belül minden értéket 0-ra módosítunk, vagyis reseteljük a mapot, mert itt már újra kezdődőtt a játék
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

                        //A csatához szükséges függvények meghívása mint fentebb
                        //Map kirajzolása, menu életre keltése
                        game.AIGenerate(AI_Map);
                        game.PrintMap(Map, AI_Map, ref WinCount, ref LoseCount);
                        game.Menu(EnemyShips, Map, AI_Map, WinCount, LoseCount);
                    }

                    //Kedves kis szöveg a felhasználónk ha már nem szeretne játszani
                    else if (yes == "N" || yes == "n")
                    {
                        Clear();
                        AnsiConsole.Write(new Markup("[cyan3]Reméljük hamar viszontlátunk![/]"));
                        Environment.Exit(0);
                    }
                }
                else
                {
                    //Lövés funkció AI oldal
                    game.AI_Shoot(Map, AI_Map, FriendlyShips, WinCount, LoseCount);

                    //Hajó mennyiségek vizsgálata a player oldalán, lényegében ugyan az mint fentebb
                    if (FriendlyShips[0] == 0 && FriendlyShips[1] == 0 && FriendlyShips[2] == 0 && FriendlyShips[3] == 0 && FriendlyShips[4] == 0)
                    {
                        run = false;
                        LoseCount++;
                        game.Write(ref WinCount, ref LoseCount);
                        Clear();
                        game.PrintMap(Map, AI_Map, ref WinCount, ref LoseCount);
                        AnsiConsole.Write(new Markup("[red1]⛔Sajnos az ellenség elsüllyesztette az összes hajódat, ezzel elvesztetted a csatát!⛔[/]"));
                    }
                    if (!run)
                    {
                        WriteLine();
                        AnsiConsole.Write(new Markup("[cyan3]Szeretnél még egyet játszani? (I/N)[/]"));
                        WriteLine();
                        ForegroundColor = ConsoleColor.White;
                        string yes = ReadLine();
                        if (yes.ToUpper() == "I")
                        {
                            //Új játék
                            run = true;
                            AnsiConsole.Write(new Markup("[cyan3]Akkor kezdődjön az új csata![/]"));
                            Thread.Sleep(1000);
                            Clear();

                            //Reset map
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

                            //A csatához szükséges függvények meghívása mint fentebb
                            //Map kirajzolása, menu életre keltése
                            game.AIGenerate(AI_Map);
                            game.PrintMap(Map, AI_Map, ref WinCount, ref LoseCount);
                            game.Menu(EnemyShips, Map, AI_Map, WinCount, LoseCount);
                        }
                        else if (yes.ToUpper() == "N")
                        {
                            Clear();
                            AnsiConsole.Write(new Markup("[cyan3]Reméljük hamar viszontlátunk![/]"));
                            Environment.Exit(0);
                        }
                    }
                }
            }
        }
    }
}

//Ezt a játékot HaxMaster, TheClashFruit, és HerBen készítette
//Ha tetszik a játék egy "Star"-t megköszönünk!
//Az alábbi linken érhető el:
//https://github.com/ItsMeHaxMaster/Torpedo-he