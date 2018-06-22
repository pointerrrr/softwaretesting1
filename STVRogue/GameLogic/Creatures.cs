using System;
using System.Collections.Generic;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Creature
    {
        public String id;
        public String name;
        public int HP;
        public uint AttackRating = 1;
        public Node location;
        public Creature() { }
        virtual public void Attack(Creature foe)
        {
            foe.HP = (int)Math.Max(0, foe.HP - AttackRating);
            String killMsg = foe.HP == 0 ? ", KILLING it" : "";
            Logger.log("Creature " + id + " attacks " + foe.id + killMsg + ".");
        }
    }

    public class Monster : Creature
    {
        public Pack pack;

        /* Create a monster with a random HP */
        public Monster(String id)
        {
            this.id = id; name = "Orc";
            HP = 1 + RandomGenerator.rnd.Next(6);
        }
    }

    public class Player : Creature
    {
        public Dungeon dungeon;
        public int HPbase = 100;
        public Boolean accelerated = false;
        public uint KillPoint = 0;
        public List<Item> bag = new List<Item>();
        public List<Item> usedItems = new List<Item>();
        public Node previousLocation;
        public List<Node> previousLocations = new List<Node>();
        public Game game;
        

        public Player()
        {
            //this.dungeon = dungeon;
            id = "player";
            AttackRating = 5;
            HP = HPbase;
        }

        public void use(Item item)
        {
            if (!bag.Contains(item) || item.used) throw new ArgumentException();
            item.use(this);
            bag.Remove(item);
            usedItems.Add(item);
        }

        override public void Attack(Creature foe)
        {
            if (!(foe is Monster)) throw new ArgumentException();
            Monster foe_ = foe as Monster;
            if (!accelerated)
            {
                base.Attack(foe);
                if (foe_.HP == 0)
                {
                    foe_.pack.members.Remove(foe_);
                    KillPoint++;
                }
            }
            else
            {
                int packCount = foe_.pack.members.Count;
                foe_.pack.members.RemoveAll(target => target.HP <= 0);
                KillPoint += (uint) (packCount - foe_.pack.members.Count) ;
                List<Monster> alive = new List<Monster>();
                foreach (Monster target in foe_.pack.members)
                {
                    base.Attack(target);
                    if (target.HP != 0)
                        alive.Add(target);
                    else
                        KillPoint++;
                }
                foe_.pack.members = alive;
                accelerated = false;
            }
            if (foe_.pack.members.Count == 0)
                foe_.pack.location.packs.Remove(foe_.pack);
        }

        public bool Move(Node node)
        {
            if (location.neighbors.Contains(node))
            {
                if(!previousLocations.Contains(location))
                    previousLocations.Add(location);
                location = node;
                if (location.items.Count > 0)
                    foreach (Item item in location.items)
                        bag.Add(item);
                location.items.Clear();                
                return true;
            }
            return false;
        }
    }
}
