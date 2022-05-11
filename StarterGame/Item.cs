using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    public class Item : IItem
    {
        private string _name;
        public string Name { set { _name = value; } get { return _name; } }
        public string LongName { get { return Name + (_decorator != null ? ", " + _decorator.LongName : ""); } }
        private IItem _decorator;
        private float _weight;
        public float Weight { set { _weight = value; } get { return _weight + (_decorator != null ? _decorator.Weight : 0); } } //add decorator weight to item
        private int _value;
        public int Value { set { _value = value; } get { return _value + (_decorator != null ? _decorator.Value : 0); } } //add decorator weight to item
        private bool _pickupable;
        private int _volume;
        public int Volume { set { _volume = value; }get { return _volume; } }

        public bool Pickupable { set { _pickupable = value; } get { return _pickupable; } }
        public string Description { get { return Name + ", Weight = " + Weight + ", Value = " + Value+", Volume: "+Volume; } }
        public Item() : this("No Name") { }
        public Item(string name) : this(name, 1f) { }
        public Item(string name, float weight) : this(name, weight, 1) { }
        public Item(string name, float weight, int theValue) : this(name, weight, theValue, 1) { }
        public Item(string name, float weight, int theValue,int volume) : this(name, weight, theValue,volume, true) { }
        public Item(string name, float weight, int theValue, int volume,bool pickup)
        {
            Name = name;
            Weight = weight;
            Value = theValue;
            Volume = volume;
            Pickupable = pickup;
            _decorator = null;
        }
        public void AddDecorator(IItem decorator)
        {
            if (_decorator == null)
            {
                _decorator = decorator;
            }
            else
            {
                _decorator.AddDecorator(decorator);
            }
        }
    }

    public class ItemContainer : Item
    {
        private Dictionary<string, IItem> _items;
        public ItemContainer() : base()
        {
            _items = new Dictionary<string, IItem>();
            this.Name = "";
        }

        public void Add(IItem item)
        {
            _items.Add(item.Name, item);
        }
        public IItem GetItem(string item)
        {
            _items.TryGetValue(item, out IItem gotItem);
            return gotItem;
        }

        public IItem Remove(IItem item)
        {
            _items.Remove(item.Name, out IItem gotItem);
            return gotItem;
        }

        public float TotalWeight()
        {
            float TotalWeight = 0;
            Dictionary<string, IItem>.KeyCollection keys = _items.Keys;
            foreach (string itemName in keys)
            {
                TotalWeight = TotalWeight + _items[itemName].Weight;
            }
            return TotalWeight;
        }

        public float TotalVolume()
        {
            int TotalVolume = 0;
            Dictionary<string, IItem>.KeyCollection keys = _items.Keys;
            foreach (string itemName in keys)
            {
                TotalVolume = TotalVolume + _items[itemName].Volume;
            }
            return TotalVolume;
        }

        public string ListItems()
        {
            string itemNames = "";
            foreach (string itemName in _items.Keys)
            {
                itemNames = itemNames+" " + itemName;
            }
            return itemNames;
        }
    }
}
