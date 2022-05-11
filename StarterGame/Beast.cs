using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    public class Beast : IBeast
    {
        //monster character
        private IItem _item;
        private string _name;
        private Room _currentRoom = null;
        public Room CurrentRoom { set { _currentRoom = value; } get { return _currentRoom; } }
        public string Name { set { _name = value; } get { return _name; } }
        private int _health;
        public virtual int Health { set { _health = value; } get { return _health; } }
        private int _strength;
        public virtual int Strength { set { _strength = value; } get { return _strength; } }
        public virtual string Description { get {return Name + ", Strength: "+ Strength+", Health: "+ Health; } }

        public Beast() : this("no name") { }
        public Beast(string name): this(name, 1) { }
        public Beast(string name, int strength):this(name, strength, 10) { }

        public Beast(string name, int strength, int health)
        {
            _name = name;
            _strength = strength;
            _health = health;
            _item = null;
        }

       

        public void GiveBeastItem(IItem item)
        {
            _item = item;
        }

        public IItem TakeBeastItem()
        {
            IItem gotItem = _item;
            _item = null;
            return gotItem;
        }

        public string GetBeastItem()
        {
           return "The beast is holding: "+ _item.Name;
        }

        public void LoseHealth(int playerStrength)
        {
            _health = _health - playerStrength;
        }

        public int Attack()
        {
            return _strength;
        }

        public void Die()
        {
            CurrentRoom.Drop(this.TakeBeastItem());
            CurrentRoom.RemoveBeast(this);
        }

    }

    public class BeastContainer : Beast
    {
        private Dictionary<string, Beast> _beasts;
        public BeastContainer() : base()
        {
            _beasts = new Dictionary<string, Beast>();
            this.Name = "";
        }

        public void Add(Beast beast)
        {
            _beasts.Add(beast.Name, beast);
        }

        public Beast Remove(Beast beast)
        {
            _beasts.Remove(beast.Name, out Beast gotBeast);
            return gotBeast;
        }

        public Beast GetBeast(string beastName)
        {
            _beasts.TryGetValue(beastName, out Beast gotBeast);
            return gotBeast;
        }

        public string ListBeasts()
        {
            string beastNames = "****Beasts: ";
            //Dictionary<string, Beast>.KeyCollection keys = _beasts.Keys;
            foreach (string beastName in _beasts.Keys)
            {
                beastNames = beastNames+" " + beastName;
            }
            return beastNames;
        }

        

    }
}
