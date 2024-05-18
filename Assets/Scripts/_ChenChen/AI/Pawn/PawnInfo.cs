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

        // 睡眠值
        [SerializeField] private Stats sleepiness = new Stats("睡眠", 100, 100);
        public Stats Sleepiness
        {
            get { return sleepiness; }
            set { sleepiness = value; }
        }

        // 构造函数
        public PawnInfo()
        {
        }

        public PawnInfo(bool isDead, bool isSelect, bool isOnWork, bool isOnBattle, bool isDrafted, Stats hp, Stats sleepiness)
        {
            IsDead = isDead;
            IsSelect = isSelect;
            IsOnWork = isOnWork;
            IsOnBattle = isOnBattle;
            IsDrafted = isDrafted;
            HP = hp;
            Sleepiness = sleepiness;
        }

        // 克隆方法
        public object Clone()
        {
            return new PawnInfo(IsDead, IsSelect, IsOnWork, IsOnBattle, IsDrafted, HP.Clone(), Sleepiness.Clone());
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
                // 限制当前值不小于 0
                if (value < 0) curValue = 0;
                else curValue = value;
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
            CurValue = curValue;
            MaxValue = maxValue;
        }

        // 克隆方法
        public Stats Clone()
        {
            return new Stats(Name, CurValue, MaxValue);
        }
    }
}
