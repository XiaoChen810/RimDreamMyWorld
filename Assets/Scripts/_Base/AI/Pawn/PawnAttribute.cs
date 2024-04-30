using UnityEngine;

namespace ChenChen_AI
{

    /// <summary>
    /// �����������
    /// </summary>
    [System.Serializable]
    public class PawnAttribute
    {
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
        public void InitPawnAttribute()
        {
            A_Combat = new PawnAbility(Random.Range(0, 3), Random.Range(0, 20), Random.Range(5, 10));
            A_Culinary = new PawnAbility(Random.Range(0, 3), Random.Range(0, 20), Random.Range(5, 10));
            A_Construction = new PawnAbility(Random.Range(0, 3), Random.Range(0, 20), Random.Range(5, 10));
            A_Survival = new PawnAbility(Random.Range(0, 3), Random.Range(0, 20), Random.Range(5, 10));
            A_Craftsmanship = new PawnAbility(Random.Range(0, 3), Random.Range(0, 20), Random.Range(5, 10));
            A_Medical = new PawnAbility(Random.Range(0, 3), Random.Range(0, 20), Random.Range(5, 10));
            A_Carrying = new PawnAbility(Random.Range(0, 3), Random.Range(0, 20), Random.Range(5, 10));
            A_Research = new PawnAbility(Random.Range(0, 3), Random.Range(0, 20), Random.Range(5, 10));
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
    }
}