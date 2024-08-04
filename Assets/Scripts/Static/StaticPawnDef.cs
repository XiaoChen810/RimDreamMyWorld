using ChenChen_AI;
using System;
using System.IO;
using UnityEngine;

public static class StaticPawnDef
{
    private static readonly string NameFilePath = "String/Name/Pawn_Flower";
    private static readonly string[] Descriptions = { "普普通通的光头", "活泼的单马尾女孩", "热血的红毛", "猥琐的黄毛", "单纯的平头男", "可爱的小男孩" };
    private static readonly string[] PrefabPaths = { "Prefabs/Pawn/Bald", "Prefabs/Pawn/SinglePonytail", "Prefabs/Pawn/RedHair", "Prefabs/Pawn/YellowHair", "Prefabs/Pawn/CrewCut", "Prefabs/Pawn/Boy" };

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

    public static PawnKindDef GetRandomPawn()
    {
        System.Random rand = new System.Random();

        string[] names = LoadNamesFromFile();
        int randomIndexForName = rand.Next(names.Length);
        int randomIndexForDescription = rand.Next(Descriptions.Length);

        return new PawnKindDef(
            pawnName: names[randomIndexForName],
            pawnDescription: Descriptions[randomIndexForDescription],
            prefabPath: PrefabPaths[randomIndexForDescription]
        );
    }
}
