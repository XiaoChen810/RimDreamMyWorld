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

        public bool CanSelect;

        public bool CanGetJob;

        public bool CanBattle;

        public bool CanDrafted;

        public PawnKindDef(string pawnName, string pawnFaction, string pawnDescription, string prefabPath,
            bool canSelect = true, bool canGetJob = true, bool canBattle = true, bool canDrafted = true)
        {
            PawnName = pawnName;
            PawnFaction = pawnFaction;
            PawnDescription = pawnDescription;
            PrefabPath = prefabPath;
            CanSelect = canSelect;
            CanGetJob = canGetJob;
            CanBattle = canBattle;
            CanDrafted = canDrafted;
        }

        public PawnKindDef(Pawn pawn)
        {
            PawnName = pawn.PawnName;
            PawnFaction = pawn.FactionName;
            PawnDescription = pawn.Description;
            PrefabPath = pawn.PrefabPath;
            CanSelect = pawn.CanSelect;
            CanGetJob = pawn.CanGetJob;
            CanBattle = pawn.CanBattle;
            CanDrafted = pawn.CanDrafted;
        }
    }
}