using System;

namespace ChenChen_AI
{
    [System.Serializable]
    public class AnimalInfo : ICloneable
    {
        public float Health = 100;

        public bool IsTrade;

        public bool IsFlagTrade;

        public bool IsOnTrade;

        public object Clone()
        {
            object clone = this.MemberwiseClone();
            return clone;
        }
    }
}
