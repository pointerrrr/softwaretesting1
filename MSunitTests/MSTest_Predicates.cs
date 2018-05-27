using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.GameLogic;
using System.Collections.Generic;

namespace STVRogue.Utils
{
    [TestClass]
    public class MSTest_Predicates
    {
        [TestMethod]
        public void MSTest_reachableNodes()
        {
            Node N0 = new Node("N0");
            Node N1 = new Node("N1");
            Node N2 = new Node("N2");
            Node N3 = new Node("N3");
            Node N4 = new Node("N4");
            N0.connect(N1);
            N1.connect(N2);
            N1.connect(N3);
            N3.connect(N2);

            Predicates utils = new Predicates();
            List<Node> V = utils.reachableNodes(N0);
            foreach (Node O in V) Console.WriteLine("** " + O.id);
            Assert.IsTrue(V.Contains(N0));
            Assert.IsTrue(V.Contains(N1));
            Assert.IsTrue(V.Contains(N2));
            Assert.IsTrue(V.Contains(N3));
            Assert.IsFalse(V.Contains(N4));
        }

        [TestMethod]
        public void MSTest_EndReachable()
        {
            Dungeon dungeon = new Dungeon(4, 9, 0);
            
            Predicates utils = new Predicates();
            bool endFound = utils.isReachable(dungeon.startNode, dungeon.exitNode);
            Assert.IsTrue(endFound);
        }

        [TestMethod]
        public void MSTest_is_path_length_one()
        {
            List<Node> nodeList = new List<Node>();
            Node node1 = new Node("1");
            nodeList.Add(node1);
            Predicates utils = new Predicates();
            Assert.IsTrue(utils.isPath(nodeList));
            
        }

        [TestMethod]
        public void MSTest_is_path_valid_path()
        {
            List<Node> nodeList = new List<Node>();
            Node node1 = new Node("1");
            Node node2 = new Node("2");
            node1.connect(node2);
            nodeList.Add(node1);
            nodeList.Add(node2);

            Predicates utils = new Predicates();
            Assert.IsTrue(utils.isPath(nodeList));
        }

        [TestMethod]
        public void MSTest_is_path_invalid_path()
        {
            List<Node> nodeList = new List<Node>();
            Node node1 = new Node("1");
            Node node2 = new Node("2");
            nodeList.Add(node1);
            nodeList.Add(node2);

            Predicates utils = new Predicates();
            Assert.IsFalse(utils.isPath(nodeList));
        }

        [TestMethod]
        public void MSTest_invalid_dungeon_start_is_bridge()
        {
            Predicates pred = new Predicates();
            
            Node nodeStart = new Bridge("start");
            Node nodeExit = new Node("exit");

            Assert.IsFalse(pred.isValidDungeon(nodeStart, nodeExit, 2));
        }

        [TestMethod]
        public void MSTest_invalid_dungeon_number_of_bridges_not_level()
        {
            Predicates pred = new Predicates();

            Node nodeStart = new Node("start");
            Bridge b1 = new Bridge("1");
            Bridge b2 = new Bridge("2");
            Node nodeExit = new Node("exit");

            nodeStart.connect(b1);
            b1.connect(b2);
            b2.connect(nodeExit);

            Assert.IsFalse(pred.isValidDungeon(nodeStart, nodeExit, 3));
        }

        [TestMethod]
        public void MSTest_invalid_dungeon_cannot_reach_exit()
        {
            Predicates pred = new Predicates();

            Node nodeStart = new Node("start");
            Bridge b1 = new Bridge("1");
            Bridge b2 = new Bridge("2");
            Node nodeExit = new Node("exit");
            nodeStart.connect(b1);
            b1.connect(b2);

            Assert.IsFalse(pred.isValidDungeon(nodeStart, nodeExit, 2));
        }

        [TestMethod]
        public void MSTest_invalid_dungeon_connection_not_both_ways()
        {
            Predicates pred = new Predicates();

            Node nodeStart = new Node("start");
            Node n1 = new Node("n1");
            Bridge b1 = new Bridge("b1");
            Bridge b2 = new Bridge("b2");
            Node nodeExit = new Node("exit");

            nodeStart.neighbors.Add(n1);
            nodeStart.connect(b1);
            b1.connect(b2);
            b2.connect(nodeExit);

            Assert.IsFalse(pred.isValidDungeon(nodeStart, nodeExit, 2));
        }

        [TestMethod]
        public void MSTest_invalid_dungeon_bridge_fails_bridge_test()
        {
            Predicates pred = new Predicates();

            Node nodeStart = new Node("start");
            Bridge b1 = new Bridge("1");
            Bridge b2 = new Bridge("2");
            Bridge b3 = new Bridge("3");
            Bridge b4 = new Bridge("4");
            Node nodeExit = new Node("exit");

            nodeStart.connect(b1);
            b1.connect(b3);
            b2.connect(b3);
            b3.connect(b4);
            b4.connect(nodeExit);
            nodeStart.connect(b2);

            Assert.IsFalse(pred.isValidDungeon(nodeStart, nodeExit, 2));
        }

        [TestMethod]
        public void MSTest_invalid_dungeon_non_bridge_passes_bridge_test()
        {
            Predicates pred = new Predicates();

            Node nodeStart = new Node("start");
            Node n1 = new Node("1");
            Bridge b1 = new Bridge("1");
            Node nodeExit = new Node("exit");

            nodeStart.connect(n1);
            n1.connect(b1);
            b1.connect(nodeExit);

            Assert.IsFalse(pred.isValidDungeon(nodeStart, nodeExit, 2));
        }

        [TestMethod]
        public void MSTest_invalid_dungeon_connectivity_higher_than_3()
        {
            Predicates pred = new Predicates();

            Node nodeStart = new Node("start");
            Node nodeExit = new Node("exit");
            Node n1 = new Node();
            Node n2 = new Node();
            Node n3 = new Node();
            Node n4 = new Node();
            Node n5 = new Node();
            Node n6 = new Node();
            Bridge b1 = new Bridge("1");
            nodeStart.connect(b1);
            b1.connect(nodeExit);
            b1.connect(n1);
            b1.connect(n3);
            n1.connect(n2);
            n1.connect(n4);
            n1.connect(n5);
            n2.connect(n3);
            n2.connect(n4);
            n2.connect(n6);
            n3.connect(n5);
            n3.connect(n6);
            n4.connect(n5);
            n5.connect(n6);            

            Assert.IsFalse(pred.isValidDungeon(nodeStart, nodeExit, 1));
        }
    }
}
