﻿using System;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Game
    {
        public Player player;
        public Dungeon dungeon;

        /* This creates a player and a random dungeon of the given difficulty level and node-capacity
         * The player is positioned at the dungeon's starting-node.
         * The constructor also randomly seeds monster-packs and items into the dungeon. The total
         * number of monsters are as specified. Monster-packs should be seeded as such that
         * the nodes' capacity are not violated. Furthermore the seeding of the monsters
         * and items should meet the balance requirements stated in the Project Document.
         */
        public Game(uint difficultyLevel, uint nodeCapcityMultiplier, uint numberOfMonsters)
        {
            Logger.log("Creating a game of difficulty level " + difficultyLevel + ", node capacity multiplier "
                       + nodeCapcityMultiplier + ", and " + numberOfMonsters + " monsters.");
            player = new Player();
            dungeon = new Dungeon(difficultyLevel, nodeCapcityMultiplier, numberOfMonsters);
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
                    return true;
                case "use-potion":
                    UseHealingPotionCommand usecommand = userCommand as UseHealingPotionCommand;
                    if (!usecommand.potion.used)
                    {
                        usecommand.potion.use(player);
                        player.bag.Remove(usecommand.potion);
                        return true;
                    }
                    return false;
                case "move-to":
                    MoveCommand movecommand = userCommand as MoveCommand;
                    return player.Move(movecommand.node);
                default:
                    return false;
            }
            
        }
    }

    public class GameCreationException : Exception
    {
        public GameCreationException() { }
        public GameCreationException(String explanation) : base(explanation) { }
    }
}
