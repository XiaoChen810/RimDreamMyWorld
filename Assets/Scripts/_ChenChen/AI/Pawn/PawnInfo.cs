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
        [Header("人物需求")]
        [SerializeReference] public PawnNeed Need;

        public int HP;

        public object Clone()
        {
            PawnInfo clone = (PawnInfo)MemberwiseClone();
            //clone.Need = (clone.Need != null) ? (PawnNeed)Need.Clone() : null;
            clone.Need = (PawnNeed)Need.Clone();
            return clone;
        }
    }
}
