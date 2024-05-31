using System;
using UnityEngine;

namespace ChenChen_AI
{
    [System.Serializable]
    public class PawnInfo : ICloneable
    {
        // 生存状态
        [SerializeField] private bool isDead;
        public bool IsDead
        {
            get { return isDead; }
            set { isDead = value; }
        }

        // 是否被选择
        [SerializeField] private bool isSelect;
        public bool IsSelect
        {
            get { return isSelect; }
            set { isSelect = value; }
        }

        // 是否在工作
        [SerializeField] private bool isOnWork;
        public bool IsOnWork
        {
            get { return isOnWork; }
            set { isOnWork = value; }
        }

        // 是否在战斗
        [SerializeField] private bool isOnBattle;
        public bool IsOnBattle
        {
            get { return isOnBattle; }
            set { isOnBattle = value; }
        }

        // 是否被征召
        [SerializeField] private bool isDrafted;
        public bool IsDrafted
        {
            get { return isDrafted; }
            set { isDrafted = value; }
        }

        // 健康值
        [SerializeField] private Stats hp = new Stats("健康值", 100, 100);
        public Stats HP
        {
            get { return hp; }
            set { hp = value; }
        }

        // 困意值
        [SerializeField] private Stats sleepiness = new Stats("睡眠", 100, 100);
        public Stats Sleepiness
        {
            get { return sleepiness; }
            set { sleepiness = value; }
        }

        // 心情值
        [SerializeField] private Stats happiness = new Stats("心情", 100, 100);
        public Stats Happiness
        {
            get { return happiness; }
            set { happiness = value; }
        }

        // 构造函数
        public PawnInfo()
        {
        }

        public PawnInfo(bool isDead, bool isSelect, bool isOnWork, bool isOnBattle, bool isDrafted, Stats hp, Stats sleepiness, Stats happiness)
        {
            IsDead = isDead;
            IsSelect = isSelect;
            IsOnWork = isOnWork;
            IsOnBattle = isOnBattle;
            IsDrafted = isDrafted;
            HP = hp;
            Sleepiness = sleepiness;
            Happiness = happiness;
        }

        // 克隆方法
        public object Clone()
        {
            PawnInfo clone = (PawnInfo)MemberwiseClone();
            clone.IsSelect = false;
            clone.IsOnWork = false;
            clone.IsDrafted = false;
            clone.HP = HP.Clone();
            clone.Sleepiness = Sleepiness.Clone();
            clone.Happiness = Happiness.Clone();
            return clone;
        }
    }

    [System.Serializable]
    public class Stats
    {
        // 属性名称
        [SerializeField] private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        // 当前值
        [SerializeField] private float curValue;
        public float CurValue
        {
            get { return curValue; }
            set
            {
                // 限制当前值不小于 0 且不大于 maxValue
                curValue = Mathf.Clamp(value, 0, maxValue);
            }
        }

        // 最大值
        [SerializeField] private float maxValue;
        public float MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }

        // 是否达到最大值
        public bool IsMax => CurValue >= MaxValue;

        // 是否达到最小值
        public bool IsSpace => CurValue <= 0;

        // 构造函数
        public Stats(string name, float curValue, float maxValue)
        {
            Name = name;
            MaxValue = maxValue;
            CurValue = curValue;
        }

        // 克隆方法
        public Stats Clone()
        {
            return new Stats(Name, CurValue, MaxValue);
        }
    }
}
