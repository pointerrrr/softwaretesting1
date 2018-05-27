using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    [TestClass]
    public class MSTest_Dungeon
    {
        Dungeon dungeon = new Dungeon(1, 1, 0);
        Predicates utils = new Predicates();

        [TestMethod]
        public void MSTest_shortest_path()
        {
            Node n0 = new Node("N0");
            Node n1 = new Node("N1");
            Node n2 = new Node("N2");
            Node n3 = new Node("N3");
            Node n4 = new Node("N4");
            Node n5 = new Node("N5");
            Node n6 = new Node("N6");

            // Shortest path is n0, n2, n6.

            // n0--n1--n3
            // |       |
            // n2---   |
            // |   |   |
            // n5--n6--n4

            n0.connect(n2);
            n2.connect(n5);
            n2.connect(n6);
            n5.connect(n6);

            n0.connect(n1);
            n1.connect(n3);
            n3.connect(n4);
            n4.connect(n6);

            List<Node> shortest_path = new List<Node>{ n2, n6 };

            List<Node> result_path = dungeon.shortestpath(n0, n6);

            CollectionAssert.AreEqual(shortest_path, result_path);
        }

        [TestMethod]
        public void MSTest_calculate_connectivity()
        {
            // n4--n5      n9
            // |   |       |
            // n0--n1--n2--n3
            // |   |
            // n6  n7--n8

            Node n0 = new Node("N0");
            Node n1 = new Node("N1");
            Node n2 = new Node("N2");
            Node n3 = new Node("N3");
            Node n4 = new Node("N4");
            Node n5 = new Node("N5");
            Node n6 = new Node("N6");
            Node n7 = new Node("N7");
            Node n8 = new Node("N8");
            Node n9 = new Node("N9");

            n0.connect(n1);
            n0.connect(n4);
            n0.connect(n6);
            n1.connect(n2);
            n1.connect(n5);
            n1.connect(n7);
            n2.connect(n3);
            n3.connect(n9);
            n4.connect(n6);
            n7.connect(n8);

            List<Node> nodes = new List<Node>{ n0, n1, n2, n3, n4, n5, n6, n7, n8, n9 };
            double expected_conn = 20 / 9;
            double actual_conn = dungeon.calculateConnectivity(nodes);

            Assert.IsTrue(expected_conn == actual_conn);
        }

        [TestMethod]
        public void MSTest_level()
        {
            Dungeon testDungeon = new Dungeon(1, 1, 0);

            // start--b1--exit
            //   |    |
            //   ----n0

            Node start = new Node("start");
            Node exit = new Node("exit");
            Bridge b1 = new Bridge("1");
            Node n0 = new Node("N0");

            testDungeon.startNode = start;
            testDungeon.exitNode = exit;

            b1.connectToNodeOfSameZone(start);
            b1.connectToNodeOfNextZone(exit);
            b1.connectToNodeOfSameZone(n0);
            start.connect(n0);

            uint levelBridge = testDungeon.level(b1);
            uint levelNode = testDungeon.level(n0);

            Assert.IsTrue(levelBridge == 1);
            Assert.IsTrue(levelNode == 0);
        }

        [TestMethod]
        public void MSTest_bridge_disconnect()
        {
            Dungeon testDungeon = new Dungeon(1, 1, 0);

            //        n0--n1--    n3--n4--   n6--n7--
            //        |       |   |      |   |      |
            // start--|--n2---b1--|--n5--b2--|--n8--exit

            Node start = new Node("start");
            Node exit = new Node("exit");
            Node n0 = new Node("N0");
            Node n1 = new Node("N1");
            Node n2 = new Node("N2");
            Node n3 = new Node("N3");
            Node n4 = new Node("N4");
            Node n5 = new Node("N5");
            Node n6 = new Node("N6");
            Node n7 = new Node("N7");
            Node n8 = new Node("N8");

            Bridge b1 = new Bridge("1");
            Bridge b2 = new Bridge("2");

            start.connect(n0);
            start.connect(n2);
            n0.connect(n1);
            b1.connectToNodeOfSameZone(n1);
            b1.connectToNodeOfSameZone(n2);
            b1.connectToNodeOfNextZone(n3);
            b1.connectToNodeOfNextZone(n5);
            n3.connect(n4);
            b2.connectToNodeOfSameZone(n4);
            b2.connectToNodeOfSameZone(n5);
            b2.connectToNodeOfNextZone(n6);
            b2.connectToNodeOfNextZone(n8);
            n6.connect(n7);
            n7.connect(exit);
            n8.connect(n8);

            testDungeon.startNode = start;
            testDungeon.exitNode = exit;

            testDungeon.disconnect(b1);

            List<Node> reachable = utils.reachableNodes(b1);

            List<Node> notConnected = new List<Node>{ start, n0, n1, n2 };
            List<Node> connected = new List<Node> { n3, n4, n5, b2, n6, n7, n7, n8, exit };

            Assert.IsTrue(testDungeon.startNode == b1);
            foreach (Node node in notConnected)
                Assert.IsFalse(reachable.Contains(node));
            foreach (Node node in connected)
                Assert.IsTrue(reachable.Contains(node));
        }

        [TestMethod]
        public void MSTest_population_possible()
        {
            Dungeon testDungeon = new Dungeon(2, 2, 6);
            Queue<Node> queue = new Queue<Node>();
            List<Node> passed = new List<Node>();
            Predicates predicates = new Predicates();
            int[] zoneMonsterCount = new int[] { 0, 0, 0 };
            // BFS using a queue
            queue.Enqueue(testDungeon.startNode);
            passed.Add(testDungeon.startNode);
            while (queue.Count != 0)
            {
                Node currentNode = queue.Dequeue();
                foreach (Node neighbor in currentNode.neighbors)
                {
                    if (!passed.Contains(neighbor))
                    {
                        passed.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }

                }

                foreach (Pack pack in currentNode.packs)
                    zoneMonsterCount[currentNode.zoneId - 1] += pack.members.Count;
            }

            Assert.IsTrue(zoneMonsterCount[0] == 1);
            Assert.IsTrue(zoneMonsterCount[1] == 2);
            Assert.IsTrue(zoneMonsterCount[2] == 3);
        }

        [TestMethod]
        [ExpectedException(typeof(GameCreationException))]
        public void MSTest_population_impossible()
        {
            Dungeon testDungeon = new Dungeon(2, 2, 80);
        }

        [TestMethod]
        public void MSTest_valid_dungeon_check_valid_dungeon()
        {
            Dungeon.TestDungeon(dungeon.startNode, dungeon.exitNode, dungeon.difficultyLevel);
        }

        [TestMethod]
        [ExpectedException(typeof(GameCreationException))]
        public void MSTest_valid_dungeon_check_invalid_dungeon()
        {
            Dungeon invalidDungeon = dungeon;
            dungeon.startNode.connect(new Node());
            dungeon.startNode.connect(new Node());
            dungeon.startNode.connect(new Node());
            Dungeon.TestDungeon(invalidDungeon.startNode, invalidDungeon.exitNode, invalidDungeon.difficultyLevel);
        }
    }
}
