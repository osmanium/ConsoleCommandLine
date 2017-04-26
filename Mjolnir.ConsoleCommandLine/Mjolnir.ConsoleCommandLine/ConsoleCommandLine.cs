using Microsoft.Xrm.Sdk;
using Mjolnir.ConsoleCommandLine.Tracer;
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
        private Dictionary<string, string> Parameters = null;


        public bool IsTypeLoadError { get; private set; } = false;

        public List<Tuple<string, ConsoleCommandAttribute, Type>> Commands { get; private set; }

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
            Commands = new List<Tuple<string, ConsoleCommandAttribute, Type>>();
        }
        #endregion

        #region Publis Methods
        public void Initialize()
        {
            //Iterate commands in current folder, in different assemblies
            IsTypeLoadError = RegisterCommands();

            if (!IsTypeLoadError)
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
            ProcessArgs(line.Split(' '));
        }

        public void Run(string[] args)
        {
            if (args == null || !args.Any())
            {
                do
                {
                    Console.Write(DateTime.Now.ToShortTimeString() + " > ");
                    var line = Console.ReadLine().Trim();

                    //var line = CommandAutoComplete.ReadHintedLine(Commands, command => command.Item1);

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

            Parse(args);

            string commandText = string.Empty;

            //Find the mathcing command through attributes
            var commandKey = Commands.Where(w => w.Item1.ToLowerInvariant() == args.FirstOrDefault().ToLowerInvariant())
                                     .FirstOrDefault();

            if (commandKey != null && !string.IsNullOrWhiteSpace(commandKey.Item1))
                commandText = commandKey.Item1;
            else
            {
                tracer.Trace("Command not found. To list the commands use 'ListCommands'.");
                return;
            }



            //Execute the command
            ExecuteCommand(commandText, tracer, new Nothing());
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
                                        .Where(w => w.GetCustomAttribute<ConsoleCommandAttribute>() != null);

                    foreach (var type in types)
                    {
                        var attributeValue = type.GetCustomAttribute<ConsoleCommandAttribute>();
                        Commands.Add(new Tuple<string, ConsoleCommandAttribute, Type>(attributeValue.Command, attributeValue, type));
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

            return false;
        }

        private object ExecuteCommand(string commandText, ITracingService tracer, object input)
        {
            var command = Commands.FirstOrDefault(w => w.Item1.ToLowerInvariant() == commandText.ToLowerInvariant());


            if (command.Item2.DependentCommand != null)
            {
                var dependentConsoleCommandAttribute = command.Item2.DependentCommand.GetCustomAttribute<ConsoleCommandAttribute>();

                //TODO : Validate inputs

                input = ExecuteCommand(dependentConsoleCommandAttribute.Command, tracer, input);
            }

            dynamic commandDependentTypeInstance = Activator.CreateInstance(command.Item3);

            //TODO : Fill the similar properties with parameters
            var properties = command.Item3.GetProperties().Where(w => w.GetCustomAttribute<CommandLineAttributeBase>() != null);

            foreach (var property in properties)
            {
                var parameter = Parameters[property.Name];

                if (!string.IsNullOrWhiteSpace(parameter))
                {
                    try
                    {
                        //TODO : Based on property type, cast the value, ; StringInputAttribute - IntInputAttribute
                        property.SetValue(commandDependentTypeInstance, parameter);
                    }
                    catch (Exception ex)
                    {
                        //
                    }
                }
            }

            //TODO : Make tracer based on app.config
            return commandDependentTypeInstance.ExecuteCommand(tracer, input);
        }

        private Dictionary<string, string> Parse(string[] args)
        {
            Parameters = new Dictionary<string, string>();

            int i = 0;

            while (i < args.Length)
            {
                if (args[i].Length > 1 && args[i][0] == '-')
                {
                    // The current string is a parameter name
                    string key = args[i].Substring(1, args[i].Length - 1);
                    string value = "";
                    i++;
                    if (i < args.Length)
                    {
                        if (args[i].Length > 0 && args[i][0] == '-')
                        {
                            // The next string is a new parameter, do not nothing
                        }
                        else
                        {
                            // The next string is a value, read the value and move forward
                            value = args[i];
                            i++;
                        }
                    }

                    Parameters[key] = value;
                }
                else
                {
                    i++;
                }
            }

            return Parameters;
        }
        #endregion

    }
}
