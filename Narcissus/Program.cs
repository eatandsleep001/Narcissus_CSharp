using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NarcissusNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = @"Narcissus";

            if (args.Length == 0)
                DoWithoutArgs();
        }

        static void DoWithoutArgs()
        {
            string url = null;
            int totalView = 0;
            Narcissus narcissus = null;

            do
            {
                Console.Write("Url: ");
                url = Console.ReadLine();
            } while (!Regex.IsMatch(url, @"http://anime47.com/phim/(.*?).html"));

            do
            {
                Console.Write("Total view: ");
            }
            while (!int.TryParse(Console.ReadLine(), out totalView));

            narcissus = new Narcissus(url, totalView);
            narcissus.Run();
        }
    }
}
