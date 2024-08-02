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

        public string PawnDescription;

        public string PrefabPath;

        public bool StopUpdate;
        // 选择
        public bool CanSelect = true;
        // 工作
        public bool CanGetJob = true;
        // 战斗
        public bool CanBattle = true;
        // 征兆
        public bool CanDrafted = true;
        // 建造
        public bool CanBuild = true;
        // 种植
        public bool CanPlant = true;
        // 觅食，狩猎，钓鱼
        public bool CanForaging = true;


        public PawnKindDef() { }

        public PawnKindDef(string pawnName, string pawnDescription, string prefabPath)
        {
            PawnName = pawnName;
            PawnDescription = pawnDescription;
            PrefabPath = prefabPath;
            StopUpdate = false;
        }

        public object Clone()
        {
            PawnKindDef clone = (PawnKindDef)MemberwiseClone();
            clone.CanGetJob = true;
            return clone;
        }
    }
}