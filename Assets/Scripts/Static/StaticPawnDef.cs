using ChenChen_AI;
using System;

public static class StaticPawnDef
{
    public static PawnKindDef s_Bald
    {
        get
        {
            return new PawnKindDef(pawnName: "光头",
                                   pawnFaction: "殖民地",
                                   pawnDescription: "普普通通的光头",
                                   prefabPath: "Prefabs/Pawn/Bald");
        }
    }
    public static PawnKindDef s_SinglePonytail
    {
        get
        {
            return new PawnKindDef(pawnName: "单马尾",
                                   pawnFaction: "殖民地",
                                   pawnDescription: "活泼的单马尾女孩",
                                   prefabPath: "Prefabs/Pawn/SinglePonytail");
        }
    }
    public static PawnKindDef s_RedHair 
    {
        get
        {
            return new PawnKindDef(pawnName: "红毛",
                                   pawnFaction: "殖民地",
                                   pawnDescription: "热血的红毛",
                                   prefabPath: "Prefabs/Pawn/RedHair");
        }
    } 
    public static PawnKindDef s_YellowHair
    {
        get
        {
            return new PawnKindDef(pawnName: "黄毛",
                                   pawnFaction: "殖民地",
                                   pawnDescription: "猥琐的黄毛",
                                   prefabPath: "Prefabs/Pawn/YellowHair");
        }
    } 
    public static PawnKindDef s_CrewCut
    {
        get
        {
            return new PawnKindDef(pawnName: "平头男",
                                   pawnFaction: "殖民地",
                                   pawnDescription: "单纯的平头男",
                                   prefabPath: "Prefabs/Pawn/CrewCut");
        }
    }

    public static PawnKindDef s_Boy
    {
        get
        {
            return new PawnKindDef(pawnName: "小男孩",
                                   pawnFaction: "殖民地",
                                   pawnDescription: "可爱的小男孩",
                                   prefabPath: "Prefabs/Pawn/Boy");
        }
    }

    /// <summary>
    /// 返回一个随机的PawnKindDef对象
    /// </summary>
    /// <returns></returns>
    public static PawnKindDef GetRandomPawn()
    {
        // 创建一个随机数生成器
        Random rand = new Random();

        // 从属性数组中随机选择一个索引
        int randomIndex = rand.Next(0, 6); // 假设有6个属性，需要根据实际情况修改

        // 根据随机索引返回对应的PawnKindDef对象
        switch (randomIndex)
        {
            case 0:
                return s_Bald;
            case 1:
                return s_SinglePonytail;
            case 2:
                return s_RedHair;
            case 3:
                return s_YellowHair;
            case 4:
                return s_CrewCut;
            case 5:
                return s_Boy;
            default:
                // 如果发生意外，返回一个默认的PawnKindDef对象
                return s_Bald;
        }
    }
}
