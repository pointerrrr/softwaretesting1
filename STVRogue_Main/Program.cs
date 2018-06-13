using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.GameLogic;
namespace STVRogue
{
    /* A dummy top-level program to run the STVRogue game */
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(5, 4, 50);
            
            game.player.location = game.dungeon.startNode;
            while (true)
            {
                
                
                Node location = game.player.location;
                Console.WriteLine("PlayerHp: " + game.player.HP);
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
                char key = Console.ReadKey().KeyChar;
                Command update = new Command();
                try
                {
                    int input = 0;
                    if( int.TryParse(key.ToString(), out input))
                    {
                        update = new MoveCommand(location.neighbors[input - 1]);
                    }
                    if(key == 'h')
                    {
                        update = new UseHealingPotionCommand(game.player.bag.First( (a) => { return a.GetType() == typeof(HealingPotion); }) as HealingPotion);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("invalid input");
                }
                
                Console.Clear();
                game.update(update);
            }
        }
    }
}
