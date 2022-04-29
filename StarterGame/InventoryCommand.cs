using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    class InventoryCommand : Command
    {
        public InventoryCommand() : base()
        {
            this.Name = "inventory";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasSecondWord())
            {
                player.OutputMessage("\nI cannot do that");
            }
            else
            {
                player.Inventory();
            }
            return false;
        }
    }
}
