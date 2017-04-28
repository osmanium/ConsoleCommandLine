using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Mjolnir.ConsoleCommandLine.NoNuget
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            var commandLine = ConsoleCommandLine.Instance;
            commandLine.HeaderAction = () =>
            {
                //Console.Clear();
                Console.WriteLine("==========================================================================");
                Console.WriteLine(@"TEST CONSOLE NO NUGET");
                Console.WriteLine("==========================================================================");
                Console.WriteLine("");
            };


            commandLine.Initialize(args != null && !args.Any());


            commandLine.Run(args);
        }
    }
}
