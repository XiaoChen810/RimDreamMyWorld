using ChenChen_AI;
using System;
using System.IO;
using UnityEngine;

public static class StaticPawnDef
{
    private static readonly string NameFilePath = "String/Name/Pawn_Flower";
   
    private static string[] LoadNamesFromFile()
    {
        TextAsset nameFile = Resources.Load<TextAsset>(NameFilePath);
        if (nameFile != null)
        {
            return nameFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            throw new FileNotFoundException($"文件 {NameFilePath} 未找到。");
        }
    }

    public static string GetRandomPawnName()
    {
        System.Random rand = new System.Random();
        string[] names = LoadNamesFromFile();    
        int randomIndexForName = rand.Next(names.Length);
        return names[randomIndexForName];
    }
}
