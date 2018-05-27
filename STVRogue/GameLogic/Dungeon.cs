using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private Predicates utils = new Predicates();

        /* To create a new dungeon with the specified difficult level and capacity multiplier */
        public Dungeon(uint level, uint nodeCapacityMultiplier, uint numberOfMonsters)
        {
            RandomGenerator.initializeWithSeed(1337);
            random = RandomGenerator.rnd;
            Logger.log("Creating a dungeon of difficulty level " + level + ", node capacity multiplier " + nodeCapacityMultiplier + ".");
            difficultyLevel = level;
            M = nodeCapacityMultiplier;
            this.numberOfMonsters = numberOfMonsters;

            Node start = new Node("start");            
            Node exit = new Node("exit");
            Bridge[] bridges = new Bridge[level];
            
            for(int i = 0; i <= level; i++)
            {
                if(i < level)
                {
                    bridges[i] = new Bridge((i+1).ToString());
                }

                generateZone(i + 1, i == 0 ? start : bridges[i-1], i == level ? exit : bridges[i]);
            }
            startNode = start;
            exitNode = exit;
            
            TestDungeon(startNode, exitNode, difficultyLevel);
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
                Node temp = zone[random.Next(zone.Count)];
                int occupation = 0;
                foreach(Pack pack in temp.packs)
                    occupation += pack.members.Count;
                if (occupation < M)
                {
                    if (temp.packs.Count == 0 || random.NextDouble() < 0.25)
                    {
                        Pack pack = new Pack(temp.id + " " + monsterCount, 0);
                        pack.members.Add(monster);
                        pack.location = temp;
                        temp.packs.Add(pack);
                    }
                    else
                    {
                        temp.packs[random.Next(temp.packs.Count)].members.Add(monster);
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
            /*int counter = 0;
            int nodeId = 0;
            // Disconnect or add new nodes untill connectivity falls below 3.0
            while (calculateConnectivity(zone) > 3.0)
            {
                zone.Sort((nodeA, nodeB) => nodeA.neighbors.Count.CompareTo(nodeB.neighbors.Count));

                // If all nodes are full, disconnect with a node from the other path
                if (zone[0].neighbors.Count == 4)
                {
                    Node fullNode = zone[counter];
                    Node toDisconnect = fullNode.neighbors.Find(node => node.pathId != fullNode.pathId && node.pathId != 0);
                    if (toDisconnect == null)
                    {
                        counter++;
                        continue;
                    }
                    fullNode.disconnect(toDisconnect);
                    counter = 0;
                    continue;
                }

                Node newNode = new Node(zoneLevel, 3, nodeId);
                newNode.connect(zone[0]);
                zone.Add(newNode);
                nodeId++;
            }*/
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
        public void fight(Player player)
        {
            throw new NotImplementedException();
        }
    }

    public class Bridge : Node
    {
        public List<Node> fromNodes = new List<Node>();
        public List<Node> toNodes = new List<Node>();
        public Bridge(String id) : base(id) { }

        /* Use this to connect the bridge to a node from the same zone. */
        public void connectToNodeOfSameZone(Node nd)
        {
            base.connect(nd);
            fromNodes.Add(nd);
        }

        /* Use this to connect the bridge to a node from the next zone. */
        public void connectToNodeOfNextZone(Node nd)
        {
            base.connect(nd);
            toNodes.Add(nd);
        }
    }
}
