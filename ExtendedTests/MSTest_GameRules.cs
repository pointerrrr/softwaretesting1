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
        List<ReplayReader> replays = new List<ReplayReader>();
        string[] files = new string[] { "NOCombat_EscGame.txt", "NOCombat_ReachEnd.txt", "Combat_Kill_Die_UseItems.txt" };

        public void InitReplays()
        {
            foreach (string file in files)
                replays.Add(new ReplayReader(Environment.CurrentDirectory + "../../../../replays/" + file));
        }

        [TestMethod]
        public void Test_GameRules()
        {
            InitReplays();
            foreach (ReplayReader reader in replays)
            {
                Assert.IsTrue(reader.replay(new Always(RZone)));
                Assert.IsTrue(reader.replay(new Always(RNode)));
                Assert.IsTrue(reader.replay(new Unless(NotContested, MovedTowards)));
                Assert.IsTrue(reader.replay(new Unless(NotInLastZone, ForcedMove)));
            }
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
                        if (pack.previousLocation.Key != null && pack.previousLocation.Key != pack.location && game.player.location != pack.previousLocation.Key)
                        {
                            List<Node> path1 = game.dungeon.shortestpath(pack.previousLocation.Key, game.player.location);
                            List<Node> path2 = game.dungeon.shortestpath(pack.location, game.player.location);
                            if (path1.Count <= path2.Count)
                                return false;
                        }
                    }
            return true;
        }

        static bool NotContested(Game game)
        {
            Predicates pred = new Predicates();
            List<Node> allNodes = pred.reachableNodes(game.dungeon.startNode);
            List<Pack> allPacks = new List<Pack>();
            foreach (Node node in allNodes)
                allPacks.AddRange(node.packs);
            foreach (Pack pack in allPacks)
                if (pack.previousLocation.Key == pack.location && pack.location == game.player.location)
                    return false;
            return true;
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
                            List<Node> path = game.dungeon.shortestpath(pack.previousLocation.Key, game.player.previousLocation);
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
