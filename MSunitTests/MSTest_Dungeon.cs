using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STVRogue.GameLogic
{
    [TestClass]
    public class MSTest_Dungeon
    {
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

            Dungeon dungeon = new Dungeon(1, 1);

            List<Node> shortest_path = new List<Node>();
            shortest_path.Add(n0);
            shortest_path.Add(n2);
            shortest_path.Add(n6);

            List<Node> result_path = dungeon.shortestpath(n0, n6);

            CollectionAssert.AreEqual(shortest_path, result_path);
        }
    }
}
