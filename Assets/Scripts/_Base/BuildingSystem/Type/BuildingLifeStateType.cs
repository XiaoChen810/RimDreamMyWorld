using System;
using UnityEngine;

namespace ChenChen_BuildingSystem
{
    [Flags]
    public enum BuildingLifeStateType : byte
    {
        None = 0,
        //等待建造
        MarkBuilding = 1,
        //完成建造
        FinishedBuilding = 2,
        //等待拆除
        MarkDemolished = 3,
        //完成拆除
        FinishedDemolished = 4,
    }
}