using ChenChen_AI;
using System.Collections.Generic;
using UnityEngine;

public class PawnGeneratorTool : MonoBehaviour
{
    [SerializeField] private List<GameObject> _pawnsList = new List<GameObject>();
    /// <summary>
    /// 游戏内全部的Pawn的GameObject列表
    /// </summary>
    public List<GameObject> PawnsList
    {
        get { return _pawnsList; }
    }

    [SerializeField] private List<Pawn> _pawnWhenStartList = new List<Pawn>();
    /// <summary>
    /// 仅当进行人物选择时使用的角色列表
    /// </summary>
    public List<Pawn> PawnWhenStartList
    {
        get { return _pawnWhenStartList; }
    }

    /// <summary>
    /// 生成Pawn，并添加到PawnsList
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
            pawn.Def = (PawnKindDef)kindDef.Clone();
            pawn.Info = (PawnInfo)info.Clone();
            pawn.Attribute = (PawnAttribute)attribute.Clone();
            PawnsList.Add(newPawnObject);
            return pawn;
        }
        else
        {
            Debug.LogError("获取Pawn组件失败");
            return null;
        }

        // 根据定义生成一个Pawn
        bool TryInitGameObject(PawnKindDef kindDef, out GameObject result)
        {
            GameObject prefab = null;
            if (prefab == null)
            {
                prefab = Resources.Load<GameObject>(kindDef.PrefabPath);
            }
            if (prefab == null)
            {
                kindDef = StaticPawnDef.GetRandomPawn();
                prefab = Resources.Load<GameObject>(kindDef.PrefabPath);
            }
            if (prefab == null)
            {
                Debug.LogError("Prefab is null");
                result = null;
                return false;
            }
            result = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
            result.name = kindDef.PawnName;
            result.transform.SetParent(transform, false);
            return true;
        }
    }

    public void LoadScenePawnFromSave(Data_GameSave data_GameSave)
    {
        foreach (var pawnSave in data_GameSave.SavePawns)
        {
            Data_PawnSave newPawnSave = (Data_PawnSave) pawnSave.Clone();
            _ = GeneratePawn(newPawnSave.Position, newPawnSave.PawnKindDef, newPawnSave.PawnInfo, newPawnSave.PawnAttribute);
        }
    }

    public bool RemovePawn(Pawn pawn)
    {
        for (int i = 0; i < PawnsList.Count; i++)
        {
            if (PawnsList[i] == pawn.gameObject)
            {
                UnityEngine.Object.Destroy(PawnsList[i].gameObject);
                PawnsList.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public void StartSelect()
    {
        // 在三个固定位置生成随机人物
        PawnWhenStartList.Add(GeneratePawn(new Vector3(-5, 1.3f, 0), StaticPawnDef.GetRandomPawn(), new PawnInfo(), new PawnAttribute()));
        PawnWhenStartList.Add(GeneratePawn(new Vector3(0, 1.3f, 0), StaticPawnDef.GetRandomPawn(), new PawnInfo(), new PawnAttribute()));
        PawnWhenStartList.Add(GeneratePawn(new Vector3(5, 1.3f, 0), StaticPawnDef.GetRandomPawn(), new PawnInfo(), new PawnAttribute()));
        foreach (var pawn in PawnWhenStartList)
        {
            pawn.Def.StopUpdate = true;
        }
    }

    public Pawn ReflashSelectPawn(int index)
    {
        // 刷新指定位置的人物
        RemovePawn(PawnWhenStartList[index]);
        PawnWhenStartList[index] = GeneratePawn(new Vector3(5 * (index - 1), 1.3f, 0), StaticPawnDef.GetRandomPawn(), new PawnInfo(), new PawnAttribute());
        PawnWhenStartList[index].Def.StopUpdate = true;
        return PawnWhenStartList[index];
    }

    public void EndSelect()
    {
        for (int i = 0; i < PawnWhenStartList.Count; i++)
        {
            RemovePawn(PawnWhenStartList[i]);
        }
        PawnWhenStartList.Clear();
    }
}

