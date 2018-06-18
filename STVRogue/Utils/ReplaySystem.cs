using System;
using STVRogue.GameLogic;
using System.IO;
using System.Collections.Generic;
using static System.Environment;

namespace STVRogue.Utils
{
    public static class ReplayWriter
    {
        static StreamWriter writer;

        private static Game currenState;

        public static void InitializeReplaySystem(string name, int seed, uint difficultyLevel, uint nodeCapacityMultiplier, uint numberOfMonsters)
        {
            try
            {

                writer = new StreamWriter(GetFolderPath(SpecialFolder.MyDocuments) + "/" + name + ".txt");
                writer.WriteLine(seed);
                writer.WriteLine(difficultyLevel + "," + nodeCapacityMultiplier + "," + numberOfMonsters);
            }
            catch {
                writer = new StreamWriter(GetFolderPath(SpecialFolder.MyDocuments) + "/" + "backup" + ".txt");
                writer.WriteLine(seed);
                writer.WriteLine(difficultyLevel + "," + nodeCapacityMultiplier + "," + numberOfMonsters);
            }
        }

        public static void CloseWriter()
        {
            writer.Close();
        }

        public static void RecordKey(ConsoleKey key)
        {
            writer.Write(key.ToString() + ",");
        }
    }

    public class ReplayReader
    {
        StreamReader reader;
        Game gameState;
        string path;
        int currentTurn;
        int seed;
        uint difficultyLevel, nodeCapacityMultiplier, numberOfMonsters;
        List<ConsoleKey> commands;

        public ReplayReader(string path)
        {
            try
            {
                
                this.path = path;
                reader = new StreamReader(path);
                seed = int.Parse(reader.ReadLine());
                string line = reader.ReadLine();
                string[] values = line.Split(',');
                difficultyLevel = uint.Parse(values[0]);
                nodeCapacityMultiplier = uint.Parse(values[1]);
                numberOfMonsters = uint.Parse(values[2]);
                commands = new List<ConsoleKey>();
                ReplayWriter.InitializeReplaySystem(Path.GetFileNameWithoutExtension(path), seed, difficultyLevel, nodeCapacityMultiplier, numberOfMonsters);
                string[] res = reader.ReadToEnd().Split(',');
                foreach (string key in res)
                {
                    ConsoleKey result;
                    if(Enum.TryParse(key, out result))
                        commands.Add(result);
                }
            }
            catch { throw new InvalidDataException(); }
        }

        public bool replay(Specification spec)
        {
            reset();
            while (true)
            {
                // test if the specification holds on the current state:
                bool ok = spec.Test(getState());
                if (ok)
                {
                    // if the specification holds, move to the next turn (if there is a next turn):
                    if (hasNextTurn())
                        replayTurn();
                    else
                        break;
                }
                else // oterwise the test fails:
                {
                    return false;
                }
            }
            return true;
        }

        private void replayTurn()
        {
            if (gameState.player.location.contested(gameState.player))
            {
                
                if(commands[currentTurn] == ConsoleKey.I || commands[currentTurn] == ConsoleKey.F)
                    gameState.player.location.updateFightWithItem(gameState.player, commands[currentTurn++], commands[currentTurn++]);
                else
                    gameState.player.location.updateFightState(gameState.player, commands[currentTurn++]);
            }
            else
            {
                Command updateCommand = Dungeon.updateCommand(gameState, commands[currentTurn++]);
                gameState.update(updateCommand);
            }
        }

        private void reset()
        {
            currentTurn = 0;
            RandomGenerator.initializeWithSeed(seed);
            gameState = new Game(difficultyLevel, nodeCapacityMultiplier, numberOfMonsters);
        }

        private bool hasNextTurn()
        {
            return currentTurn < commands.Count;
        }

        private Game getState()
        {
            return gameState;
        }
    }

    public abstract class Specification
    {
        public abstract bool Test(Game gameState);
    }

    public class Always : Specification
    {
        private Predicate<Game> predicate;
        public Always (Predicate<Game> p)
        {
            predicate = p;
        }

        public override bool Test(Game gameState)
        {
            return predicate(gameState);
        }
    }

    public class Unless : Specification
    {
        public bool history = true;

        public Predicate<Game> always;
        public Predicate<Game> unless;

        public Unless(Predicate<Game> p, Predicate<Game> q)
        {
            always = p;
            unless = q;
        }

        public override bool Test(Game gameState)
        {
            return history = !history || (history && (always(gameState) || unless(gameState)));
        }            
    }
}
