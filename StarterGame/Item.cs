using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    public class Item : IItem
    {
        private string _name;
        public string Name { set { _name = value; } get { return _name; } }
        public string LongName { get { return Name + (_decorator != null ?", " +_decorator.LongName : ""); } }
        private IItem _decorator;
        private float _weight;
        public float Weight { set { _weight = value; } get { return _weight + (_decorator!=null?_decorator.Weight:0); } } //add decorator weight to item
        private float _value;
        public float Value { set { _value = value; } get { return _value + (_decorator != null ? _decorator.Value : 0); } } //add decorator weight to item
        private bool _pickupable;
        public bool Pickupable { set { _pickupable = value; } get { return _pickupable; } }
        public string Description { get { return Name + ", Weight = " + Weight + ", Value = " + Value; } }
        public Item(): this("No Name") { }
        public Item(string name): this(name,1f) { }
        public Item(string name, float weight): this(name, weight, 1) { }
        public Item(string name, float weight, float theValue): this(name, weight, theValue, true) { }
        public Item(string name, float weight, float theValue, bool pickup)
        {
            Name = name;
            Weight = weight;
            Value = theValue;
            Pickupable = pickup;
            _decorator = null;
        }
        public void AddDecorator(IItem decorator)
        {
            if(_decorator == null)
            {
                _decorator = decorator;
            }
            else
            {
                _decorator.AddDecorator(decorator);
            }
        }
    }
}
