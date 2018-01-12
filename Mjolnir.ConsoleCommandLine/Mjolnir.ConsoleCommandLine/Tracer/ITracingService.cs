using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mjolnir.ConsoleCommandLine.Tracer
{
    public interface ITracingService
    {
        void Trace(string format, params object[] args);
    }
}
