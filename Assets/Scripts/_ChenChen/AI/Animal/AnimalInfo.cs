using System;

namespace ChenChen_AI
{
    [System.Serializable]
    public class AnimalInfo : ICloneable
    {
        // 剩余生命值
        public float Health;
        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
