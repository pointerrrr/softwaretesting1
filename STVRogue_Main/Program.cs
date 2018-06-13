using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.GameLogic;
using STVRogue.Utils;
namespace STVRogue
{
    /* A dummy top-level program to run the STVRogue game */
    class Program
    {
        static void Main(string[] args)
        {
            RandomGenerator.initializeWithSeed(1337);
            Game game = new Game(5, 4, 50);
            
            game.player.location = game.dungeon.startNode;

            Console.WriteLine("Press a button to start");
            Console.ReadKey();
            Console.Clear();

            while (true)
            {
                if (game.player.location == game.dungeon.exitNode)
                {
                    Console.WriteLine("You WON!");
                    Console.ReadKey();
                    break;
                }

                if(game.player.location.contested(game.player))
                {
                    game.player.location.combat(game.player);
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

                

                if(game.player.bag.Exists(a => a.GetType() == typeof(HealingPotion)) )
                {
                    Console.WriteLine("h: use healing potion");
                }

                Console.WriteLine("d: do nothing");
                Console.WriteLine("esc: exit");
                ConsoleKey key = Console.ReadKey().Key;
                Command update = new Command();
                try
                {
                    if (key == ConsoleKey.Escape)
                        return;
                    int input = 0;
                    string b = key.ToString();
                    if( int.TryParse(key.ToString().Last().ToString(), out input))
                    {
                        update = new MoveCommand(location.neighbors[input - 1]);
                    }
                    if(key == ConsoleKey.H)
                    {
                        update = new UseHealingPotionCommand(game.player.bag.First( (a) => { return a.GetType() == typeof(HealingPotion); }) as HealingPotion);
                    }                    
                    game.update(update);
                }
                catch (Exception) { }
                Console.Clear();
            }
        }
    }
}
