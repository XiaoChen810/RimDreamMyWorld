using UnityEngine;

[System.Serializable]
public class Data_GameSave
{
    public Data_GameSave(string saveName)
    {
        SaveName = saveName;
    }

    // �浵����
    public string SaveName;
    // �浵����
    public string SaveSeed;

}
