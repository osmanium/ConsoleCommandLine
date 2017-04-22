using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mjolnir.ConsoleCommandLine.NoNuget
{
    [ConsoleCommandAttribute(
        Command = "Clear2",
        Desription = "")]
    public class ClearCommand2 : ConsoleCommandBase
    {
        public override object ExecuteCommand(ITracingService tracer, object input)
        {
            Console.Clear();
            return true;
        }
    }
}
