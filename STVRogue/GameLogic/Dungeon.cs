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

        /* To create a new dungeon with the specified difficult level and capacity multiplier */
        public Dungeon(uint level, uint nodeCapacityMultiplier)
        {
            RandomGenerator.initializeWithSeed(1337);
            random = RandomGenerator.rnd;
            Logger.log("Creating a dungeon of difficulty level " + level + ", node capacity multiplier " + nodeCapacityMultiplier + ".");
            difficultyLevel = level;
            M = nodeCapacityMultiplier;

            Node start = new Node("start");            
            Node exit = new Node("exit");
            List<List<Node>> zones = new List<List<Node>>();
            Bridge[] bridges = new Bridge[level];
            
            for(int i = 0; i <= level; i++)
            {
                if(i < level)
                    bridges[i] = new Bridge(i.ToString());
                zones.Add( generateZone(i, i == 0 ? start : bridges[i-1], i == level ? exit : bridges[i]));
            }
            startNode = start;
            exitNode = exit;
        }

        // based on https://stackoverflow.com/a/39112650
        public List<Node> generateZone(int zoneId, Node start, Node end)
        {
            List<Node> result = new List<Node>();
            int size = random.Next(1, 8);
            for (int i = 0; i < size; i++)
            {
                result.Add( new Node(zoneId + "." + i));
            }

            int startConnections = Math.Min(random.Next(0, 2), result.Count-1);
            for (int i = 0; i <= startConnections; i++)
            {
                result[i].connectedWithStart = true;
                result[i].connect(start);
            }

            int endConnections = Math.Min(random.Next(0, 2), result.Count-1);
            for (int i = 0; i <= startConnections; i++)
            {
                result[result.Count- 1 - i].connect(end);
            }

            foreach (Node node in result)
            {
                int numConnections = random.Next(0,3);
                for (int i = 0; i < numConnections; i++)
                {
                    //Find a random node to connect to
                    int idx = (int)(random.NextDouble() * result.Count);
                    if (result[idx].neighbors.Count < 4)
                        node.connect(result[idx]);
                }
            }

            foreach(Node node in result)
            {
                List<Node> connected = new List<Node>();
                foreach (Node temp in result)
                    if (temp.connectedWithStart)
                        connected.Add(temp);
                while (!node.connectedWithStart)
                {
                    int idx = random.Next(0, connected.Count);
                    //if (connected[idx].neighbors.Count < 4)
                        node.connect(connected[idx]);
                }
            }

            return result;
        }

        /*
        public List<Node> generateZonae(int zoneId)
        {
            List<Node> result = new List<Node>();
            int size = random.Next(1,8);
            for(int i = 0; i < size; i++)
            {
                result[i] = new Node(zoneId + "." + i);
            }
            
            List<Node[]> nodeList = new List<Node[]>();
            int nodeTotal = 0;
            int columnCount = 0;
            while(nodeTotal < size)
            {
                nodeList[columnCount] = new Node[3];

                int rando = random.Next(0, 7);

                for(int i = 0; i < 3; i++)
                    nodeList[columnCount][i] = permutations[rando, i] ? new Node() : null;
                nodeTotal += rando / 3 + 1;
                columnCount++;
            }
            for (int i = 0; i < 3; i++)
                if(nodeList[0][i] != null)
                    nodeList[0][i].connectedWithStart = true;


            for(int i = 0; i < columnCount - 1; i++)
                for (int k = 0; k < 3; k++)
                    for (int j = 0; j < 2; j++)
                    {
                        if (k > 0)
                            if (random.Next(0, 3) == 2 && nodeList[i + j][k - 1] != null)
                                nodeList[i + j][k - 1].connect(nodeList[i][k]);
                        if (j != 0)
                            if(random.Next(0,3) == 2 && nodeList[i + j][k] != null)
                                nodeList[i + j][k].connect(nodeList[i][k]);
                        if (k < 2)
                            if(random.Next(0,3) == 2 && nodeList[i + j][k + 1] != null)
                                nodeList[i + j][k + 1].connect(nodeList[i][k]);
                    }

            for (int i = 1; i < columnCount; i++)
                for (int j = 0; j < 3; j++)
                {
                    Node node = nodeList[i][j];
                    if(node != null)
                    {
                        if (!node.connectedWithStart)
                        {
                            List<Node> list = new List<Node>();
                            for (int k = -1; k < 2; k++)
                            {
                                if (j == 0)
                                {

                                }
                                if (j == 1)
                                {

                                }
                                if (j == 2)
                                {

                                }
                            }
                        }
                    }
                }

            return null;
        }*/

        //Credits to https://stackoverflow.com/questions/273313/randomize-a-listt
        public void Shuffle(List<Node> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Node value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        bool[,] permutations = new bool[7, 3] { { true, false, false }, { false, true, false }, { false, false, true }, { true, true, false }, { false, true, true }, { true, false, true }, { true, true, true } };

        /* Return a shortest path between node u and node v */
        public List<Node> shortestpath(Node u, Node v) { throw new NotImplementedException(); }


        /* To disconnect a bridge from the rest of the zone the bridge is in. */
        public void disconnect(Bridge b)
        {
            Logger.log("Disconnecting the bridge " + b.id + " from its zone.");
            throw new NotImplementedException();
        }

        /* To calculate the level of the given node. */
        public uint level(Node d) { throw new NotImplementedException(); }
    }

    public class Node
    {
        public String id;
        public List<Node> neighbors = new List<Node>();
        public List<Pack> packs = new List<Pack>();
        public List<Item> items = new List<Item>();
        public bool connectedWithStart = false;

        public Node() { }
        public Node(String id) { this.id = id; }

        /* To connect this node to another node. */
        public void connect(Node nd)
        {
            if (connectedWithStart)
                nd.connectedWithStart = true;
            if (nd.connectedWithStart)
                connectedWithStart = true;
            neighbors.Add(nd); nd.neighbors.Add(this);
            checkStart();
            nd.checkStart();
        }

        public void checkStart()
        {
            if (connectedWithStart)
                foreach (Node node in neighbors)
                {
                    if (!node.connectedWithStart)
                    {
                        node.connectedWithStart = true;
                        node.checkStart();
                    }
                }
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
        List<Node> fromNodes = new List<Node>();
        List<Node> toNodes = new List<Node>();
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
