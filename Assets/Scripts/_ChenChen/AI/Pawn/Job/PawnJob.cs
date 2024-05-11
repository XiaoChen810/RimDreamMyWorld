using UnityEngine;

namespace ChenChen_AI
{
    public abstract class PawnJob : StateBase
    {
        /// <summary>
        /// ���������������
        /// </summary>
        protected Pawn _pawn;

        public PawnJob(Pawn pawn, float maxTick, StateBase next = null) : base(pawn.StateMachine, next)
        {
            this._pawn = pawn;
            this.MaxTick = maxTick;
        }
    }
}