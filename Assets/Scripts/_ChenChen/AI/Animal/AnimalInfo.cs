using System;

namespace ChenChen_AI
{
    [System.Serializable]
    public class AnimalInfo : ICloneable
    {
        // 剩余生命值
        public float Health = 100;

        // 是否被驯服
        public bool IsTrade;

        // 是否标记驯服
        public bool IsFlagTrade;

        // 是否正在被驯服
        public bool IsOnTrade;

        public object Clone()
        {
            object clone = this.MemberwiseClone();
            return clone;
        }
    }
}
