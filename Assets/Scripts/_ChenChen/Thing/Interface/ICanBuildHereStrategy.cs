using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{
    public interface ICanBuildHereStrategy
    {
        bool CanBuildHere(Bounds bounds);
    }
}
