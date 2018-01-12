using CommandLine;
using Mjolnir.ConsoleCommandLine.Tracer;
using Mjolnir.ConsoleCommandLine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mjolnir.ConsoleCommandLine.Commands
{
    [Verb("List-Commands")]
    public class ListCommandsCommand : ConsoleCommandBase
    {
        public override object ExecuteCommand(ITracingService tracer, object input)
        {
            var commands = ConsoleCommandLine.Instance.Commands;

            var table = new TableToConsole("Command", "Type");

            foreach (var command in commands)
            {
                table.AddRow(command.Item1, command.Item2.FullName);
            }

            table.Write(Format.MarkDown);

            return true;
        }
    }
}
