using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data_GameSave
{
    public Data_GameSave(string saveName)
    {
        SaveName = saveName;
    }

    // ´æµµÃû×Ö
    public string SaveName;

    public Data_MapSave SaveMap;

    public List<Data_ThingSave> SaveThings;
}
