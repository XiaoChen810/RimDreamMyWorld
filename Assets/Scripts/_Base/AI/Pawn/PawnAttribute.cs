using System;
using UnityEngine;

namespace ChenChen_AI
{

    /// <summary>
    /// �����������
    /// </summary>
    [System.Serializable]
    public class PawnAttribute : ICloneable
    {
        public PawnAttribute()
        {

        }

        public PawnAttribute(PawnAbility a_Combat, PawnAbility a_Culinary, PawnAbility a_Construction, PawnAbility a_Survival, PawnAbility a_Craftsmanship, PawnAbility a_Medical, PawnAbility a_Carrying, PawnAbility a_Research)
        {
            A_Combat = a_Combat;
            A_Culinary = a_Culinary;
            A_Construction = a_Construction;
            A_Survival = a_Survival;
            A_Craftsmanship = a_Craftsmanship;
            A_Medical = a_Medical;
            A_Carrying = a_Carrying;
            A_Research = a_Research;
        }

        /// <summary>
        /// ս������
        /// </summary>
        public PawnAbility A_Combat;
        /// <summary>
        /// �������
        /// </summary>
        public PawnAbility A_Culinary;
        /// <summary>
        /// ��������
        /// </summary>
        public PawnAbility A_Construction;
        /// <summary>
        /// ��������
        /// </summary>
        public PawnAbility A_Survival;
        /// <summary>
        /// ��������
        /// </summary>
        public PawnAbility A_Craftsmanship;
        /// <summary>
        /// ҽ������
        /// </summary>
        public PawnAbility A_Medical;
        /// <summary>
        /// ��������
        /// </summary>
        public PawnAbility A_Carrying;
        /// <summary>
        /// ��������
        /// </summary>
        public PawnAbility A_Research;

        /// <summary>
        /// ��ʼ�������������������
        /// </summary>
        public PawnAttribute InitPawnAttribute()
        {
            A_Combat = new PawnAbility(UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(0, 20), UnityEngine.Random.Range(5, 10));
            A_Culinary = new PawnAbility(UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(0, 20), UnityEngine.Random.Range(5, 10));
            A_Construction = new PawnAbility(UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(0, 20), UnityEngine.Random.Range(5, 10));
            A_Survival = new PawnAbility(UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(0, 20), UnityEngine.Random.Range(5, 10));
            A_Craftsmanship = new PawnAbility(UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(0, 20), UnityEngine.Random.Range(5, 10));
            A_Medical = new PawnAbility(UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(0, 20), UnityEngine.Random.Range(5, 10));
            A_Carrying = new PawnAbility(UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(0, 20), UnityEngine.Random.Range(5, 10));
            A_Research = new PawnAbility(UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(0, 20), UnityEngine.Random.Range(5, 10));
            return this;
        }

        /// <summary>
        /// ��������ֵ�ɳ�
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="ablility"> Ҫ�ı������ </param>
        public void Study(float baseValue, PawnAbility ablility)
        {
            ablility.EXP += baseValue * (0.5f + ablility.Interest);
        }

        public object Clone()
        {
            PawnAttribute clone = (PawnAttribute)MemberwiseClone();
            clone.A_Combat = (PawnAbility)A_Combat.Clone();
            clone.A_Culinary = (PawnAbility)A_Culinary.Clone();
            clone.A_Construction = (PawnAbility)A_Construction.Clone();
            clone.A_Survival = (PawnAbility)A_Survival.Clone();
            clone.A_Craftsmanship = (PawnAbility)A_Craftsmanship.Clone();
            clone.A_Medical = (PawnAbility)A_Medical.Clone();
            clone.A_Carrying = (PawnAbility)A_Carrying.Clone();
            clone.A_Research = (PawnAbility)A_Research.Clone();
            return clone;
        }
    }
}