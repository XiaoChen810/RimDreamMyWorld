using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Data_MonsterSave
{
    public int indexID;

    public Vector2 position;

    public Data_MonsterSave(int indexID, Vector2 position)
    {
        this.indexID = indexID;
        this.position = position;
    }
}
