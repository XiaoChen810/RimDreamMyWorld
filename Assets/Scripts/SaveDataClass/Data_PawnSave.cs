using ChenChen_AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data_PawnSave
{
    public Vector3 Position;

    public PawnKindDef PawnKindDef;

    public PawnInfo PawnInfo;

    public PawnAttribute PawnAttribute;

    public Data_PawnSave(Vector3 position, PawnKindDef pawnKindDef, PawnAttribute pawnAttribute, PawnInfo pawnInfo)
    {
        Position = position;
        PawnKindDef = pawnKindDef ?? throw new ArgumentNullException(nameof(pawnKindDef));
        PawnAttribute = pawnAttribute ?? throw new ArgumentNullException(nameof(pawnAttribute));
        PawnInfo = pawnInfo ?? throw new ArgumentNullException(nameof(pawnInfo));
    }
}
