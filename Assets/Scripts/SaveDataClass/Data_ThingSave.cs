using ChenChen_BuildingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ĳλ����ĳ���壬�������������Ķ����λ��
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
