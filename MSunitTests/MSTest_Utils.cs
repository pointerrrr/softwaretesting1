using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.GameLogic;
using STVRogue.Utils;

namespace UnitTests_STVRogue
{
    [TestClass]
    public class MSTest_Utils
    {
        [TestMethod]
        public void MSTest_new_rng()
        {
            RandomGenerator.clearRNG();
            Random rng = RandomGenerator.rnd;
            Assert.IsNotNull(rng);
        }

        [TestMethod]
        public void MSTest_new_rng_with_seed()
        {
            RandomGenerator.initializeWithSeed(1337);
            Random rng = RandomGenerator.rnd;
            int next = rng.Next();
            // 448584296 is the first number Random generates with seed 1337
            Assert.AreEqual(next, 448584296);
        }

        [TestMethod]
        public void MSTest_logger()
        {
            StringWriter output = new StringWriter();
            Console.SetOut(output);            
            Logger.log("test");
            string consoleOutput = output.ToString();
            Assert.AreEqual("** test\r\n", consoleOutput);
        }
    }
}
