using STVRogue.GameLogic;
namespace STVRogue
{
    public class Command
    {
        public override string ToString()
        {
            return "no-action";
        }
    }

    public class UseHealingPotionCommand : Command
    {
        public HealingPotion potion;

        public UseHealingPotionCommand(HealingPotion potion)
        {
            this.potion = potion;
        }

        public override string ToString()
        {
            return "use-potion";
        }
    }

    public class MoveCommand : Command
    {
        public Node node;

        public MoveCommand(Node node)
        {
            this.node = node;
        }

        public override string ToString()
        {
            return "move-to";
        }
    }

    public class ExitCommand : Command
    {
        public override string ToString()
        {
            return "exit";
        }
    }
}
