using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.Utils;
using STVRogue.GameLogic;

namespace STVRogue.GameLogic
{
    [TestClass]
    public class MSTest_GameRules
    {
        [TestMethod]
        public void MSTest_RZone()
        {
            ReplayReader reader = new ReplayReader(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/default.txt");
            Assert.IsTrue(reader.replay(new Specification(RZone)));
        }

        static bool RZone(Game game)
        {
            Predicates pred = new Predicates();
            List<Node> allNodes = pred.reachableNodes(game.dungeon.startNode);
            foreach (Node node in allNodes)
            {
                foreach (Pack pack in node.packs)
                    if (!(pack.id.Split('.')[0] == node.zoneId.ToString()))
                        return false;
            }
            return true;
        }
    }
}
