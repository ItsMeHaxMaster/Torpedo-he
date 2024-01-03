using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Torpedo_AI
{
    internal class Program
    {
        static void Main(string[] args)
        {

        }
        static void Ai_Random()
        {
            Random randInt = new Random();
            string chars = "ABCDEFGHIJ";

            for (int i = 0; i < 5; i++)
            {
                int vertical = randInt.Next(0, 1); //Ha 0 vízszintes, ha 1 függőleges
                int randomNum = randInt.Next(1, 11);
                char randomChar = chars[randInt.Next(1, 11)];

                int[] ships = {5, 4, 3, 3, 2};
                int ship_L = ships[randInt.Next(0, 4)]; //ship_L = ship_Lenght

                //From értékek: betű, szám - fromChar, fromNum
                //To értékek betű, szám - toChar, toNum

                if(vertical == 0)
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
                    if(10 - randomNum > ship_L)
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
    }
}
