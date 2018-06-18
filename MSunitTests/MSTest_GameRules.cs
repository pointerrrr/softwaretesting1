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
            Assert.IsTrue(reader.replay(new Always(RZone)));
            Assert.IsTrue(reader.replay(new Always(RNode)));
            Assert.IsTrue(reader.replay(new Unless(NotContested, MovedTowards)));
            Assert.IsTrue(reader.replay(new Unless(NotInLastZone, ForcedMove)));
            reader = new ReplayReader(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/test.txt");
            Assert.IsTrue(reader.replay(new Always(RZone)));
            Assert.IsTrue(reader.replay(new Always(RNode)));
            Assert.IsTrue(reader.replay(new Unless(NotContested, MovedTowards)));
            Assert.IsTrue(reader.replay(new Unless(NotInLastZone, ForcedMove)));
        }

        static bool RZone(Game game)
        {
            Predicates pred = new Predicates();
            List<Node> allNodes = pred.reachableNodes(game.dungeon.startNode);
            foreach (Node node in allNodes)
            {
                foreach (Pack pack in node.packs)
                    if (pack.previousLocation.Key != null && pack.location.zoneId != pack.previousLocation.Key.zoneId)
                        return false;
            }
            return true;
        }

        static bool RNode(Game game)
        {
            Predicates pred = new Predicates();
            List<Node> allNodes = pred.reachableNodes(game.dungeon.startNode);
            foreach (Node node in allNodes)
            {
                int capacity = (int)(game.dungeon.M * (game.dungeon.level(node) + 1));
                int count = 0;
                foreach (Pack pack in node.packs)
                    count += pack.members.Count;
                if (count > capacity)
                    return false;
            }
            return true;
        }

        static bool MovedTowards(Game game)
        {
            Predicates pred = new Predicates();
            List<Node> allNodes = pred.reachableNodes(game.dungeon.startNode);
            foreach (Node node in allNodes)
                foreach (Pack pack in node.packs)
                    if (pack.location.zoneId == game.player.location.zoneId)
                    {
                        if (pack.previousLocation.Key != null && pack.previousLocation.Key != pack.location)
                        {
                            List<Node> path = game.dungeon.shortestpath(pack.previousLocation.Key, game.player.location);
                            if (path.First() != pack.location && pack.previousLocation.Key != game.player.location)
                                return false;
                        }
                    }
            return true;
        }

        static bool NotContested(Game game)
        {
            return !game.player.location.contested(game.player);
        }


        static bool NotInLastZone(Game game)
        {
            return game.player.location.zoneId != game.dungeon.difficultyLevel + 1;
        }

        static bool ForcedMove(Game game)
        {
            Predicates pred = new Predicates();
            List<Node> allNodes = pred.reachableNodes(game.dungeon.startNode);
            foreach (Node node in allNodes)
                foreach (Pack pack in node.packs)
                    if (pack.location.zoneId == game.player.location.zoneId)
                    {
                        if (pack.previousLocation.Key != null && pack.previousLocation.Key != pack.location)
                        {
                            List<Node> path = game.dungeon.shortestpath(pack.previousLocation.Key, game.player.location);
                            if (path.First() != pack.location && pack.previousLocation.Key != game.player.location)
                                return false;
                        }
                        else if(pack.previousLocation.Key != null && pack.previousLocation.Key == pack.location && pack.previousLocation.Value != 1 && pack.location != game.player.location)
                        {
                            return false;
                        }
                    }
            return true;
        }
    }
}
