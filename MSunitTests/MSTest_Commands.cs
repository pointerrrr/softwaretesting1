using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue;

namespace UnitTests_STVRogue
{
    [TestClass]
    public class MSTest_Commands
    {
        [TestMethod]
        public void MSTest_constructor()
        {
            Command cmd = new Command();
            Assert.IsNotNull(cmd);
        }

        [TestMethod]
        public void MSTest_cmd_to_string()
        {
            Command cmd = new Command();
            Assert.AreEqual("no-action", cmd.ToString());
        }

    }
}
