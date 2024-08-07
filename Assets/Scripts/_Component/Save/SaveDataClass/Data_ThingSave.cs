using ChenChen_Thing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 表明某位置有某物体，里面包含了物体的名字，位置，旋转，状态
/// </summary>
[System.Serializable]
public struct Data_ThingSave
{
    public string DefName;
    public Vector2 ThingPos;
    public Quaternion ThingRot;
    public BuildingLifeStateType LifeState;

    public Data_ThingSave(string defName, Vector2 thingPos, Quaternion thingRot, BuildingLifeStateType lifeState)
    {
        DefName = defName;
        ThingPos = thingPos;
        ThingRot = thingRot;
        LifeState = lifeState;
    }
}
