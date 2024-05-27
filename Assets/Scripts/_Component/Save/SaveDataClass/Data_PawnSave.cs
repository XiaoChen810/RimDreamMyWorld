using ChenChen_AI;
using System;
using UnityEngine;

[System.Serializable]
public struct Data_PawnSave : ICloneable
{
    public Vector2 Position;

    public PawnKindDef PawnKindDef;

    public PawnInfo PawnInfo;

    public PawnAttribute PawnAttribute;

    public Data_PawnSave(Vector2 position, PawnKindDef pawnKindDef, PawnAttribute pawnAttribute, PawnInfo pawnInfo)
    {
        Position = position;
        PawnKindDef = pawnKindDef ?? throw new ArgumentNullException(nameof(pawnKindDef));
        PawnAttribute = pawnAttribute ?? throw new ArgumentNullException(nameof(pawnAttribute));
        PawnInfo = pawnInfo ?? throw new ArgumentNullException(nameof(pawnInfo));
    }

    public object Clone()
    {
        // deep copy
        Data_PawnSave clone = (Data_PawnSave)this.MemberwiseClone();
        clone.Position = Position;
        clone.PawnKindDef = (PawnKindDef)PawnKindDef.Clone();
        clone.PawnInfo = (PawnInfo)PawnInfo.Clone();
        clone.PawnAttribute = (PawnAttribute)PawnAttribute.Clone();
        return clone;
    }
}
