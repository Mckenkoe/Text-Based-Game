using System.Collections;
using System.Collections.Generic;
using System;

namespace StarterGame
{
    public interface IRoomDelegate
    {
        Dictionary<string, Room> ContainingRoomExits { set; }
        Room ContainingRoom { get; set; }
        Room GetExit(string exitName);
        string GetExits();
        string Description();
    }

    public class TrapRoom :IRoomDelegate
    {
        private string unlockedWord;
        public Room ContainingRoom { get; set; }  //Not workiong?
        public Dictionary<string, Room> ContainingRoomExits { set; get; }
        private Dictionary<string, Room> _containingRoomExits { set { _containingRoomExits = value; } }
        public TrapRoom(): this("test") { }
        //Designated constructor
        public TrapRoom(string theWord)
        {
            unlockedWord = theWord;
            NotificationCenter.Instance.AddObserver("PlayerSaidAWord", PlayerSaidAWord);
        }
        public Room GetExit(string exitName)
        {
            return null;
        }
        public string GetExits()
        {
            return "You are trapped.";
        }
        public string Description()
        {
            return "you are in a trap room." + "\n" + GetExits();
        }
        public void PlayerSaidAWord(Notification notification)
        {
            Player player = (Player)notification.Object;
            if (player.CurrentRoom == ContainingRoom)
            {
                Dictionary<string, Object> userInfo = notification.UserInfo;
                string word = (string)userInfo["word"];
                if(word == unlockedWord)
                {
                    ContainingRoom.Delegate = null;
                    player.OutputMessage("You are free!");
                }
                else
                {
                    player.OutputMessage("You said the wrong word.");
                }
            }        
        }
    }

    public class EchoRoom: IRoomDelegate
    {
        public EchoRoom()
        {
            NotificationCenter.Instance.AddObserver("PlayerSaidAWord", PlayerSaidAWord);
        }
        public Room ContainingRoom { get; set; }
        public Dictionary<string, Room> ContainingRoomExits { set; get; }
        private Dictionary<string, Room> _containingRoomExits { set { _containingRoomExits = value; } }
        public Room GetExit(string exitName)
        {
            Room room = null;
            ContainingRoomExits.TryGetValue(exitName, out room);
            return room;
        }
        public string GetExits()
        {
            string exitNames = "Exits: ";
            Dictionary<string, Room>.KeyCollection keys = ContainingRoomExits.Keys;
            foreach (string exitName in keys)
            {
                exitNames += " " + exitName;
            }
            return exitNames;
        }
        public string Description()
        {
            return "\nThis is an echo room\n"+ ContainingRoom.Tag+ ". \nYou are " + ContainingRoom.Tag + ".\n *** " + this.GetExits(); ;        }
        public void PlayerSaidAWord(Notification notification)
        {
            Player player = (Player)notification.Object;
            if(ContainingRoom == player.CurrentRoom)
            {
                Dictionary<string, object> userInfo = notification.UserInfo;
                string word = (string)userInfo["word"];
                player.OutputMessage("\n" + word + "..." + word + "..." + word + "\n");
            }
           
        }
    }

    public class Room
    {
        private Dictionary<string, Room> _exits;
        private Dictionary<string, IItem> _items;
        private string _tag;
        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        private IRoomDelegate _delegate;
        public IRoomDelegate Delegate
        {
            set
            {
                _delegate = value;
                if(value != null)
                {
                    _delegate.ContainingRoom = this;
                    _delegate.ContainingRoomExits = _exits;
                    //additems?
                }
            }
            get
            {
               return _delegate;
            }
        }

        public Room() : this("No Tag") { }

        // Designated Constructor
        public Room(string tag)
        {
            _exits = new Dictionary<string, Room>();
            _items = new Dictionary<string, IItem>();
            this.Tag = tag;
            
        }

        public void SetExit(string exitName, Room room)
        {   
            //if room is passed as null then remove the exitname
            if(room != null)
            {
                _exits[exitName] = room;
            }
            else
            {
                _exits.Remove(exitName);
            }
        }

        public Room GetExit(string exitName) { 
            if(Delegate == null)
            {
                Room room = null;
                _exits.TryGetValue(exitName, out room);
                return room;
            }
            return Delegate.GetExit(exitName);
        }

        public string GetExits()
        {
            if(Delegate == null)
            {
                string exitNames = "Exits: ";
                Dictionary<string, Room>.KeyCollection keys = _exits.Keys;
                foreach (string exitName in keys)
                {
                    exitNames += " " + exitName;
                }
                return exitNames;
            }
            else
            {
                return Delegate.GetExits();
            }
        }

        //add item to items in room 
        public void Drop(IItem item)
        {
            _items.Add(item.Name,item);
            
        }

        

        public IItem GetItem(string itemName)
        {
            if(_items.ContainsKey(itemName))
            {
                return _items[itemName];
            }
            return null;
        }
        public IItem Remove(string itemName)
        {
            if (_items.ContainsKey(itemName))
            {
                IItem gotItem = _items[itemName];
                _items.Remove(itemName);
                return gotItem;
            }
            return null;
        }

        

        public string GetItems()
        {
            string itemNames = "****Items: ";
            Dictionary<string, IItem>.KeyCollection keys = _items.Keys;
            foreach (string itemName in keys)
            {
                itemNames += " " + itemName;
            }
            return itemNames;
        }

        public string Description()
        {
            if(Delegate == null)
            {
                return "You are " + this.Tag + ".\n *** " + this.GetExits();
            }
            else
            {
                return Delegate.Description();
            }
        }
    }
}
