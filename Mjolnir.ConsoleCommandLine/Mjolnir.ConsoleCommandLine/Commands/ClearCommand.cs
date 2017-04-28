using CommandLine;
using Microsoft.Xrm.Sdk;
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
        public override object ExecuteCommand(ITracingService tracer, object input)
        {
            Console.Clear();
            return true;
        }
    }
}
