using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.GameLogic;

namespace STVRogue_Main
{
    class ReplaySystem
    {
        bool replay(Specification S)
        {
            reset();
            while (true)
            {
                // test if the specification holds on the current state:
                bool ok = S.test(getState());
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
                return true;
            }
            return false;
        }

        void replayTurn()
        {

        }

        void reset()
        {

        }

        bool hasNextTurn()
        {
            return false;
        }

        Game getState()
        {
            return null;
        }
    }

    public class Specification
    {
        public bool test(Game gameState)
        {
            return false;
        }
    }
}
