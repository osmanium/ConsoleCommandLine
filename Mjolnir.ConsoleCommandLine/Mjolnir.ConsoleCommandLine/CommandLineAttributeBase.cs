using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mjolnir.ConsoleCommandLine
{
    public class CommandLineAttributeBase : Attribute
    {
        public bool IsRequired { get; set; }
        public string Description { get; set; }
    }
}
