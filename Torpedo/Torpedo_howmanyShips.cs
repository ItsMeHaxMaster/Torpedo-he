using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torpedo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string res = howmanyShips(2, 5, 'A', 'F');
            Console.WriteLine(res);

        }
        static string howmanyShips(int yourNums_from, int yourNums_to, char yourLetter_from, char yourLetter_to)
        {

            //Simulation
            //int yourNums_from = 2;
            //int yourNums_to = 5;

            //All ships do you have
            int five = 1;
            int four = 1;
            int three = 2;
            int two = 1;

            // Create dictionary
            Dictionary<char, int> Letters = new Dictionary<char, int>();

            // Letters & numbers add to the dict
            for (char leter = 'A'; leter <= 'J'; leter++)
            {
                int num = (int)leter - (int)'A' + 1;
                Letters.Add(leter, num);
            }
            //Simulation
            //char yourLetter_from = 'A';
            //char yourLetter_to = 'A';

            // Indexdiff
            int index_from = Letters[yourLetter_from];
            int indcex_to = Letters[yourLetter_to];
            int indexDifference = Math.Abs(indcex_to - index_from);


            //How many ships do you have
            if (yourNums_from == yourNums_to)
            { }

            else if (yourNums_to - yourNums_from == 5) { five--; }
            else if (yourNums_to - yourNums_from == 4) { four--; }
            else if (yourNums_to - yourNums_from == 3) { three--; }
            else if (yourNums_to - yourNums_from == 2) { two--; }

            //Letter examination
            if (indexDifference == 0) { }

            else if (indexDifference == 5) { five--; }
            else if (indexDifference == 4) { four--; }
            else if (indexDifference == 3) { three--; }
            else if (indexDifference == 2) { two--; }

            return $"{five} {four} {three} {two}";
        }
    }
}
