// Author: Ryan Cobb (@cobbr_io)
// Project: SharpSploit (https://github.com/cobbr/SharpSploit)
// License: BSD 3-Clause

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharpSploit.LateralMovement;

namespace SharpSploit.Tests.LateralMovement
{
    [TestClass]
    public class PowerShellRemotingTests
    {
        [TestMethod]
        public void TestInvokeCommand()
        {
            var result = PowerShellRemoting.InvokeCommand("dc", "whoami; hostname");
            Assert.IsTrue(!string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void TestInvokeCommandWCredentials()
        {
            var result = PowerShellRemoting.InvokeCommand("dc", "whoami; hostname", "DEV", "Administrator", "Passw0rd!");
            Assert.IsTrue(!string.IsNullOrEmpty(result));
        }
    }
}