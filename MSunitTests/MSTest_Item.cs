using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STVRogue.GameLogic
{
    [TestClass]
    public class MSTest_Item
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
        public void MSTest_healing_potion_unused()
        {
            Player player = new Player();
            player.HP = 80;

            HealingPotion pot = new HealingPotion("testPot");
            pot.HPvalue = 8;

            pot.use(player);

            Assert.IsTrue(player.HP == 88);
            Assert.IsTrue(pot.used);
        }

        [TestMethod]
        public void MSTest_healing_potion_unused_above_max()
        {
            Player player = new Player();
            player.HP = 95;

            HealingPotion pot = new HealingPotion("testPot");
            pot.HPvalue = 8;

            pot.use(player);

            Assert.IsTrue(player.HP == 100);
            Assert.IsTrue(pot.used);
        }

        [TestMethod]
        public void MSTest_healing_potion_used()
        {
            Player player = new Player();
            player.HP = 95;

            HealingPotion pot = new HealingPotion("testPot");
            pot.HPvalue = 8;
            pot.used = true;

            pot.use(player);

            Assert.IsTrue(player.HP == 95);
        }

        [TestMethod]
        public void MSTest_crystal_unused_node()
        {
            initDungeon();
            Player player = new Player();
            Crystal crystal = new Crystal("testCrystal");

            player.location = n0;

            crystal.use(player);

            Assert.IsTrue(player.accelerated);
            Assert.IsTrue(testDungeon.startNode == start);
        }

        [TestMethod]
        public void MSTest_crystal_unused_bridge()
        {
            initDungeon();
            Player player = new Player();
            Crystal crystal = new Crystal("testCrystal");

            player.dungeon = testDungeon;
            player.location = b1;
            crystal.use(player);

            Assert.IsTrue(player.accelerated);
            Assert.IsTrue(testDungeon.startNode == b1);
        }

        [TestMethod]
        public void MSTest_crystal_used()
        {
            initDungeon();
            Player player = new Player();
            Crystal crystal = new Crystal("testCrystal");

            player.dungeon = testDungeon;
            player.location = b1;

            crystal.used = true;
            crystal.use(player);

            Assert.IsFalse(player.accelerated);
            Assert.IsTrue(testDungeon.startNode == start);
        }
    }
}
