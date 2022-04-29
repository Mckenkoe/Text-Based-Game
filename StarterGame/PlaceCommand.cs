using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    class PlaceCommand : Command
    {
        public PlaceCommand() : base()
        {
            this.Name = "place";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasSecondWord())
            {
                player.Place(this.SecondWord);
            }
            else
            {
                player.ErrorMessage("\nPlace what?");
            }
            return false;
        }
    }
}
