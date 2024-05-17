using System;
using UnityEngine;

namespace ChenChen_AI
{

    /// <summary>
    /// 人物的属性类
    /// </summary>
    [System.Serializable]
    public class PawnAttribute : ICloneable
    {
        public PawnAttribute()
        {
            System.Random random = new System.Random();
            A_Combat = new PawnAbility(random.Next(0, 3), random.Next(0, 20), random.Next(0, 20));
            A_Culinary = new PawnAbility(random.Next(0, 3), random.Next(0, 20), random.Next(0, 20));
            A_Construction = new PawnAbility(random.Next(0, 3), random.Next(0, 20), random.Next(0, 20));
            A_Survival = new PawnAbility(random.Next(0, 3), random.Next(0, 20), random.Next(0, 20));
            A_Craftsmanship = new PawnAbility(random.Next(0, 3), random.Next(0, 20), random.Next(0, 20));
            A_Medical = new PawnAbility(random.Next(0, 3), random.Next(0, 20), random.Next(0, 20));
            A_Carrying = new PawnAbility(random.Next(0, 3), random.Next(0, 20), random.Next(0, 20));
            A_Research = new PawnAbility(random.Next(0, 3), random.Next(0, 20), random.Next(0, 20));
        }

        /// <summary>
        /// 战斗能力
        /// </summary>
        public PawnAbility A_Combat;
        /// <summary>
        /// 烹饪能力
        /// </summary>
        public PawnAbility A_Culinary;
        /// <summary>
        /// 建造能力
        /// </summary>
        public PawnAbility A_Construction;
        /// <summary>
        /// 生存能力
        /// </summary>
        public PawnAbility A_Survival;
        /// <summary>
        /// 工艺能力
        /// </summary>
        public PawnAbility A_Craftsmanship;
        /// <summary>
        /// 医护能力
        /// </summary>
        public PawnAbility A_Medical;
        /// <summary>
        /// 搬运能力
        /// </summary>
        public PawnAbility A_Carrying;
        /// <summary>
        /// 科研能力
        /// </summary>
        public PawnAbility A_Research;

        /// <summary>
        /// 人物能力值成长
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="ablility"> 要改变的能力 </param>
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