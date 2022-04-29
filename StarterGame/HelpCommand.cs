using System.Collections;
using System.Collections.Generic;

namespace StarterGame
{
    public class HelpCommand : Command
    {
        private CommandWords _words;

        public HelpCommand() : this(new CommandWords()){}

        // Designated Constructor
        public HelpCommand(CommandWords commands) : base()
        {
            _words = commands;
            this.Name = "help";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasSecondWord())
            {
                player.OutputMessage("\nI cannot help you with " + this.SecondWord);
            }
            else
            {
                player.OutputMessage("\nYou are in a large castle. What will you do? \n\nYour available commands are " + _words.Description());
            }
            return false;
        }
    }
}
