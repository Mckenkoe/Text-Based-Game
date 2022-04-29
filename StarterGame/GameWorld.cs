using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    public class GameWorld
    {
        //Singleton Pattern (Only one instance and globally accessable)

        //Only instance of gameworld
        private static GameWorld _instance;
        
        //Global access to gameworld
        public static GameWorld Instance()
        {
            if(_instance == null)
            {
                _instance = new GameWorld();
            }
            return _instance;
        }
        private Room _entrance;
        public Room Entrance { get { return _entrance; } }
        private Room _exit;
        private Room _trigger;

        private GameWorld()
        {
            _entrance = CreateWorld();

            //make method into an observer
            NotificationCenter.Instance.AddObserver("PlayerWillEnterRoom", PLayerWillEnterRoom);
            //make method into an observer
            NotificationCenter.Instance.AddObserver("PlayerDidEnterRoom", PlayerDidEnterRoom);
        }

        public void PLayerWillEnterRoom(Notification notification)
        {
            Player player = (Player)notification.Object;
            if(player.CurrentRoom == _exit)
            {

            }
            player.OutputMessage("*** The player will leave " + player.CurrentRoom.Tag);
        }

        public void PlayerDidEnterRoom(Notification notification)
        {
            Player player = (Player)notification.Object;
            if(player.CurrentRoom == _trigger)
            {
                //sends messages directly through player, instead of console
                //player.OutputMessage("*** The player entered the trigger room");
                _exit.SetExit("vortex", _entrance);
                player.OutputMessage("\n A new exit has been made!");
            }
            
        }

        private Room CreateWorld()
        {
            Room CastleEntrance = new Room("in front of a large castle");
            Room DiningHall = new Room("in a large dining hall");
            Room MainHall = new Room("in the main hall of the castle");
            Room TowerFloorOne = new Room("in the first floor of the tower");
            Room TowerFloorTwo = new Room("in the second floor of the tower");
            Room Kitchen = new Room("in the castle's large kitchen");
            Room Garden = new Room("in the garden");
            Room Vault = new Room("in the castle vault");
            Room ThroneRoom = new Room("in the throne room");

            CastleEntrance.SetExit("west", MainHall);

            MainHall.SetExit("east", CastleEntrance);
            MainHall.SetExit("south", DiningHall);
            MainHall.SetExit("west", Garden);
            MainHall.SetExit("north", TowerFloorOne);

            DiningHall.SetExit("west", Kitchen);
            DiningHall.SetExit("north", MainHall);

            Kitchen.SetExit("east", DiningHall);
            Kitchen.SetExit("north", ThroneRoom);

            ThroneRoom.SetExit("south", Kitchen);
            ThroneRoom.SetExit("north", Vault);
            ThroneRoom.SetExit("east", Garden);

            Garden.SetExit("west", ThroneRoom);
            Garden.SetExit("east", MainHall);

            Vault.SetExit("south", ThroneRoom);
            Vault.SetExit("east", TowerFloorOne);

            TowerFloorOne.SetExit("south", MainHall);
            TowerFloorOne.SetExit("west", Vault);
            TowerFloorOne.SetExit("north", TowerFloorTwo);

            TowerFloorTwo.SetExit("south", TowerFloorOne);

            //create trigger to activate trap room
            _trigger = Kitchen;
            
            //set final room (exit)
            _exit = TowerFloorTwo;

            //Set the Delegates
            Vault.Delegate = new TrapRoom("please");
            TowerFloorTwo.Delegate = new EchoRoom();

            //Set items for rooms
            Item pin = new Item("pin", 1, 1, false);
            Garden.Drop(pin);
            Item mouseToy = new Item("mouseToy", 2, 5);
            Garden.Drop(mouseToy);
            Item dogToy = new Item("dogToy", 10, 1);
            Garden.Drop(dogToy);
            Item dagger = new Item("dagger");
            Item diamond = new Item("diamond", 10, 10);
            dagger.AddDecorator(diamond);
            CastleEntrance.Drop(dagger);

            return CastleEntrance;
        }

    }
}
