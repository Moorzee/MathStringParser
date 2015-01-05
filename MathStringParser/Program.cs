using System;
using System.Globalization;

namespace MathStringParser
{
    class Program
    {
        static bool _quit;
        static void Main(string[] args)
        {
            while (!_quit)
            {
                GoCalc();
            }

        }

        private static void GoCalc()
        {
            Console.WriteLine("Enter string input:");
            var input = Console.ReadLine();
            var parserResult = new MathParser().ParseEntire(input);
            Console.WriteLine(parserResult.ToString(CultureInfo.InvariantCulture));
            Console.WriteLine("'A' key for another calculation or any other key to quit...");
            var keyPressed = Console.ReadKey();

            if (keyPressed.Key == ConsoleKey.A)
            {
                Console.Clear();
                return;
            }
            _quit = true;
        }
    }
}
