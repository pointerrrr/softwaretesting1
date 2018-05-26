using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STVRogue.GameLogic
{
    /* An example of a test class written using VisualStudio's own testing
     * framework. 
     * This one is to unit-test the class Player. The test is incomplete though, 
     * as it only contains two test cases. 
     */
    [TestClass]
    public class MSTest_Player
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MSTest_use_onEmptyBag()
        {
            Player P = new Player();
            P.use(new Item());
        }

        [TestMethod]
        public void MSTest_use_item_in_bag()
        {
            Player P = new Player();
            Item x = new HealingPotion("pot1");
            P.bag.Add(x);
            P.use(x);
            Assert.IsFalse(P.bag.Contains(x));
        }

        [TestMethod]
        public void MSTest_kill_single_monster()
        {
            Player p = new Player();
            Node node = new Node();
            Pack pack = new Pack("1", 1);
            Monster monster = new Monster("peter");

            pack.members.Clear();
            pack.location = node;

            node.packs.Add(pack);

            monster.HP = 3;
            monster.pack = pack;

            pack.members.Add(monster);

            p.Attack(monster);
            Assert.IsTrue(monster.HP == 0);
            Assert.IsTrue(pack.members.Count == 0);
            Assert.IsTrue(node.packs.Count == 0);
        }

        [TestMethod]
        public void MSTest_not_kill_single_monster()
        {
            Player p = new Player();
            Pack pack = new Pack("1", 1);
            Monster monster = new Monster("peter");

            pack.members.Clear();

            monster.HP = 6;
            monster.pack = pack;

            pack.members.Add(monster);

            p.Attack(monster);
            Assert.IsTrue(monster.HP == 1);
            Assert.IsTrue(pack.members.Count == 1);
        }

        [TestMethod]
        public void MSTest_kill_full_pack()
        {
            Player p = new Player();
            Node node = new Node();
            Pack pack = new Pack("1", 1);
            Monster monster1 = new Monster("peter");
            Monster monster2 = new Monster("jan");
            Monster monster3 = new Monster("hendrik");

            pack.members.Clear();
            pack.location = node;

            node.packs.Add(pack);

            monster1.HP = 3;
            monster1.pack = pack;

            monster2.HP = 3;
            monster2.pack = pack;

            monster3.HP = 3;
            monster3.pack = pack;

            pack.members.Add(monster1);
            pack.members.Add(monster2);
            pack.members.Add(monster3);

            p.accelerated = true;

            p.Attack(monster1);

            Assert.IsTrue(pack.members.Count == 0);
            Assert.IsTrue(node.packs.Count == 0);
        }

        [TestMethod]
        public void MSTest_kill_partial_pack()
        {
            Player p = new Player();
            Node node = new Node();
            Pack pack = new Pack("1", 1);
            Monster monster1 = new Monster("peter");
            Monster monster2 = new Monster("jan");
            Monster monster3 = new Monster("hendrik");

            pack.members.Clear();
            pack.location = node;

            node.packs.Add(pack);

            monster1.HP = 3;
            monster1.pack = pack;

            monster2.HP = 6;
            monster2.pack = pack;

            monster3.HP = 6;
            monster3.pack = pack;

            pack.members.Add(monster1);
            pack.members.Add(monster2);
            pack.members.Add(monster3);

            p.accelerated = true;

            p.Attack(monster1);

            Assert.IsTrue(pack.members.Count == 2);
            Assert.IsFalse(pack.members.Contains(monster1));
            Assert.IsTrue(node.packs.Count == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MSTest_attack_non_monster()
        {
            Player p = new Player();
            p.Attack(p);
        }
    }
}
