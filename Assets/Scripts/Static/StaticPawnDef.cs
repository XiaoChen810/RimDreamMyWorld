using ChenChen_AI;
using System;

public static class StaticPawnDef
{
    public static PawnKindDef s_Bald = new PawnKindDef(
        pawnName: "光头",
        pawnFaction: "殖民地",
        pawnDescription: "普普通通的光头",
        prefabPath: "Prefabs/Pawn/Bald");
    public static PawnKindDef s_SinglePonytail = new PawnKindDef(
        pawnName: "单马尾",
        pawnFaction: "殖民地",
        pawnDescription: "活泼的单马尾女孩",
        prefabPath: "Prefabs/Pawn/SinglePonytail");
    public static PawnKindDef s_RedHair = new PawnKindDef(
        pawnName: "红毛",
        pawnFaction: "殖民地",
        pawnDescription: "热血的红毛",
        prefabPath: "Prefabs/Pawn/RedHair");
    public static PawnKindDef s_YellowHair = new PawnKindDef(
        pawnName: "黄毛",
        pawnFaction: "殖民地",
        pawnDescription: "猥琐的黄毛",
        prefabPath: "Prefabs/Pawn/YellowHair");
    public static PawnKindDef s_CrewCut = new PawnKindDef(
        pawnName: "平头男",
        pawnFaction: "殖民地",
        pawnDescription: "单纯的平头男",
        prefabPath: "Prefabs/Pawn/CrewCut");
    public static PawnKindDef s_Boy = new PawnKindDef(
        pawnName: "小男孩",
        pawnFaction: "殖民地",
        pawnDescription: "可爱的小男孩",
        prefabPath: "Prefabs/Pawn/Boy");
}
