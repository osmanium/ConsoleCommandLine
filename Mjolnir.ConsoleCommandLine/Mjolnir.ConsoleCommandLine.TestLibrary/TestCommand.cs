using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mjolnir.ConsoleCommandLine.TestLibrary
{
    [ConsoleCommandAttribute(
       Command = "TestCommand",
       Desription = "")]
    public class TestCommand : ConsoleCommandBase
    {
        public override object ExecuteCommand(ITracingService tracer, object input)
        {
            Console.WriteLine("Hello from test library!");
            return true;
        }
    }
}
