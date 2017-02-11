using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mjolnir.ConsoleCommandLine.UnitTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mjolnir.ConsoleCommandLine.UnitTests
{
    [TestClass]
    public class ConsoleCommandLineTests : ConsoleCommandLineTestBase
    {
        [TestMethod]
        public void ClearTest()
        {
            var cmd = ConsoleCommandLine.Instance;
            cmd.ProcessLine("clear");
        }
    }
}
