using ChenChen_Thing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ĳλ����ĳ���壬�����������������֣�λ�ã���ת��״̬
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
