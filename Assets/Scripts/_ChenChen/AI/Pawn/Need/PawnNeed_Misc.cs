using UnityEngine;

namespace ChenChen_AI
{
    public class PawnNeed_Misc : PawnNeed
    {
        private static readonly float probaility = 1;

        public PawnNeed_Misc(string description) : base(probaility, description)
        {
        }
    }
}
