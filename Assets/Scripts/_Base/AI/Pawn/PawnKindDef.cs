using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    [System.Serializable]
    public class PawnKindDef
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

        public PawnKindDef(Pawn pawn)
        {
            PawnName = pawn.Def.PawnName;
            PawnFaction = pawn.Def.PawnFaction;
            PawnDescription = pawn.Def.PawnDescription;
            PrefabPath = pawn.Def.PrefabPath;
            StopUpdate = false;
            CanSelect = pawn.Def.CanSelect;
            CanGetJob = pawn.Def.CanGetJob;
            CanBattle = pawn.Def.CanBattle;
            CanDrafted = pawn.Def.CanDrafted;
        }

        public PawnKindDef(PawnKindDef copyDef)
        {
            PawnName = copyDef.PawnName;
            PawnFaction = copyDef.PawnFaction;
            PawnDescription = copyDef.PawnDescription;
            PrefabPath = copyDef.PrefabPath;
            StopUpdate = copyDef.StopUpdate;
            CanSelect = copyDef.CanSelect;
            CanGetJob = copyDef.CanGetJob;
            CanBattle = copyDef.CanBattle;
            CanDrafted = copyDef.CanDrafted;
        }
    }
}