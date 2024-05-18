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
        // ѡ��
        public bool CanSelect = true;
        // ����
        public bool CanGetJob = true;
        // ս��
        public bool CanBattle = true;
        // ����
        public bool CanDrafted = true;
        // ����
        public bool CanBuild = true;
        // ��ֲ
        public bool CanPlant = true;
        // ��ʳ�����ԣ�����
        public bool CanForaging = true;


        public PawnKindDef() { }

        public PawnKindDef(string pawnName, string pawnFaction, string pawnDescription, string prefabPath)
        {
            PawnName = pawnName;
            PawnFaction = pawnFaction;
            PawnDescription = pawnDescription;
            PrefabPath = prefabPath;
            StopUpdate = false;
        }

        public object Clone()
        {
            PawnKindDef clone = (PawnKindDef)MemberwiseClone();
            return clone;
        }
    }
}