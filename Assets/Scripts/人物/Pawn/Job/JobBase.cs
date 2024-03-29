using UnityEngine;

namespace ChenChen_AI
{
    public abstract class JobBase : StateBase
    {
        protected Pawn pawn;

        public JobBase(Pawn pawn, StateBase next = null) : base(pawn.StateMachine, next)
        {
            this.pawn = pawn;
        }
    }
}