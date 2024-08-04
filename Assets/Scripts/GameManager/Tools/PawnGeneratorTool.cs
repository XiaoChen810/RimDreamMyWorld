using ChenChen_AI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PawnGeneratorTool : MonoBehaviour
{
    [SerializeField] private List<Pawn> _pawnsList = new List<Pawn>();
    /// <summary>
    /// 游戏内全部的Pawn列表
    /// </summary>
    public IReadOnlyList<Pawn> PawnList_All
    {
        get
        {
            return _pawnsList.AsReadOnly();
        }
    }

    public IReadOnlyList<Pawn> PawnList_Live
    {
        get
        {
            return _pawnsList.Where(x => !x.Info.IsDead).ToList();
        }
    }

    public IReadOnlyList<Pawn> PawnList_Died
    {
        get
        {
            return _pawnsList.Where(x => x.Info.IsDead).ToList();
        }
    }

    public IReadOnlyList<Pawn> PawnList_Colony
    {
        get
        {
            return _pawnsList.Where(x => x.Faction == GameManager.PLAYER_FACTION).ToList();
        }
    }

    // ! ! ! 仅当进行人物选择时使用的角色列表 ! ! !
    private List<Pawn> _pawnWhenStartList = new List<Pawn>();
    public List<Pawn> PawnWhenStartList
    {
        get { return _pawnWhenStartList; }
    }

    /// <summary>
    /// 生成Pawn，并添加到PawnsList
    /// </summary>
    public Pawn GeneratePawn(Vector3 position, PawnKindDef kindDef, PawnInfo info, PawnAttribute attribute)
    {
        if (!TryInitGameObject(kindDef, out Pawn newPawn))
        {
            Debug.LogWarning("初始化游戏物体失败");
            return null;
        }

        if (newPawn.TryGetComponent<Pawn>(out Pawn pawn))
        {
            pawn.Def = (PawnKindDef)kindDef.Clone();
            pawn.Info = (PawnInfo)info.Clone();
            pawn.Attribute = (PawnAttribute)attribute.Clone();
            _pawnsList.Add(newPawn);
            return pawn;
        }
        else
        {
            Debug.LogError("获取Pawn组件失败");
            return null;
        }

        bool TryInitGameObject(PawnKindDef kindDef, out Pawn result)
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
            result = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity).GetComponent<Pawn>();    
            result.name = kindDef.PawnName;
            result.transform.parent = transform;
            return true;
        }
    }

    /// <summary>
    /// 生成随机Pawn
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Pawn GeneratePawn(Vector3 position)
    {
        return GeneratePawn(position, StaticPawnDef.GetRandomPawn(), new PawnInfo(), new PawnAttribute());
    }

    /// <summary>
    /// 生成随机Pawn并指定派系
    /// </summary>
    /// <param name="position"></param>
    /// <param name="faction"></param>
    /// <returns></returns>
    public Pawn GeneratePawn(Vector3 position, string faction)
    {
        return GeneratePawn(position, StaticPawnDef.GetRandomPawn(), new PawnInfo(faction), new PawnAttribute());
    }

    public void LoadScenePawnFromSave(Data_GameSave data_GameSave)
    {
        foreach (var pawnSave in data_GameSave.SavePawns)
        {
            Data_PawnSave newPawnSave = (Data_PawnSave) pawnSave.Clone();
            _ = GeneratePawn(newPawnSave.Position, newPawnSave.PawnKindDef, newPawnSave.PawnInfo, newPawnSave.PawnAttribute);
        }
    }

    /// <summary>
    /// 彻底移除Pawn，注意使用时机，只是死亡的话不要使用
    /// </summary>
    /// <param name="pawn"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public bool RemovePawn(Pawn pawn, float time)
    {
        for (int i = 0; i < PawnList_All.Count; i++)
        {
            if (PawnList_All[i] == pawn)
            {            
                _pawnsList.RemoveAt(i);
                Destroy(pawn.gameObject, time);
                return true;
            }
        }
        return false;
    }

    public void StartSelect()
    {
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
        RemovePawn(PawnWhenStartList[index], 0);
        PawnWhenStartList[index] = GeneratePawn(new Vector3(5 * (index - 1), 1.3f, 0), StaticPawnDef.GetRandomPawn(), new PawnInfo(), new PawnAttribute());
        PawnWhenStartList[index].Def.StopUpdate = true;
        return PawnWhenStartList[index];
    }

    public void EndSelect()
    {
        for (int i = 0; i < PawnWhenStartList.Count; i++)
        {
            RemovePawn(PawnWhenStartList[i], 0);
        }
        PawnWhenStartList.Clear();
    }
}

