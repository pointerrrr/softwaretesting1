using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue;

namespace UnitTests_STVRogue
{
    [TestClass]
    public class MSTest_Game
    {
        [TestMethod]
        public void new_game()
        {
            STVRogue.GameLogic.Game game = new STVRogue.GameLogic.Game(2, 2, 6);
        }

        [TestMethod]
        public void update_test()
        {
            STVRogue.GameLogic.Game game = new STVRogue.GameLogic.Game(2, 2, 6);
            Assert.IsTrue(game.update(new Command()));
        }

        [TestMethod]
        [ExpectedException(typeof(STVRogue.GameLogic.GameCreationException))]
        public void game_creation_exception_with_explanation()
        {
            throw new STVRogue.GameLogic.GameCreationException("test");
        }
    }
}
