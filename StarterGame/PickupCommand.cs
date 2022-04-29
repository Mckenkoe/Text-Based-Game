using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    class PickupCommand : Command
    {
        public PickupCommand() : base()
        {
            this.Name = "pickup";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasSecondWord())
            {
                player.Pickup(this.SecondWord);
            }
            else
            {
                player.ErrorMessage("\nPick up what?");
            }
            return false;
        }
    }
}
