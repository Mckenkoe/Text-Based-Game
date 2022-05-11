using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    public interface IItem
    {
        string Name { get; }
        string LongName { get; }
        float Weight { get; }
        int Value { get; }
        int Volume { get; }
        bool Pickupable { get; }
        string Description { get; }
        void AddDecorator(IItem decorator);
    }
}
