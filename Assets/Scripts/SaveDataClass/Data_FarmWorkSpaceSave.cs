using ChenChen_CropSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data_FarmWorkSpaceSave
{
    public string workSpaceName;

    public string cropName; //该种植区种植的作物

    public Bounds bounds;

    public List<Data_CropSave> crops;

    public Data_FarmWorkSpaceSave(string workSpaceName, string cropName, Bounds bounds)
    {
        this.workSpaceName = workSpaceName;
        this.cropName = cropName;
        this.bounds = bounds;
        this.crops = new();
    }
}
