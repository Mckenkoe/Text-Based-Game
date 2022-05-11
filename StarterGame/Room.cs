using System.Collections;
using System.Collections.Generic;
using System;

namespace StarterGame
{
    //delegate pattern
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
        public Room ContainingRoom { get; set; } 
        public Dictionary<string, Room> ContainingRoomExits { set; get; }
        private Dictionary<string, Room> _containingRoomExits { set { _containingRoomExits = value; } }
        private Dictionary<string, IItem> _unlockedItems;
        private ItemContainer _items;
        private Dictionary<string, IItem> _unlockItems;
        private string _unlockedWord;
        public TrapRoom(): this("test") { }
        //Designated constructor
        public TrapRoom(string word)
        {
            _unlockedItems = new Dictionary<string, IItem>();
            Item egg = new Item("egg", 5);
            _unlockedItems.Add("egg",egg);
            Item backpack = new Item("backpack", 0, 35);
            _unlockedItems.Add("backpack", backpack);

            _unlockedWord = word;
            _items = new ItemContainer();

            NotificationCenter.Instance.AddObserver("PlayerDroppedItem", PlayerDroppedItem);
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
            return "you are stuck now. Place egg to get special item and get out, or say open to just get out." + "\n" + GetExits();
        }

        public void PlayerDroppedItem(Notification notification)
        {
            Player player = (Player)notification.Object;
            if (player.CurrentRoom == ContainingRoom)
            {
                Dictionary<string, Object> userInfo = notification.UserInfo;
                IItem item = (IItem)userInfo["item"];
                if (_unlockedItems.ContainsKey(item.Name))
                {
                    ContainingRoom.Delegate = null;
                    player.OutputMessage("You are free!");
                    ContainingRoom.Drop(_unlockedItems["backpack"]); 
                }
            }
        }
        public void PlayerSaidAWord(Notification notification)
        {
            Player player = (Player)notification.Object;
            if (player.CurrentRoom == ContainingRoom)
            {
                Dictionary<string, Object> userInfo = notification.UserInfo;
                string word = (string)userInfo["word"];
                if (word == _unlockedWord)
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

    public class WinRoom:IRoomDelegate
    {
        private Dictionary<string, Room> _containingRoomExits { set { _containingRoomExits = value; } }
        public Dictionary<string, Room> ContainingRoomExits { set; get; }
        private ItemContainer _items;
        public Room ContainingRoom { get; set; }
        public WinRoom()
        {
            _items = new ItemContainer();

            NotificationCenter.Instance.AddObserver("PlayerDroppedItemWin", PlayerDroppedItemWin);
        }
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
        public void PlayerDroppedItemWin(Notification notification)
        {
            Player player = (Player)notification.Object;
            if (player.CurrentRoom == ContainingRoom)
            {
                IItem milk = ContainingRoom.GetItem("milk");
                IItem egg = ContainingRoom.GetItem("egg");
                IItem sugar= ContainingRoom.GetItem("sugar");
                IItem flour = ContainingRoom.GetItem("flour");
                IItem chocolate = ContainingRoom.GetItem("chocolate");
                if (milk != null && egg != null && sugar != null && flour != null && chocolate != null)
                {
                    player.WinMessage("\n\n-------\n\nYOU WON!!\n\n-------\n\n");
                }
            }
        }
        public string Description()
        {
            return "Place all required ingredients to escape the castle!\nIngredients:\n- Milk\n- Flour\n- Sugar\n- egg\n- chocolate";
        }
    }

    public class Room
    {
        private Dictionary<string, Room> _exits;
        private ItemContainer _items;
        private BeastContainer _beasts;
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
            _items = new ItemContainer();
            _beasts = new BeastContainer();
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
            _items.Add(item);
            
        }

        public IItem GetItem(string itemName)
        {
            return _items.GetItem(itemName);
        }
        public IItem Remove(IItem item)
        {
            return _items.Remove(item);
        }

        public void PutBeast(Beast beast)
        {
            _beasts.Add(beast);
            _beasts.GetBeast(beast.Name).CurrentRoom = this;
        }

        public Beast RemoveBeast(Beast beast)
        {
            return _beasts.Remove(beast);
            _beasts.Remove(beast);
        }

        public Beast GetBeast(string beastName)
        {
           return _beasts.GetBeast(beastName);
        }

        public string GetBeasts()
        {
            return _beasts.ListBeasts();
        }

        public string GetItems()
        {
            return "****Items: "+ _items.ListItems();
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
