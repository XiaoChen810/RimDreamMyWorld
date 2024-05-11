using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    [System.Serializable]
    public class PawnKindDef : ICloneable
    {
        public string PawnName;

        public string PawnFaction;

        public string PawnDescription;

        public string PrefabPath;

        public bool StopUpdate;

        public bool CanSelect;

        public bool CanGetJob;

        public bool CanBattle;

        public bool CanDrafted;

        public PawnKindDef() { }

        public PawnKindDef(string pawnName, string pawnFaction, string pawnDescription, string prefabPath,
            bool canSelect = true, bool canGetJob = true, bool canBattle = true, bool canDrafted = true)
        {
            PawnName = pawnName;
            PawnFaction = pawnFaction;
            PawnDescription = pawnDescription;
            PrefabPath = prefabPath;
            StopUpdate = false;
            CanSelect = canSelect;
            CanGetJob = canGetJob;
            CanBattle = canBattle;
            CanDrafted = canDrafted;
        }

        public object Clone()
        {
            PawnKindDef clone = (PawnKindDef)MemberwiseClone();
            return clone;
        }
    }
}