using System.Collections;
using System.Collections.Generic;
using System;

namespace StarterGame
{
    public class Game
    {
        private Dictionary<string, Beast> _beasts;
        private Player _player;
        private Parser _parser;
        private bool _playing;

        //static int EXPLORE = 0;
        //static int FIGHT = 1;
        //int state = EXPLORE;

        static int PLAYERTURN = 0;
        static int BEASTTURN = 1;
        int state = PLAYERTURN;

        public Game()
        {
            _playing = false;
            _parser = new Parser(new CommandWords());
            _player = new Player(GameWorld.Instance().Entrance); //access instance in gameworld and passes entrance to player
            _beasts = new Dictionary<string, Beast>();

            //beasts
            Beast chicken = new Beast("chicken", 2, 5);
            _beasts.Add("chicken", chicken);
            Beast cow = new Beast("cow", 4, 15);
            _beasts.Add("cow", cow);
            Beast dog = new Beast("dog", 7, 10);
            _beasts.Add("dog", dog);

        }

       

        /**
     *  Main play routine.  Loops until end of play.
     */ 
        public void Play()
        {

            // Enter the main command loop.  Here we repeatedly read commands and
            // execute them until the game is over.

            bool finished = false;
            while (!finished)
            {

                Console.Write("\n>");
                Command command = _parser.ParseCommand(Console.ReadLine());
                if (command == null)
                {
                    Console.WriteLine("I don't understand...");
                }
                else
                {
                    finished = command.Execute(_player);
                }

                
            }
        }


        public void Start()
        {
            _playing = true;
            _player.OutputMessage(Welcome());
        }

        public void End()
        {
            _playing = false;
            _player.OutputMessage(Goodbye());
        }

        public string Welcome()
        {
            return "Welcome to the Castle!\n\nCan you make your way out?\n\nType 'help' if you need help." + _player.CurrentRoom.Description();
        }

        public string Goodbye()
        {
            return "\nThank you for playing, Goodbye. \n";
        }

    }
}
