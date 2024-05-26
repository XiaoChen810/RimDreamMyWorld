using ChenChen_CropSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Data_CropSave
{
    public Vector2 position;

    public float CurNutrient;

    public int CurPeriodIndex;

    public Data_CropSave(Vector2 position, float curNutrient, int curPeriodIndex)
    {
        this.position = position;
        CurNutrient = curNutrient;
        CurPeriodIndex = curPeriodIndex;
    }
}
