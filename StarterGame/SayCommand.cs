﻿using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    class SayCommand : Command
    {
        public SayCommand() : base()
        {
            this.Name = "say";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasSecondWord())
            {
                player.Say(this.SecondWord);
            }
            else
            {
                player.ErrorMessage("\nSay What?");
            }
            return false;
        }
    }
}
