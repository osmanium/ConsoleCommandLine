using CommandLine;
using Mjolnir.ConsoleCommandLine.Tracer;
using Mjolnir.ConsoleCommandLine.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mjolnir.ConsoleCommandLine
{
    public class ConsoleCommandLine
    {
        private Parser _parser = null;

        public bool IsTypeLoadError { get; private set; } = false;

        //(Command Name - ConsoleCommand - Verb)
        public List<Tuple<string, Type, VerbAttribute>> Commands { get; private set; }

        private static ConsoleCommandLine instance;
        public static ConsoleCommandLine Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConsoleCommandLine();
                }
                return instance;
            }
        }

        public Action HeaderAction { get; set; }

        #region CTOR
        private ConsoleCommandLine()
        {
            //(Command Name - Command Type - ConsoleCommand - Verb)
            Commands = new List<Tuple<string, Type, VerbAttribute>>();

            _parser = new Parser((s) =>
            {
                s.IgnoreUnknownArguments = true;
                s.EnableDashDash = true;
                s.CaseSensitive = false;
                s.HelpWriter = Console.Out;
            });
        }
        #endregion

        #region Public Methods
        public void Initialize(bool isHeaderShow)
        {
            //Iterate commands in current folder, in different assemblies
            IsTypeLoadError = RegisterCommands();

            if (!IsTypeLoadError && isHeaderShow)
                ShowHeader();
        }

        public void ShowHeader()
        {
            if (HeaderAction != null)
            {
                try
                {
                    HeaderAction.Invoke();
                }
                catch (Exception) { /*Surpress exception*/}
            }
        }

        public void ProcessLine(string line)
        {
            var newArgs = StringToArgs.CommandLineToArgs(line);

            ProcessArgs(newArgs);
        }

        public void Run(string[] args)
        {
            if (args == null || !args.Any())
            {
                do
                {
                    Console.Write(DateTime.Now.ToShortTimeString() + " > ");
                    var line = Console.ReadLine().Trim();

                    if (line.ToLowerInvariant() != "exit")
                        Instance.ProcessLine(line.Trim());
                    else
                        break;
                } while (true);
            }
            else
            {
                Instance.ProcessArgs(args);
            }
        }
        #endregion

        #region Private Methods
        private static List<Type> FindAllDerivedTypes<T>(Assembly assembly)
        {
            var derivedType = typeof(T);
            return assembly
                .GetTypes()
                .Where(t =>
                    derivedType.IsAssignableFrom(t)
                    ).ToList();
        }

        private void ProcessArgs(string[] args)
        {
            var tracer = new ConsoleTracer();

            string commandText = string.Empty;
            
            //Find the mathcing command through attributes
            var commandKey = Commands.Where(w => w.Item1.ToLowerInvariant() == args.FirstOrDefault().ToLowerInvariant())
                                     .FirstOrDefault();

            if (commandKey != null && !string.IsNullOrWhiteSpace(commandKey.Item1))
                commandText = commandKey.Item1;
            else
            {
                tracer.Trace("Command not found. To list the commands use 'List-Commands'.");
                return;
            }


            var parseResult = _parser.ParseArguments(args, Commands.Select(s => s.Item2).ToArray());

            if (parseResult is Parsed<object>)
            {
                //Parsed
                var parsedResult = parseResult as Parsed<object>;
                
                var parsedCommand = parsedResult.Value as ConsoleCommandBase;

                //Execute the command
                parsedCommand.ExecuteCommand(tracer, new Nothing());
            }
            else
            {
                //Error Parsed

                var errorParseResult = parseResult as NotParsed<object>;

                tracer.Trace($"Error in parsing command line,");

                foreach (var error in errorParseResult.Errors)
                {
                    tracer.Trace($"{error}");
                }

                return;
            }
        }

        private bool RegisterCommands()
        {
            List<Assembly> allAssemblies = new List<Assembly>();
            bool isError = false;

            isError = LoadAssemblies(allAssemblies);

            foreach (var assembly in allAssemblies)
            {
                try
                {
                    var types = FindAllDerivedTypes<ConsoleCommandBase>(assembly)
                                        .Where(w => w.GetCustomAttribute<VerbAttribute>() != null);

                    foreach (var type in types)
                    {
                        var verbAttrValue = type.GetCustomAttribute<VerbAttribute>();

                        //(Command Name - Command Type - ConsoleCommand - Verb)
                        Commands.Add(new Tuple<string, Type, VerbAttribute>(verbAttrValue.Name, type, verbAttrValue));
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine($"Assembly Load Error for {assembly.FullName} : {ex.Message}");
                    isError = true;
                }
            }
            return isError;
        }

        private static bool LoadAssemblies(List<Assembly> allAssemblies)
        {
            var commandsPathConfig = ConfigurationManager.AppSettings[Constants.CommandsFolderConfigKey];
            if (commandsPathConfig == null)
            {
                Console.WriteLine($"{Constants.CommandsFolderConfigKey} is missing in config.");
                return true;
            }

            var includedAssembliesConfig = ConfigurationManager.AppSettings[Constants.CommandAssembliesKey];
            if (includedAssembliesConfig == null)
            {
                Console.WriteLine($"{Constants.CommandAssembliesKey} is missing in config.");
                return true;
            }


            var commandAssembliesList = includedAssembliesConfig.Split(';');

            string commandsFolderPath = string.Empty;


            if (Path.IsPathRooted(commandsPathConfig))
                commandsFolderPath = commandsPathConfig;
            else
                commandsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), commandsPathConfig);



            foreach (string dll in Directory.GetFiles(commandsFolderPath, "*.dll"))
                if (commandAssembliesList.Contains(Path.GetFileName(dll)))
                {
                    allAssemblies.Add(Assembly.LoadFile(dll));
                    Console.WriteLine($"{Path.GetFileName(dll)} is loaded..");
                }

            foreach (string exe in Directory.GetFiles(commandsFolderPath, "*.exe"))
                if (commandAssembliesList.Contains(Path.GetFileName(exe)))
                {
                    allAssemblies.Add(Assembly.LoadFile(exe));
                    Console.WriteLine($"{Path.GetFileName(exe)} is loaded..");
                }

            var mjolnirCommandLineDllPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Mjolnir.ConsoleCommandLine.dll");
            allAssemblies.Add(Assembly.LoadFile(mjolnirCommandLineDllPath));
            Console.WriteLine($"{Path.GetFileName(mjolnirCommandLineDllPath)} is loaded..");

            allAssemblies.Add(Assembly.LoadFile(Assembly.GetEntryAssembly().Location));
            Console.WriteLine($"{Path.GetFileName(Assembly.GetEntryAssembly().Location)} is loaded..");

            return false;
        }
        #endregion

    }
}
