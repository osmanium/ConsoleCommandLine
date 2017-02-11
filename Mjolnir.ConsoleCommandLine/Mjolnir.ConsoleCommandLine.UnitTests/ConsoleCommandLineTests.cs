using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mjolnir.ConsoleCommandLine.UnitTests
{
    [TestClass]
    public class ConsoleCommandLineTests
    {
        [TestMethod]
        public void InitalizeCMD()
        {
            var cmd = ConsoleCommandLine.Instance;

            cmd.Initialize();

            cmd.Run(null);
        }
    }
}
