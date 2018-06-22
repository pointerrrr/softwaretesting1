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

        // Make sure the replay system works by checking if the NOCombat_ReachEnd replay indeed reaches the end
        [TestMethod]
        public void Test_ReplaySystem()
        {
            InitReplays();
            Assert.IsFalse(replays[1].replay(new Always(a => a.player.location != a.dungeon.exitNode)));
        }

        // Properties of entities in our logic that might not speak for itself
        // Entity.PreviousLocation = location at start of turn
        // Entity.Location = location at end of turn
        // Entity.StartLocation = location at start of game
        // Methods in our Logic system that might not speak for itself
        // CannotMove(Entity, Node) = cannot move the Entity to the Node

        // always
        // (G => (∀x : x ∈ G.Dungeon.AllPacks : x.Location.ZoneID = x.StartLocation.ZoneID))
        [TestMethod]
        public void Test_RZone()
        {
            InitReplays();
            foreach (ReplayReader reader in replays)
                Assert.IsTrue(reader.replay(new Always(RZone)));            
        }

        // always
        // (G => (∀x : x ∈ G.Dungeon.AllNodes : x.MonsterCount <= x.MaxCapacity))
        [TestMethod]
        public void Test_RNode()
        {
            InitReplays();
            foreach (ReplayReader reader in replays)
                Assert.IsTrue(reader.replay(new Always(RNode)));
        }

        // (G => ¬Contested(G.Player.Location))
        // unless
        // (G => (∀x : x ∈ G.Dungeon.AllPacks : ShortestPath(x.PreviousLocation, G.Player.Location) ≤ ShortestPath(x.Location, G.Player.Location))
        [TestMethod]
        public void Test_RAlert()
        {
            InitReplays();
            foreach (ReplayReader reader in replays)
                Assert.IsTrue(reader.replay(new Unless(NotContested, MovedTowards)));
        }

        // (G => G.Player.Location.ZoneID <> G.Dungeon.DifficultyLevel + 1)
        // unless
        // (G => (∀x : x ∈ G.Dungeon.AllPacks ∧ (x.Location.ZoneID = G.Player.Location.ZoneID) : (ShortestPath(x.PreviousLocation, G.Player.Location).Length < ShortestPath(x.Location, G.Player.Location).Length) ∨ CannotMove(x, ShortestPath(x.PreviousLocation, x.Player.Location)[0]))
        [TestMethod]
        public void Test_REndZone()
        {
            InitReplays();
            foreach (ReplayReader reader in replays)
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
                        if (pack.previousLocation.Key != null && pack.previousLocation.Key != pack.location && game.player.previousLocation != pack.previousLocation.Key)
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
                if (pack.previousLocation.Key == pack.location && pack.location == game.player.location && pack.location.contested(game.player))
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
                {
                    if (pack.location.zoneId == game.dungeon.difficultyLevel + 1 && pack.previousLocation.Key != game.player.previousLocation && pack.previousLocation.Key != null && pack.location.zoneId == game.player.location.zoneId)
                    {
                        
                        if (pack.previousLocation.Key != pack.location && game.player.previousLocation != pack.previousLocation.Key)
                        {
                            List<Node> path1 = game.dungeon.shortestpath(pack.previousLocation.Key, game.player.location);
                            List<Node> path2 = game.dungeon.shortestpath(pack.location, game.player.location);
                            if (path1.Count <= path2.Count)
                                return false;
                        }
                        else if (pack.previousLocation.Key == pack.location && pack.previousLocation.Value != 1)
                            return false;
                    }
                    
                }
            return true;
        }
    }
}
