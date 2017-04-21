using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mjolnir.ConsoleCommandLine.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            var commandLine = ConsoleCommandLine.Instance;
            commandLine.HeaderAction = () =>
            {
                Console.WriteLine("==========================================================================");
                Console.WriteLine(@"TEST CONSOLE");
                Console.WriteLine("==========================================================================");
                Console.WriteLine("");
            };


            commandLine.Initialize();

            if (!commandLine.IsTypeLoadError)
                commandLine.Run(args);
            else Console.ReadKey();
        }
    }
}
