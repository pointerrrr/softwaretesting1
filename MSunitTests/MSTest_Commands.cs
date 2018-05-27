using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.Utils;

namespace UnitTests_STVRogue
{
    [TestClass]
    public class MSTest_Commands
    {
        [TestMethod]
        public void test_constructor()
        {
            STVRogue.Command cmd = new STVRogue.Command();
            Assert.IsNotNull(cmd);
        }

        [TestMethod]
        public void test_cmd_to_string()
        {
            STVRogue.Command cmd = new STVRogue.Command();
            Assert.AreEqual("no-action", cmd.ToString());
        }

    }
}
