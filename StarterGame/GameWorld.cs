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
            _trigger = Vault;
            
            //set final room (exit)
            _exit = TowerFloorTwo;

            //Set the Delegates
            Vault.Delegate = new TrapRoom("open");
            TowerFloorTwo.Delegate = new WinRoom();

            //Create items
            Item flour = new Item("flour", 5,0,1);
            Item sugar = new Item("sugar", 5,0,1);
            Item milk = new Item("milk", 5,3);
            Item egg = new Item("egg", 5,0,1);
            Item chocolate = new Item("chocolate", 5,0,2);
            Item bagel = new Item("bagel", 3, 10,0,false);
            Item apple = new Item("apple", 2, 5,0, false);
            Item banana = new Item("banana", 2, 5,0, false);
            Item grape = new Item("grape", 1, 5,0,false);

            //item with decorator
            Item dagger = new Item("dagger");
            Item diamond = new Item("diamond", 10, 10);
            dagger.AddDecorator(diamond);

            //Set items for rooms
            CastleEntrance.Drop(dagger);

            Garden.Drop(flour);
            DiningHall.Drop(sugar);
            ThroneRoom.Drop(chocolate);
            TowerFloorOne.Drop(bagel);
            MainHall.Drop(apple);
            Kitchen.Drop(banana);
            Kitchen.Drop(grape);


            //set beasts(monsters/ enemies) and give items
            Beast cow = new Beast("cow", 3, 17);
            cow.GiveBeastItem(milk);
            Garden.PutBeast(cow);
            Beast chicken = new Beast("chicken", 2, 6);
            chicken.GiveBeastItem(egg);
            Kitchen.PutBeast(chicken);
            //Beast cat = new Beast("cat",4, 12);

            return CastleEntrance;
        }

    }
}
