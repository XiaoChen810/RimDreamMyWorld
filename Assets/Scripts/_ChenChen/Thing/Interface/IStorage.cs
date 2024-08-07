using ChenChen_Core;
using System;
using System.Collections.Generic;

namespace ChenChen_Thing
{
    public interface IStorage
    {
        Dictionary<string, int> Bag { get; }

        void Put(string name, int num);

        int Get(string name, int num);
    }
}
