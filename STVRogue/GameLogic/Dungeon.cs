using System;
using System.Collections.Generic;
using System.Linq;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Dungeon
    {
        Random random;
        public Node startNode;
        public Node exitNode;
        public uint difficultyLevel;
        /* a constant multiplier that determines the maximum number of monster-packs per node: */
        public uint M;
        public uint numberOfMonsters;
        public int totalMonsterHP = 0;
        private Predicates utils = new Predicates();

        /* To create a new dungeon with the specified difficult level and capacity multiplier */
        public Dungeon(uint level, uint nodeCapacityMultiplier, uint numberOfMonsters)
        {
            random = RandomGenerator.rnd;
            Logger.log("Creating a dungeon of difficulty level " + level + ", node capacity multiplier " + nodeCapacityMultiplier + ".");
            difficultyLevel = level;
            M = nodeCapacityMultiplier;
            this.numberOfMonsters = numberOfMonsters;

            Node start = new Node("start");
            start.zoneId = 0;
            Node exit = new Node("exit");
            exit.zoneId = (int)level + 1;
            Bridge[] bridges = new Bridge[level];
            
            for(int i = 0; i <= level; i++)
            {
                if(i < level)
                {
                    bridges[i] = new Bridge(i+1, 0, 0);
                }

                generateZone(i + 1, i == 0 ? start : bridges[i-1], i == level ? exit : bridges[i]);
            }
            startNode = start;
            exitNode = exit;
            
            TestDungeon(startNode, exitNode, difficultyLevel);
            AddItems();

        }

        public void AddItems()
        {
            List<Node> allNodes = utils.reachableNodes(startNode);
            int totalHealth = 0;
            while (totalHealth + 100 < 0.8f * totalMonsterHP - 10)
            {
                HealingPotion pot = new HealingPotion(totalHealth.ToString());
                allNodes[random.Next(0, allNodes.Count)].items.Add(pot);
                totalHealth += (int)pot.HPvalue;
            }

            int crystals = random.Next(1, (int)difficultyLevel * 2);

            for(int i = 0; i < crystals; i++)
            {
                Crystal crystal = new Crystal(i.ToString());
                allNodes[random.Next(0, allNodes.Count)].items.Add(crystal);
            }
        }

        public static void TestDungeon(Node startNode, Node exitNode, uint difficultyLevel)
        {
            Predicates utils = new Predicates();
            if (utils.isValidDungeon(startNode, exitNode, difficultyLevel))
                Logger.log("Created a valid dungeon");
            else
                throw new GameCreationException();
        }

        // Generate a zone by creating 2 paths to the next bridge and adding random connections in between those paths
        public List<Node> generateZone(int zoneLevel, Node start, Node end)
        {
            List<Node> zone = new List<Node>();
            zone.AddRange(createLinearPath(start, end, random.Next(1, 4), zoneLevel, 1));
            zone.AddRange(createLinearPath(start, end, random.Next(1, 4), zoneLevel, 2));

            int counter = 0;
            while (random.NextDouble() < 0.75 && counter < 5)
            {
                Node nodeA = zone[random.Next(zone.Count)];
                Node nodeB = zone[random.Next(zone.Count)];

                if (nodeA.id != nodeB.id)
                    if (nodeA.neighbors.Count < 4 & nodeB.neighbors.Count < 4
                        & !nodeA.neighbors.Contains(nodeB))
                    {
                        nodeA.connect(nodeB);
                        continue;
                    }

                counter++;
            }

            highConnectivitySolution(zone, zoneLevel);

            int monsterCount = (int)((2 * zoneLevel * numberOfMonsters) / ((difficultyLevel + 2) * (difficultyLevel + 1)) + 0.5f);
            if (monsterCount > zone.Count * M)
                throw new GameCreationException();
            while (monsterCount > 0)
            {
                Monster monster = new Monster(monsterCount.ToString());
                totalMonsterHP += monster.HP;
                Node temp = zone[random.Next(zone.Count)];
                int occupation = 0;
                foreach(Pack pack in temp.packs)
                    occupation += pack.members.Count;
                if (occupation < M)
                {
                    if (temp.packs.Count == 0 || random.NextDouble() < 0.25)
                    {
                        Pack pack = new Pack(temp.id + " " + monsterCount, 0);
                        pack.AddMonster(monster);
                        pack.dungeon = this;
                        pack.location = temp;
                        temp.packs.Add(pack);
                    }
                    else
                    {
                        temp.packs[random.Next(temp.packs.Count)].AddMonster(monster);
                    }
                    monsterCount--;
                }
            }

            return zone;
        }

        public static void highConnectivitySolution(List<Node> zone, int zoneLevel)
        {
            bool canConnect = false;
            Random rnd = RandomGenerator.rnd;
            while (calculateConnectivity(zone) > 3)
            {
                List<Node> newNodes = new List<Node>();
                canConnect = false;
                foreach (Node node in zone)
                    if (node.neighbors.Count < 4)
                    {
                        Node n = new Node(zoneLevel.ToString());
                        node.connect(n);
                        newNodes.Add(n);
                        canConnect = true;
                    }
                zone.AddRange(newNodes);
                if (!canConnect)
                {
                    
                    int idx = rnd.Next(0, zone.Count - 1);
                    int idx2 = rnd.Next(0, zone[idx].neighbors.Count - 1);
                    Node n2 = zone[idx].neighbors[idx2];
                    zone[idx].disconnect(n2);
                    Node n = new Node(zoneLevel.ToString());
                    n.connect(n2);                    
                    n.connect(zone[idx]);
                    zone.Add(n);
                }
            }
        }

        // Create direct path of given length between start and end bridge
        public List<Node> createLinearPath(Node start, Node end, int pathLength, int zoneId, int pathId)
        {
            List<Node> path = new List<Node>();

            for (int i = 0; i < pathLength; i++)
            {
                Node node = new Node(zoneId, pathId, i);
                path.Add(node);

                if (pathLength == 1 || i == pathLength - 1)
                {
                    if (end.id == "exit")
                        node.connect(end);
                    else
                    {
                        Bridge endBridge = (Bridge)end;
                        endBridge.connectToNodeOfSameZone(node);
                    }
                }

                if (i == 0)
                {
                    if (start.id == "start")
                        node.connect(start);
                    else
                    {
                        Bridge startBridge = (Bridge)start;
                        startBridge.connectToNodeOfNextZone(node);
                    }
                    continue;
                }

                node.connect(path[i - 1]);
            }

            return path;
        }

        public static double calculateConnectivity(List<Node> zone)
        {
            double result = 0.0;
            foreach (Node node in zone)
                result += node.neighbors.Count;

            return result / zone.Count;

        }

        /* Return a shortest path between node u and node v */
        public List<Node> shortestpath(Node u, Node v)
        {
            List<Node> result = new List<Node>();
            Queue<Node> queue = new Queue<Node>();
            Dictionary<Node, Node> passed = new Dictionary<Node, Node>();

            // BFS using a queue
            bool found = false;
            queue.Enqueue(u);
            passed.Add(u, null);
            while(queue.Count != 0 & !found)
            {
                Node currentNode = queue.Dequeue();
                foreach (Node neigbor in currentNode.neighbors)
                {
                    if(!passed.ContainsKey(neigbor))
                        passed.Add(neigbor, currentNode);
                    if(neigbor == v)
                    {
                        found = true;
                        break;
                    }
                    if (!queue.Contains(neigbor))
                        queue.Enqueue(neigbor);
                }
            }

            // Reroute back from v to u and set up path
            result.Add(v);
            Node previous = passed[v];
            result.Add(previous);

            while (previous != u)
            {
                previous = passed[previous];
                result.Add(previous);
            }

            result.Reverse();
            result.RemoveAt(0);
            return result;
        }


        /* To disconnect a bridge from the rest of the zone the bridge is in. */
        public void disconnect(Bridge b)
        {
            Logger.log("Disconnecting the bridge " + b.id + " from its zone.");
            foreach(Node node in b.fromNodes)
                b.disconnect(node);

            startNode = b;
        }

        /* To calculate the level of the given node. */
        public uint level(Node d)
        {
            if (utils.isBridge(startNode, exitNode, d))
                return utils.countNumberOfBridges(startNode, d) + 1;
            else
                return 0;
        }

        public static Command updateCommand(Game game, ConsoleKey key)
        {
            Command update = new Command();
            Node location = game.player.location;
            try
            {
                int input = 0;
                if (key == ConsoleKey.Escape)
                {
                    ReplayWriter.CloseWriter();
                    return new ExitCommand();
                }
                if (int.TryParse(key.ToString().Last().ToString(), out input))
                {
                    update = new MoveCommand(location.neighbors[input - 1]);
                }
                if (key == ConsoleKey.H)
                {
                    update = new UseHealingPotionCommand(game.player.bag.First((a) => { return a.GetType() == typeof(HealingPotion); }) as HealingPotion);
                }
                return update;
            }
            catch (Exception e)
            {
                return update;
            }
        }
    }

    public class Node
    {
        public String id;
        public int zoneId;
        public int pathId;
        public int nodeId;
        public List<Node> neighbors = new List<Node>();
        public List<Pack> packs = new List<Pack>();
        public List<Item> items = new List<Item>();

        public Node() { }
        public Node(String id) { this.id = id; }

        public Node(int zoneId, int pathId, int nodeId)
        {
            this.zoneId = zoneId; this.pathId = pathId; this.nodeId = nodeId;
            id = zoneId + "." + pathId + "." + nodeId;
        }

        /* To connect this node to another node. */
        public void connect(Node nd)
        {
            neighbors.Add(nd); nd.neighbors.Add(this);
        }

        /* To disconnect this node from the given node. */
        public void disconnect(Node nd)
        {
            neighbors.Remove(nd); nd.neighbors.Remove(this);
        }

        /* Execute a fight between the player and the packs in this node.
         * Such a fight can take multiple rounds as describe in the Project Document.
         * A fight terminates when either the node has no more monster-pack, or when
         * the player's HP is reduced to 0. 
         */
        public void combat(Player player)
        {
            while(contested(player))
            {
                int healingPots = player.bag.Count(a => a.GetType() == typeof(HealingPotion));
                int crystals = player.bag.Count(a => a.GetType() == typeof(Crystal));
                bool showItems = healingPots > 0 || crystals > 0;
                int monsters = 0;
                foreach (Pack pack in packs)
                    monsters += pack.members.Count;

                Console.Clear();
                Console.WriteLine("** Node is contested!");
                Console.WriteLine("Player HP: " + player.HP + "/" + player.HPbase);
                Console.WriteLine("Current location: " + id);
                Console.WriteLine("Zone level: " + zoneId);
                Console.WriteLine("Packs: " + packs.Count);
                Console.WriteLine();
                Console.WriteLine("-------------------------------------------");
                foreach(Pack pack in packs)
                {
                    Console.WriteLine("-- Pack '" + pack.id + "' (" + pack.members.Count + (pack.members.Count == 1 ? " monster)" : " monsters)"));
                    foreach (Monster monster in pack.members)
                        Console.WriteLine("---- Monster '" + monster.id + "' | HP: " + monster.HP);
                }
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine();
                Console.WriteLine("Bag contains:");
                Console.WriteLine(healingPots + " Healing potions");
                Console.WriteLine(crystals + " Crystals");
                Console.WriteLine();
                Console.WriteLine("Possible commands:");
                if(showItems) Console.WriteLine("i: use item");
                Console.WriteLine("f: flee");
                Console.WriteLine("a: attack");
                Console.WriteLine("esc: exit");
                ConsoleKey action = Console.ReadKey().Key;
                ReplayWriter.RecordKey(action);

                updateFightState(player, action, ConsoleKey.NoName);

                Console.Clear();
            }
        }

        public void updateFightState(Player player, ConsoleKey action1, ConsoleKey action2)
        {
            
            int healingPots = player.bag.Count(a => a.GetType() == typeof(HealingPotion));
            int crystals = player.bag.Count(a => a.GetType() == typeof(Crystal));
            bool showItems = healingPots > 0 || crystals > 0;
            switch (action1)
            {
                case ConsoleKey.I:
                    if (showItems)
                    {
                        Console.WriteLine("Bag contents:");
                        if (healingPots > 0)
                            Console.WriteLine("h: Healing Potion (" + healingPots + " left)");
                        if (crystals > 0)
                            Console.WriteLine("c: Crystal (" + crystals + " left)");
                        Console.WriteLine("b: back");
                        ConsoleKey item1;
                        if (action2 == ConsoleKey.NoName)
                            item1 = Console.ReadKey().Key;
                        else
                            item1 = action2;
                        ReplayWriter.RecordKey(item1);
                        if (item1 == ConsoleKey.H && healingPots > 0)
                        {
                            HealingPotion hPot = (HealingPotion)player.bag.First(content => content.GetType() == typeof(HealingPotion));
                            player.use(hPot);
                            Console.WriteLine("Used a healing potion. New HP: " + player.HP);
                            player.Attack(packs[0].members[0]);
                            if (contested(player))
                                monsterCombatTurn(player);
                            return;
                        }
                        else if (item1 == ConsoleKey.C && crystals > 0)
                        {
                            Crystal crystal = (Crystal)player.bag.First(content => content.GetType() == typeof(Crystal));
                            player.use(crystal);
                            Console.WriteLine("Used a crystal. You are now accelerated.");
                            player.Attack(packs[0].members[0]);
                            if (contested(player))
                                monsterCombatTurn(player);
                            return;
                        }
                        else
                            return;
                    }
                    else { return; }
                case ConsoleKey.F:
                    Console.WriteLine("Choose where to flee to:");
                    for (int i = 0; i < neighbors.Count; i++)
                        Console.WriteLine((i + 1) + ": move to " + neighbors[i].id);

                    ConsoleKey item2;
                    if (action2 == ConsoleKey.NoName)
                        item2 = Console.ReadKey().Key;
                    else
                        item2 = action2;
                    ReplayWriter.RecordKey(item2);
                    char key = item2.ToString().Last();
                    try
                    {
                        Console.Clear();
                        player.Move(neighbors[int.Parse(key.ToString()) - 1]);
                    }
                    catch { Console.Clear(); Console.WriteLine("Invalid input. Try again!"); return; }
                    break;
                case ConsoleKey.A:
                    player.Attack(packs[0].members[0]);
                    if (contested(player))
                        monsterCombatTurn(player);
                    break;
                case ConsoleKey.Escape:
                    ReplayWriter.CloseWriter();
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Unknown Command. Try again!");
                    return;
            }
        }

        public bool contested(Player player)
        {
            return packs.Count > 0 && player.location == this && player.HP > 0;
        }

        private void monsterCombatTurn(Player player)
        {
            Random rnd = RandomGenerator.rnd;
            Pack pack1 = packs[0];
            Pack pack2 = null;

            int packHealth = 0;
            foreach (Monster monster in pack1.members)
                packHealth += monster.HP;

            double fleeProbability = (1 - packHealth / pack1.startingHP) / 2;
            if (rnd.NextDouble() < fleeProbability)
            {
                if (packs.Count > 1)
                    pack2 = packs[1];

                pack1.move(neighbors.First(neighbor => neighbor.zoneId == zoneId));// Might need to try extra options when node is full
                if (contested(player))
                {
                    if (pack2 != null)
                        pack2.Attack(player);
                    else
                        pack1.Attack(player);
                }
            }
            else
            {
                pack1.Attack(player);
                pack1.previousLocation = new KeyValuePair<Node,int>(pack1.location, 2);
            }
        }
    }

    public class Bridge : Node
    {
        public List<Node> fromNodes = new List<Node>();
        public List<Node> toNodes = new List<Node>();

        public Bridge(String id) : base(id) { }
        public Bridge(int zoneId, int pathId, int nodeId)
        {
            this.zoneId = zoneId; this.pathId = pathId; this.nodeId = nodeId;
            id = (zoneId + 1) + "." + pathId + "." + nodeId;
        }

        /* Use this to connect the bridge to a node from the same zone. */
        public void connectToNodeOfSameZone(Node nd)
        {
            connect(nd);
            fromNodes.Add(nd);
        }

        /* Use this to connect the bridge to a node from the next zone. */
        public void connectToNodeOfNextZone(Node nd)
        {
            connect(nd);
            toNodes.Add(nd);
        }
    }
}
