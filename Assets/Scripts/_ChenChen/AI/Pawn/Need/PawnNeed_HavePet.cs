using UnityEngine;

namespace ChenChen_AI
{
    public class PawnNeed_HavePet : PawnNeed
    {
        private static readonly string description = "想要一个宠物";
        private static readonly float probaility = 10;

        public PawnNeed_HavePet() : base(probaility, description)
        {
        }

    }
}
