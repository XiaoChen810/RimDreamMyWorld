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


        public PawnKindDef()
        {          
        }

        public PawnKindDef(string pawnName)
        {
            PawnName = pawnName;
        }

        public PawnKindDef(string pawnName, string pawnDescription)
        {
            PawnName = pawnName;
            PawnDescription = pawnDescription;
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