using System.Collections;
using System.Collections.Generic;
using System;

namespace StarterGame
{
    public class Player
    {
        Random rand = new Random();
        private int _health = 25;
        private int _strength = 3;
        private ItemContainer _items;
        private float _weightAllowed;
        public float WeightAllowed { get { return _weightAllowed; } set { _weightAllowed = value; } }
        private int _volumeAllowed;
        public int VolumeAllowed { get { return _volumeAllowed; } set { _volumeAllowed = value; } }
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
            _items = new ItemContainer();
            _weightAllowed = 20;
            _volumeAllowed = 5;
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
            this.OutputMessage("\n" + this.CurrentRoom.GetBeasts());
        }

        //checks if item is in room, then checks if item is in inventory
        //prints item's description (name and weight) 
        public void Look(string name)
        {
            if(this.CurrentRoom.GetItem(name)!= null || _items.GetItem(name)!=null)
            {
                IItem gotRoomItem = this.CurrentRoom.GetItem(name);
                IItem gotInvItem = _items.GetItem(name);
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
                    ErrorMessage("The item " + name + " is not here...");
                }
            }
            else if (this.CurrentRoom.GetBeast(name) != null)
            {
                this.WarningMessage(this.CurrentRoom.GetBeast(name).Description);
                this.WarningMessage("Items: "+ this.CurrentRoom.GetBeast(name).GetBeastItem());
            }
            
        }

        //yucky code but item is checked if its in the room, then if its pickupable, then if theres room in the inventory
        public void Pickup(string itemName)
        {
            if(this.CurrentRoom.GetItem(itemName) != null)
            {
                IItem gotItem = this.CurrentRoom.GetItem(itemName);
                if (gotItem.Pickupable)
                {
                    if((((_items.TotalWeight()+gotItem.Weight)  < _weightAllowed)&&_items.TotalVolume()<_volumeAllowed)&&(gotItem.Name != "backpack"))
                    {
                        _items.Add(gotItem);
                        this.InformationMessage("Picked up " + itemName + "!");
                        this.CurrentRoom.Remove(gotItem);
                        if(itemName == "dagger")
                        {
                            _strength = _strength + gotItem.Value;
                        }
                    }
                    else if (gotItem.Name == "backpack") 
                    {
                        _items.Add(gotItem);
                        this.WeightAllowed = 50;
                        this.VolumeAllowed = 15;
                        this.CurrentRoom.Remove(gotItem);
                        this.InformationMessage("Picked up a special item! Now you can carry up to 50 weight!");
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

        //if item is in inventory, place item in room then remove item from inventory (aka items dictionary)
        public bool Place(string itemName)
        {
            IItem gotItem = _items.GetItem(itemName);
            if (gotItem!=null)
            {
                this.CurrentRoom.Drop(gotItem);
                _items.Remove(gotItem);
                this.InformationMessage("Placed "+itemName+ "!");
                Dictionary<string, Object> userInfo = new Dictionary<string, object>();
                userInfo["item"] = gotItem;
                Notification notification = new Notification("PlayerDroppedItem", this, userInfo);
                NotificationCenter.Instance.PostNotification(notification);


                Notification notification1 = new Notification("PlayerDroppedItemWin", this);
                NotificationCenter.Instance.PostNotification(notification1);
                return true;
            }
            else
            {
                ErrorMessage("You do not have that item.");
            }
            return false;
        }

        //prints out the items the player has picked up
        public void Inventory()
        {
            this.OutputMessage("   Your Inventory\n **-------------**");
            this.OutputMessage(_items.ListItems());
            this.OutputMessage("\nTotal weight: " + _items.TotalWeight());
        }

        public void Fight(string name)
        {
            Beast gotBeast = this.CurrentRoom.GetBeast(name);
            if (gotBeast != null)
            {
                while(_health > 0 && gotBeast.Health > 0)
                {
                    int roundStr = rand.Next(1, _strength);
                    this.OutputMessage("You have " + _health + " health");
                    this.OutputMessage("The " + name + " has " + gotBeast.Health + " health");
                    gotBeast.LoseHealth(roundStr);
                    this.InformationMessage("You attacked with "+ roundStr+ " damage");
                    this.ErrorMessage(name + " attacked with " + gotBeast.Strength+" damage");
                    _health = _health - gotBeast.Attack();
                    //pause fight for 1 seconds
                    System.Threading.Thread.Sleep(1000);
                }
                if(_health <= 0)
                {
                    ErrorMessage("you have no..health");
                    
                }
                else if(gotBeast.Health <= 0)
                {
                    gotBeast.Die();
                    InformationMessage("You defeated the " + gotBeast.Name+"\n\n Did it drop any items?");

                }
            }
            else
            {
                this.WarningMessage("There is no " + name + " here.");
            }
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

        public void WinMessage(string message)
        {
            ColoredMessage(message, ConsoleColor.Green);
        }
    }

}
