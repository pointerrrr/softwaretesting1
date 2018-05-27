using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue;
using STVRogue.GameLogic;

namespace UnitTests_STVRogue
{
    [TestClass]
    public class MSTest_Game
    {
        [TestMethod]
        public void MSTest_new_game()
        {
            Game game = new Game(2, 2, 6);
        }

        [TestMethod]
        public void MSTest_update()
        {
            Game game = new Game(2, 2, 6);
            Assert.IsTrue(game.update(new Command()));
        }

        [TestMethod]
        [ExpectedException(typeof(GameCreationException))]
        public void MSTest_game_creation_exception_with_explanation()
        {
            throw new GameCreationException("test");
        }
    }
}
