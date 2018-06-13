using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests_STVRogue
{
    [TestClass]
    public class MSTest_Commands
    {
        [TestMethod]
        public void MSTest_constructor()
        {
            STVRogue.Command cmd = new STVRogue.NoActionCommand();
            Assert.IsNotNull(cmd);
        }

        [TestMethod]
        public void MSTest_cmd_to_string()
        {
            STVRogue.Command cmd = new STVRogue.NoActionCommand();
            Assert.AreEqual("no-action", cmd.ToString());
        }

    }
}
