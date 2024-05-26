using System.Collections.Generic;
using UnityEngine;
using ChenChen_AI;

[System.Serializable]
public class Data_GameSave
{
    public Data_GameSave(string saveName, string saveDate)
    {
        SaveName = saveName;
        SaveDate = saveDate;
        SaveThings = new List<Data_ThingSave>();
        SavePawns = new List<Data_PawnSave>();
        SaveMonster = new List<Data_MonsterSave>();
        SaveFarmWorkSpace = new List<Data_FarmWorkSpaceSave>();
    }

    // ´æµµÃû×Ö
    public string SaveName;

    public string SaveDate;

    [Header("Time")]
    public int currentSeason = 1; 
    public int currentDay = 1; 
    public int currentHour = 0; 
    public int currentMinute = 0; 

    public Data_MapSave SaveMap;

    public List<Data_ThingSave> SaveThings;

    public List<Data_PawnSave> SavePawns;

    public List<Data_MonsterSave> SaveMonster;

    public List<Data_FarmWorkSpaceSave> SaveFarmWorkSpace;
}
