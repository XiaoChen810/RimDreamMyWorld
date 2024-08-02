using System;
using UnityEngine;

namespace ChenChen_AI
{
    [System.Serializable]
    public class PawnInfo : ICloneable
    {
        #region - State - 
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
        [SerializeField] private bool isInWork;
        public bool IsInWork
        {
            get { return isInWork; }
            set { isInWork = value; }
        }

        // 是否在战斗
        [SerializeField] private bool isInBattle;
        public bool IsInBattle
        {
            get { return isInBattle; }
            set { isInBattle = value; }
        }

        // 是否被征召
        [SerializeField] private bool isDrafted;
        public bool IsDrafted
        {
            get { return isDrafted; }
            set { isDrafted = value; }
        }

        #endregion

        #region - Stats -
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
        #endregion

        #region - Other -

        public string faction;

        #endregion
        public PawnInfo()
        {
            faction = GameManager.PLAYER_FACTION;
        }

        public PawnInfo(string faction)
        {
            Debug.Log("初始化指定派系的Pawn信息: " + faction);
            this.faction = faction;
        }

        // 克隆方法
        public object Clone()
        {
            PawnInfo clone = (PawnInfo)MemberwiseClone();
            clone.IsSelect = false;
            clone.IsInWork = false;
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
        [SerializeField] private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [SerializeField] private float curValue;
        public float CurValue
        {
            get { return curValue; }
            set
            {
                curValue = Mathf.Clamp(value, 0, maxValue);
            }
        }

        [SerializeField] private float maxValue;
        public float MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }

        public bool IsMax => CurValue >= MaxValue;

        public bool IsSpace => CurValue <= 0;

        public float Percentage => CurValue / MaxValue;

        public Stats(string name, float curValue, float maxValue)
        {
            Name = name;
            MaxValue = maxValue;
            CurValue = curValue;
        }

        public Stats Clone()
        {
            return new Stats(Name, CurValue, MaxValue);
        }
    }
}
