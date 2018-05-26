using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    [TestClass]
    public class MSTest_Pack
    {
        public Dungeon testDungeon = new Dungeon(1, 20, 0);

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
        public void initDungeon()
        {
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
        }

        [TestMethod]
        public void MSTest_pack_constructor()
        {
            Pack pack = new Pack("Test", 50);
            Assert.IsTrue(pack.members.Count == 50);
        }

        [TestMethod]
        public void MSTest_pack_attack_nlethal()
        {
            Pack pack = new Pack("Test", 20);
            Player player = new Player();

            player.HP = 87;

            pack.Attack(player);

            Assert.IsTrue(player.HP == 67);
        }

        [TestMethod]
        public void MSTest_pack_attack_lethal()
        {
            Pack pack = new Pack("Test", 20);
            Player player = new Player();

            player.HP = 15;

            pack.Attack(player);

            Assert.IsTrue(player.HP == 0);
        }

        [TestMethod]
        public void MSTest_pack_move_neigbor_allowed()
        {
            initDungeon();

            Pack pack = new Pack("test", 15);
            pack.location = n0;
            pack.dungeon = testDungeon;
            n0.packs.Add(pack);

            Pack pack2 = new Pack("test2", 3);
            pack2.location = n1;
            pack2.dungeon = testDungeon;
            n1.packs.Add(pack2);

            pack.move(n1);

            Assert.IsTrue(pack.location == n1);
        }

        [TestMethod]
        public void MSTest_pack_move_neigbor_capacity_full()
        {
            initDungeon();

            Pack pack = new Pack("test", 15);
            pack.location = n0;
            pack.dungeon = testDungeon;
            n0.packs.Add(pack);

            Pack pack2 = new Pack("test2", 7);
            pack2.location = n1;
            pack2.dungeon = testDungeon;
            n1.packs.Add(pack2);

            pack.move(n1);

            Assert.IsTrue(pack.location == n0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MSTest_pack_move_non_neigbor()
        {
            initDungeon();

            Pack pack = new Pack("test", 15);
            pack.location = n0;
            pack.dungeon = testDungeon;
            n0.packs.Add(pack);

            pack.move(n4);
        }

        [TestMethod]
        public void MSTest_pack_moveTowards_allowed()
        {
            initDungeon();

            Pack pack = new Pack("test", 7);
            pack.location = b1;
            pack.dungeon = testDungeon;
            b1.packs.Add(pack);

            Pack pack2 = new Pack("test2", 5);
            pack2.location = n5;
            pack2.dungeon = testDungeon;

            n5.packs.Add(pack2);

            pack.moveTowards(n6);

            Assert.IsTrue(pack.location == n5);
        }

        [TestMethod]
        public void MSTest_pack_moveTowards_capacity_full()
        {
            initDungeon();

            Pack pack = new Pack("test", 7);
            pack.location = b1;
            pack.dungeon = testDungeon;
            b1.packs.Add(pack);

            Pack pack2 = new Pack("test2", 15);
            pack2.location = n5;
            pack2.dungeon = testDungeon;

            n5.packs.Add(pack2);

            pack.moveTowards(n6);

            Assert.IsTrue(pack.location == b1);
        }
    }
}
