using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                //Console.Clear();
                Console.WriteLine("==========================================================================");
                Console.WriteLine(@"TEST CONSOLE");
                Console.WriteLine("==========================================================================");
                Console.WriteLine("");
            };


            commandLine.Initialize();

            //if (!commandLine.IsTypeLoadError)
            commandLine.Run(args);
            //else Console.ReadKey();

            Console.ReadKey();

            //Assembly testCommandsAssembly = Assembly.LoadFrom(@"C:\Prj\GitHub\Mjolnir.ConsoleCommandLine\Mjolnir.ConsoleCommandLine\Mjolnir.ConsoleCommandLine.TestConsole\bin\Debug\Commands\Mjolnir.ConsoleCommandLine.TestLibrary.dll");
            //var commands1 = testCommandsAssembly.GetTypes().Where(w => w.BaseType == typeof(ConsoleCommandBase));

            //var commands2 = testCommandsAssembly.GetTypes().Where(w => w.IsAssignableFrom(typeof(ConsoleCommandBase)));

            //var commands3 = testCommandsAssembly.GetTypes().Where(w => typeof(ConsoleCommandBase).IsAssignableFrom(w));

            //var commands4 = commands3.Where(w => w.GetCustomAttribute<ConsoleCommandAttribute>() != null);

        }
    }
}
