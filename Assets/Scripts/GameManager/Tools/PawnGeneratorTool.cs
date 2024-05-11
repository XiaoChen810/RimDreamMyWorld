using ChenChen_AI;
using System;
using UnityEngine;

public class PawnGeneratorTool
{
    public GameManager GameManager;
    public PawnGeneratorTool(GameManager gameManager)
    {
        GameManager = gameManager;
    }

    /// <summary>
    /// 生成Pawn
    /// </summary>
    /// <param name="kindDef"></param>
    /// <param name="position"></param>
    /// <param name="attribute"></param>
    /// <param name="prefab"></param>
    public Pawn GeneratePawn(Vector3 position, PawnKindDef kindDef, PawnInfo info, PawnAttribute attribute)
    {
        // 尝试初始化游戏物体
        if (!TryInitGameObject(kindDef, out GameObject newPawnObject))
        {
            Debug.LogWarning("初始化游戏物体失败");
            return null;
        }
        // 尝试获取Pawn组件并赋值
        if (newPawnObject.TryGetComponent<Pawn>(out Pawn pawn))
        {
            pawn.Def = kindDef;
            pawn.Info = info;
            pawn.Attribute = attribute;
            GameManager.PawnsList.Add(newPawnObject);
            return pawn;
        }
        else
        {
            Debug.LogError("获取Pawn组件失败");
            return null;
        }

        bool TryInitGameObject(PawnKindDef kindDef, out GameObject result)
        {
            GameObject prefab = null;
            if (prefab == null)
            {
                prefab = Resources.Load<GameObject>(kindDef.PrefabPath);
            }
            if (prefab == null)
            {
                PawnKindDef ramdomPawnKindDef = StaticPawnDef.GetRandomPawn();
                kindDef.PrefabPath = ramdomPawnKindDef.PrefabPath;
                kindDef.PawnDescription = ramdomPawnKindDef.PawnDescription;
                prefab = Resources.Load<GameObject>(kindDef.PrefabPath);
                Debug.Log("从现有Prefab中随机挑选了一个");
            }
            if (prefab == null)
            {
                Debug.LogError("Prefab is null");
                result = null;
                return false;
            }
            result = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
            result.name = kindDef.PawnName;
            result.transform.SetParent(GameManager.transform, false);
            return true;
        }
    }

    public void LoadScenePawnFromSave(Data_GameSave data_GameSave)
    {
        foreach (var pawnSave in data_GameSave.SavePawns)
        {
            Data_PawnSave newPawnSave = (Data_PawnSave) pawnSave.Clone();
            Pawn newPawn = GeneratePawn(newPawnSave.Position, newPawnSave.PawnKindDef, newPawnSave.PawnInfo, newPawnSave.PawnAttribute);
        }
    }

    public bool RemovePawn(Pawn pawn)
    {
        for (int i = 0; i < GameManager.PawnsList.Count; i++)
        {
            if (GameManager.PawnsList[i] == pawn.gameObject)
            {
                UnityEngine.Object.Destroy(GameManager.PawnsList[i].gameObject);
                GameManager.PawnsList.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public void StartSelect()
    {
        GameManager.PawnWhenStartList.Add(GeneratePawn(new Vector3(-5, 1.3f, 0), StaticPawnDef.GetRandomPawn(), new PawnInfo(), null));
        GameManager.PawnWhenStartList.Add(GeneratePawn(new Vector3(0, 1.3f, 0), StaticPawnDef.GetRandomPawn(), new PawnInfo(), null));
        GameManager.PawnWhenStartList.Add(GeneratePawn(new Vector3(5, 1.3f, 0), StaticPawnDef.GetRandomPawn(), new PawnInfo(), null));
        foreach (var pawn in GameManager.PawnWhenStartList)
        {
            pawn.Def.StopUpdate = true;
        }
    }

    public void EndSelect()
    {
        for (int i = 0; i < GameManager.PawnWhenStartList.Count; i++)
        {
            RemovePawn(GameManager.PawnWhenStartList[i]);
        }
        GameManager.PawnWhenStartList.Clear();
    }
}

