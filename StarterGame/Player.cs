using System.Collections;
using System.Collections.Generic;
using System;

namespace StarterGame
{
    public class Player
    {
        private Dictionary<string, IItem> _items;
        private float _totalWeight;
        private Stack<Room> visitedRooms = new Stack<Room>();
        private Room _currentRoom = null;
        public Room CurrentRoom
        {
            get
            {
                return _currentRoom;
            }
            set
            {
                _currentRoom = value;
            }
        }

        public Player(Room room)
        {
            _currentRoom = room;
            _items = new Dictionary<string, IItem>();
            _totalWeight = 20;
            //NotificationCenter.Instance.AddObserver("PlayerPickedUpItem", PlayerPickedUpItem);
            //NotificationCenter.Instance.AddObserver("PlayerPlacedItem", PlayerPlacedItem);
        }

        public void WaltTo(string direction)
        {
            Room nextRoom = this.CurrentRoom.GetExit(direction);
            if (nextRoom != null)
            {
                Notification notification = new Notification("PlayerWillEnterRoom", this);
                NotificationCenter.Instance.PostNotification(notification);

                //Add the room to be exited to visitedRooms stack before left
                visitedRooms.Push(this.CurrentRoom);

                this.CurrentRoom = nextRoom;
                notification = new Notification("PlayerDidEnterRoom", this);
                NotificationCenter.Instance.PostNotification(notification);
                this.OutputMessage("\n" + this.CurrentRoom.Description());

                //Print the last room visited to player, to allow them to know what room they would go back to with BackCommand
                Room lastRoom = visitedRooms.Peek();
                this.OutputMessage("Last room: " + lastRoom.Tag);

            }
            else
            {
                this.OutputMessage("\nThere is no door on " + direction);
            }
        }

        public void WaltBack()
        {
            if (visitedRooms.Count != 0)
            {
                _currentRoom = visitedRooms.Pop();
                this.OutputMessage("\n" + this._currentRoom.Description());
            }
            else
            {
                this.OutputMessage("Can't go back further");
            }
        }

        public void Say(string word)
        {
            this.OutputMessage("\n" + word + "\n");
            Dictionary<string, Object> userInfo = new Dictionary<string, object>();
            userInfo["word"] = word;
            Notification notification = new Notification("PlayerSaidAWord", this, userInfo);
            NotificationCenter.Instance.PostNotification(notification);
        }

        //Look in room (general)
        // prints room description and items in room
        public void Look()
        {
            this.OutputMessage("\n" + this.CurrentRoom.Description());
            this.OutputMessage("\n" + this.CurrentRoom.GetItems());
        }

        //checks if item is in room, then checks if item is in inventory
        //prints item's description (name and weight)
        public void Look(string itemName)
        {
            IItem gotRoomItem = this.CurrentRoom.GetItem(itemName);
            _items.TryGetValue(itemName, out IItem gotInvItem);
            if (gotRoomItem != null) //If item in current room, output description
            {
                this.OutputMessage(gotRoomItem.Description);
            }
            else if (gotInvItem != null) //if item in inventory, output descrtiption 
            {
                this.OutputMessage(gotInvItem.Description);
            }
            else
            {
                ErrorMessage("The item: " + itemName + " is not here...");
            }
        }

        public void Pickup(string itemName)
        {
            if(this.CurrentRoom.GetItem(itemName) != null)
            {
                IItem gotItem = this.CurrentRoom.GetItem(itemName);
                if (gotItem.Pickupable)
                {
                    if((_totalWeight-gotItem.Weight) > 0)
                    {
                        _items.Add(itemName, gotItem);
                        _totalWeight = _totalWeight - gotItem.Weight;
                        this.InformationMessage("Picked up " + itemName + "!");
                        //Notification notification = new Notification("PlayerPickedupItem", this);
                        //NotificationCenter.Instance.PostNotification(notification);
                        this.CurrentRoom.Remove(itemName);
                    }
                    else
                    {
                        ErrorMessage(gotItem.Name + " is too heavy for you right now! You need to get rid of some items to carry it.");
                    }
                }
                else
                {
                    ErrorMessage(itemName + " cannot be picked up.");
                }
            }
            else
            {
                ErrorMessage("The item: " + itemName + " is not here...");
            }
            
        }

        /* not necessary?
        public void PlayerPickedUpItem(Notification notification)
        {
            Player player = (Player)notification.Object;
            player.InformationMessage("Picked it up!");
        }
        */

        //if item is in inventory, place item in room then remove item from inventory (aka items dictionary)
        public void Place(string itemName)
        {
            if(_items.ContainsKey(itemName))
            {
                this.CurrentRoom.Drop(_items[itemName]);
                _items.Remove(itemName);
                this.InformationMessage("Placed "+itemName+ "!");
                //Notification notification = new Notification("PlayerPlacedItem", this);
                //NotificationCenter.Instance.PostNotification(notification);
            }
            else
            {
                ErrorMessage("You do not have that item.");
            }
        }

        /* not necessary?
        public void PlayerPlacedItem(Notification notification)
        {
            Player player = (Player)notification.Object;
            player.InformationMessage("Placed it!");
        }
        */

        //prints out the items the player has picked up
        public void Inventory()
        {
            string itemNames = "***Your Inventory\n **-------------**\n";
            float weights = 0;
            Dictionary<string, IItem>.KeyCollection keys = _items.Keys;
            foreach (string itemName in keys)
            {
                itemNames += " " + itemName+"\n";
                weights = weights + _items[itemName].Weight;
            }
            
            this.OutputMessage(itemNames);
            this.OutputMessage("Total weight: " + weights);
        }

        public void OutputMessage(string message)
        {
            Console.WriteLine(message);
        }
        public void ColoredMessage(string message, ConsoleColor color)
        {
            ConsoleColor oldcolor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            OutputMessage(message);
            Console.ForegroundColor = oldcolor;
        }

        public void InformationMessage(string message)
        {
            ColoredMessage(message, ConsoleColor.Blue);
        }

        public void WarningMessage(string message)
        {
            ColoredMessage(message, ConsoleColor.Yellow);
        }
        public void ErrorMessage(string message)
        {
            ColoredMessage(message, ConsoleColor.Red);
        }
    }

}
