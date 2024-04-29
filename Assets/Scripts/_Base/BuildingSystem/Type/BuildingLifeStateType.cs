using System;
using UnityEngine;

namespace ChenChen_BuildingSystem
{
    public enum BuildingLifeStateType : Byte
    {
        None,
        //等待建造
        MarkBuilding,
        //完成建造
        FinishedBuilding,
        //等待拆除
        MarkDemolished,
        //完成拆除
        FinishedDemolished
    }
}