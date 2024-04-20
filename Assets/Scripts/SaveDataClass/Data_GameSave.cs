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
    // ´æµµÖÖ×Ó
    public string SaveSeed;

}
