using System;
using UnityEngine;

namespace ChenChen_AI
{
    [System.Serializable]
    public class PawnInfo : ICloneable
    {
        public PawnInfo()
        {

        }

        public bool IsDead;
        public bool IsSelect;
        public bool IsOnWork;
        public bool IsOnBattle;
        public bool IsDrafted;

        public int HP;

        public object Clone()
        {
            PawnInfo clone = (PawnInfo)MemberwiseClone();
            return clone;
        }
    }
}
