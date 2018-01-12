using CommandLine;
using Mjolnir.ConsoleCommandLine.Tracer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mjolnir.ConsoleCommandLine
{
    public abstract class ConsoleCommandBase
    {
        [Option('v', "verbose", Default = false)]
        public bool Verbose { get; set; }

        public abstract object ExecuteCommand(ITracingService tracer, object input);

        public virtual string GetHelp()
        {
            //TODO : Iterate properties with types
            return null;
        }

        public void HandleCommandException(ITracingService tracer, Exception ex)
        {
            tracer.Trace(ex.Message + "\n" + ex.StackTrace + "\n");
        }
    }
}
