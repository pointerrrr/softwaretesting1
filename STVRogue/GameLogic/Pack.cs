using System;
using System.Collections.Generic;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Pack
    {
        public String id;
        public List<Monster> members = new List<Monster>();
        public int startingHP = 0;
        public Node location;
        // previous location and reason of move, reasons:
        // 0: moved
        // 1: cannot move because of capacity
        // 2: not fleeing
        // 3: tried to move out of zone
        public KeyValuePair<Node,int> previousLocation;
        public Dungeon dungeon;

        public Pack(String id, uint n)
        {
            this.id = id;
            for (int i = 0; i < n; i++)
            {
                Monster m = new Monster("" + id + "_" + i);
                members.Add(m);
                startingHP += m.HP;
            }
        }

        public void AddMonster(Monster monster)
        {
            startingHP += monster.HP;
            monster.pack = this;
            members.Add(monster);
        }

        public void Attack(Player p)
        {
            foreach (Monster m in members)
            {
                m.Attack(p);
                if (p.HP == 0) break;
            }
        }

        /* Move the pack to an adjacent node. */
        public void move(Node u)
        {
            
            if (u.zoneId != location.zoneId)
            {
                Logger.log("Pack " + id + " is trying to move out of their zone. Rejected.");
                previousLocation = new KeyValuePair<Node, int>(location,3);
                return;
            }
            if (!location.neighbors.Contains(u)) throw new ArgumentException();
            int capacity = (int) (dungeon.M * (dungeon.level(u) + 1));
            // count monsters already in the node:
            foreach (Pack Q in u.packs) {
                capacity = capacity - Q.members.Count;
            }
            // capacity now expresses how much space the node has left
            if (members.Count > capacity)
            {
                Logger.log("Pack " + id + " is trying to move to a full node " + u.id + ", but this would cause the node to exceed its capacity. Rejected.");
                previousLocation = new KeyValuePair<Node, int>(location, 1);
                return;
            }
            previousLocation = new KeyValuePair<Node, int>(location, 0);
            location = u;
            u.packs.Add(this);
            Logger.log("Pack " + id + " moves to an already full node " + u.id + ". Rejected.");

        }

        /* Move the pack one node further along a shortest path to u. */
        public void moveTowards(Node u)
        {
            if (!(location == u))
            {
                List<Node> path = dungeon.shortestpath(location, u);
                move(path[0]);
            }
        }
    }
}
