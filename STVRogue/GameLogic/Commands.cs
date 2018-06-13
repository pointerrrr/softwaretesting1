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

    public class UseCrystalCommand : Command
    {
        public Crystal crystal;

        public UseCrystalCommand(Crystal crystal)
        {
            this.crystal = crystal;
        }

        public override string ToString()
        {
            return "use-crystal";
        }
    }

    public class AttackCommand : Command
    {
        public Pack pack;

        public AttackCommand(Pack pack)
        {
            this.pack = pack;
        }

        public override string ToString()
        {
            return "attack";
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
}
