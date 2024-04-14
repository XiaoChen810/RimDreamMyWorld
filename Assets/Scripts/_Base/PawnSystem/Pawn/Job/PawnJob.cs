using UnityEngine;

namespace ChenChen_AI
{
    public abstract class PawnJob : StateBase
    {
        protected Pawn pawn;

        public PawnJob(Pawn pawn, StateBase next = null) : base(pawn.StateMachine, next)
        {
            this.pawn = pawn;
        }
    }
}