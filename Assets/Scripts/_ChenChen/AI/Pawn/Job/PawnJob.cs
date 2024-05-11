using UnityEngine;

namespace ChenChen_AI
{
    public abstract class PawnJob : StateBase
    {
        /// <summary>
        /// 做这个工作的棋子
        /// </summary>
        protected Pawn _pawn;

        public PawnJob(Pawn pawn, float maxTick, StateBase next = null) : base(pawn.StateMachine, next)
        {
            this._pawn = pawn;
            this.MaxTick = maxTick;
        }
    }
}