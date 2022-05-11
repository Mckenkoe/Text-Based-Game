using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    public interface IBeast
    {
        //monster character interface
        string Name { get; }
        int Health { get; }
        int Strength { get; }
        string Description { get; }
    }
}
