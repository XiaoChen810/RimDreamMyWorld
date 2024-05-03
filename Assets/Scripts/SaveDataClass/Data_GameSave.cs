using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data_GameSave
{
    public Data_GameSave(string saveName, string saveDate)
    {
        SaveName = saveName;
        SaveDate = saveDate;
        SaveThings = new List<Data_ThingSave>();
    }

    // ´æµµÃû×Ö
    public string SaveName;

    public string SaveDate;

    public Data_MapSave SaveMap;

    public List<Data_ThingSave> SaveThings;
}
