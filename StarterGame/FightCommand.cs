using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    class FightCommand : Command
    {
        public FightCommand() : base()
        {
            this.Name = "fight";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasSecondWord())
            {
                player.Fight(this.SecondWord);
            }
            else
            {
                player.ErrorMessage("\nFight what?");
            }
            return false;
        }
    }
}
