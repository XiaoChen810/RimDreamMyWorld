using ChenChen_AI;
using ChenChen_Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PawnGeneratorTool : MonoBehaviour
{
    #region - PawnList -
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

    #endregion

    private GameObject pawnPrefab = null;

    /// <summary>
    /// 生成Pawn，并添加到PawnsList
    /// </summary>
    public Pawn GeneratePawn(Vector3 position, PawnKindDef kindDef, PawnInfo info, PawnAttribute attribute)
    {
        if (!TryInitGameObject(kindDef, out Pawn newPawn))
        {
            Debug.LogWarning("初始化 Pawn 失败");
            return null;
        }

        newPawn.Def = (PawnKindDef)kindDef.Clone();
        newPawn.Info = (PawnInfo)info.Clone();
        newPawn.Attribute = (PawnAttribute)attribute.Clone();
        _pawnsList.Add(newPawn);
        return newPawn;

        bool TryInitGameObject(PawnKindDef kindDef, out Pawn result)
        {
            if (pawnPrefab == null)
            {
                pawnPrefab = Resources.Load<GameObject>("Prefabs/Pawn/PawnDefault");
            }

            result = Instantiate(pawnPrefab, position, Quaternion.identity).GetComponent<Pawn>();
            result.name = kindDef.PawnName;
            result.transform.parent = transform;

            XmlLoader xmlLoader = XmlLoader.Instance;
            result.SetHead(xmlLoader.GetRandom<HeadDef>(XmlLoader.Def_PawnHead));
            result.SetHair(xmlLoader.GetRandom<HairDef>(XmlLoader.Def_PawnHair), true);
            result.SetBody(xmlLoader.GetRandom<BodyDef>(XmlLoader.Def_PawnBody), true);                  
            result.SetDressed(xmlLoader.GetRandom<ApparelDef>(XmlLoader.Def_Apparel));
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
        return GeneratePawn(position, new PawnKindDef(StaticPawnDef.GetRandomPawnName()), new PawnInfo(), new PawnAttribute());
    }

    /// <summary>
    /// 生成随机Pawn并指定派系
    /// </summary>
    /// <param name="position"></param>
    /// <param name="faction"></param>
    /// <returns></returns>
    public Pawn GeneratePawn(Vector3 position, string faction)
    {
        return GeneratePawn(position, new PawnKindDef(StaticPawnDef.GetRandomPawnName()), new PawnInfo(faction), new PawnAttribute());
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

    private readonly float HIGH = 2f;

    public void StartSelect()
    {
        PawnWhenStartList.Add(GeneratePawn(new Vector3(-5, HIGH, 0)));
        PawnWhenStartList.Add(GeneratePawn(new Vector3(0, HIGH, 0)));
        PawnWhenStartList.Add(GeneratePawn(new Vector3(5, HIGH, 0)));
        foreach (var pawn in PawnWhenStartList)
        {
            pawn.Def.StopUpdate = true;
        }
    }

    public Pawn ReflashSelectPawn(int index)
    {
        RemovePawn(PawnWhenStartList[index], 0);
        PawnWhenStartList[index] = GeneratePawn(new Vector3(5 * (index - 1), HIGH, 0));
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

