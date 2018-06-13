using System;
using STVRogue.Utils;
using System.Collections.Generic;

namespace STVRogue.GameLogic
{
    public class Game
    {
        public Player player;
        public Dungeon dungeon;
        Random rnd;
        Predicates pred = new Predicates();

        /* This creates a player and a random dungeon of the given difficulty level and node-capacity
         * The player is positioned at the dungeon's starting-node.
         * The constructor also randomly seeds monster-packs and items into the dungeon. The total
         * number of monsters are as specified. Monster-packs should be seeded as such that
         * the nodes' capacity are not violated. Furthermore the seeding of the monsters
         * and items should meet the balance requirements stated in the Project Document.
         */
        public Game(uint difficultyLevel, uint nodeCapcityMultiplier, uint numberOfMonsters)
        {
            rnd = RandomGenerator.rnd;
            Logger.log("Creating a game of difficulty level " + difficultyLevel + ", node capacity multiplier "
                       + nodeCapcityMultiplier + ", and " + numberOfMonsters + " monsters.");
            player = new Player();
            dungeon = new Dungeon(difficultyLevel, nodeCapcityMultiplier, numberOfMonsters);
            player.location = dungeon.startNode;
            player.HP =  (int) Math.Max(1, Math.Min(100, dungeon.totalMonsterHP * 0.79));
        }

        /*
         * A single update turn to the game. 
         */
        public Boolean update(Command userCommand)
        {
            Logger.log("Player does " + userCommand);
            
            switch(userCommand.ToString())
            {
                case "no-action":
                    break;
                case "use-potion":
                    UseHealingPotionCommand usecommand = userCommand as UseHealingPotionCommand;
                    usecommand.potion.use(player);
                    player.bag.Remove(usecommand.potion);
                    break;
                case "move-to":
                    MoveCommand movecommand = userCommand as MoveCommand;
                    if (!player.Move(movecommand.node))
                        return false;
                    break;
                default:
                    return false;
            }
            updateMonsters();
            return true;
        }

        private void updateMonsters()
        {
            List<Node> allNodes = new List<Node>();
            allNodes = pred.reachableNodes(dungeon.startNode);
            List<Pack> allPacks = new List<Pack>();
            foreach(Node node in allNodes)
            {
                foreach (Pack pack in node.packs)
                    allPacks.Add(pack);
            }
            foreach(Pack pack in allPacks)
            {
                if (!pack.location.contested(player))
                    continue;
                if((pack.location.zoneId == player.location.zoneId && player.location.contested(player)) ||
                    player.location.zoneId == dungeon.difficultyLevel + 1 && pack.location.zoneId == dungeon.difficultyLevel + 1)
                {
                    pack.moveTowards(player.location);
                }
                else
                {
                    bool move = rnd.Next(0, 2) == 0;
                    if (move)
                    {
                        int target = rnd.Next(0, pack.location.neighbors.Count);
                        pack.move(pack.location.neighbors[target]);
                    }
                }
            }
        }        
    }


    

    public class GameCreationException : Exception
    {
        public GameCreationException() { }
        public GameCreationException(String explanation) : base(explanation) { }
    }
}
