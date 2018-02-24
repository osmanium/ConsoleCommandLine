using CommandLine;
using Mjolnir.ConsoleCommandLine.Tracer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mjolnir.ConsoleCommandLine.NoNuget
{
    [Verb("Clear2")]
    public class ClearCommand2 : ConsoleCommandBase
    {
        /// <inheritdoc />
        public override async Task<object> ExecuteCommand(ITracingService tracer, object input)
        {
            return Task.Run(() =>
            {
                Console.Clear();
                return true;
            });
        }
    }
}
