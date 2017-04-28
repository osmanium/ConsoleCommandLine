using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mjolnir.ConsoleCommandLine.UnitTests
{
    [TestClass]
    public class ConsoleCommandLineTestBase
    {
        [TestInitialize]
        public void InitalizeCMD()
        {
            var cmd = ConsoleCommandLine.Instance;

            cmd.Initialize(false);
        }
    }
}
