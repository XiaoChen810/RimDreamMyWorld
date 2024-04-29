using ChenChen_BuildingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 表明某位置有某物体，里面包含了物体的定义和位置
/// </summary>
[System.Serializable]
public class Data_ThingSave
{
    public ThingDef ThingDef;
    public Vector2Int ThingPos;
    public Quaternion ThingRot;
    public string MapName;

    public Data_ThingSave(ThingDef thingDef, Vector2Int thingPos, Quaternion thingRot, string mapName)
    {
        ThingDef = thingDef;
        ThingPos = thingPos;
        ThingRot = thingRot;
        MapName = mapName;
    }
}
