using CommandLine;
using Mjolnir.ConsoleCommandLine.Tracer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mjolnir.ConsoleCommandLine.Commands
{
    [Verb("Clear")]
    public class ClearCommand : ConsoleCommandBase
    {
        public override async Task<object> ExecuteCommand(ITracingService tracer, object input)
        {
            return await Task.Run(() =>
            {
                Console.Clear();
                return true;
            });
        }
    }
}
