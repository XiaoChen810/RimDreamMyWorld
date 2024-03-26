using UnityEngine;

namespace PawnStates
{
    public class PawnState_Idle : StateBase
    {
        private Pawn pawn;
        float _time;
        float _waitTime = 3;
        /// <summary>
        /// 闲置
        /// </summary>
        public PawnState_Idle(Pawn pawn) : base(pawn.StateMachine)
        {
            this.pawn = pawn;
        }

        public override void OnEnter()
        {
            _time = 0;
            //pawn.JobDontGet();
        }

        public override StateType OnUpdate()
        {
            if (pawn.StateMachine.GetNextState() != null || !pawn.StateMachine.SpaceStateQueue())
            {
                return StateType.Success;
            }

            _time += Time.deltaTime;
            if (_time > _waitTime)
            {
                Vector2 p = pawn.transform.position;
                p += new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
                pawn.StateMachine.SetNextState(new PawnState_Move(pawn, p));
                return StateType.Success;
            }
            return StateType.Doing;
        }

        public override void OnExit()
        {
            //pawn.JobCanGet();
        }
    }
}
