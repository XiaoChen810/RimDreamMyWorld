using UnityEngine;

namespace ChenChen_AI
{

    /// <summary>
    /// 人物的属性类
    /// </summary>
    [System.Serializable]
    public class PawnAttribute
    {
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
        /// 初始化人物能力，随机分配
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
        /// 人物能力值成长
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="ablility"> 要改变的能力 </param>
        public void Study(float baseValue, PawnAbility ablility)
        {
            ablility.EXP += baseValue * (0.5f + ablility.Interest);
        }
    }
}