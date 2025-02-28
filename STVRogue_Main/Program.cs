﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.GameLogic;
using STVRogue.Utils;

namespace STVRogue
{
    /* A dummy top-level program to run the STVRogue game */
    public class Program
    {
        static Game game;
        static void Main(string[] args)
        {
            int seed = 18927;
            RandomGenerator.initializeWithSeed(seed);
            uint difficultyLevel = 3;
            uint nodeCapacityMultiplier = 3;
            uint numberOfMonsters = 20;
            game = new Game(difficultyLevel, nodeCapacityMultiplier, numberOfMonsters);

            ReplayWriter.InitializeReplaySystem("currentReplay", seed, difficultyLevel, nodeCapacityMultiplier, numberOfMonsters);

            Console.WriteLine("Press a button to start");
            Console.ReadKey();
            Console.Clear();

            while (true)
            {
                if (game.player.location.contested(game.player))
                {
                    game.player.location.combat(game.player);
                }
                else
                {

                    if (game.player.location == game.dungeon.exitNode)
                    {
                        Console.WriteLine("You WON!");
                        ReplayWriter.CloseWriter();
                        Console.ReadKey();
                        break;
                    }

                    if (game.player.HP <= 0)
                    {
                        Console.Clear();
                        Console.WriteLine("You LOST!");
                        ReplayWriter.CloseWriter();
                        Console.ReadKey();
                        break;
                    }


                    Node location = game.player.location;
                    Console.WriteLine("PlayerHp: " + game.player.HP + "/" + game.player.HPbase);
                    Console.WriteLine("Current location: " + location.id);
                    Console.WriteLine("Bag contains:");
                    Console.WriteLine(game.player.bag.Count(a => a.GetType() == typeof(HealingPotion)) + " healing potions");
                    Console.WriteLine(game.player.bag.Count(a => a.GetType() == typeof(Crystal)) + " crystals");
                    Console.WriteLine("Available commands:");
                    for (int i = 0; i < location.neighbors.Count; i++)
                        Console.WriteLine((i + 1) + ": move to " + location.neighbors[i].id);



                    if (game.player.bag.Exists(a => a.GetType() == typeof(HealingPotion)))
                    {
                        Console.WriteLine("h: use healing potion");
                    }

                    Console.WriteLine("d: do nothing");
                    Console.WriteLine("esc: exit");
                    ConsoleKey key = Console.ReadKey().Key;
                    ReplayWriter.RecordKey(key);
                    if (key == ConsoleKey.Escape)
                    {
                        Console.Clear();
                        ReplayWriter.CloseWriter();
                        break;
                    }
                    Command update = null;
                    try
                    {
                        update = Dungeon.updateCommand(game, key);
                        if (update != null)
                            game.update(update);
                    }
                    catch (Exception) { }
                    Console.Clear();
                }
            }            
        }
    }
}
