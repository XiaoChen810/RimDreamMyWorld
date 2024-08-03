using System;
using UnityEngine;

namespace ChenChen_Thing
{
    public enum BuildingLifeStateType
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