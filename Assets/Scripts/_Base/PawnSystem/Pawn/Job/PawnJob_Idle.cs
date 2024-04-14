using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Idle : PawnJob
    {
        float _time;
        float _waitTime = 3;
        /// <summary>
        /// 闲置
        /// </summary>
        public PawnJob_Idle(Pawn pawn) : base(pawn)
        {
            this.pawn = pawn;
        }

        public override bool OnEnter()
        {
            _time = 0;
            return true;
        }

        public override StateType OnUpdate()
        {
            if (pawn.StateMachine.NextState != null || !(pawn.StateMachine.StateQueue.Count == 0))
            {
                return StateType.Success;
            }

            _time += Time.deltaTime;
            if (_time > _waitTime)
            {
                Vector2 p = pawn.transform.position;
                p += new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
                pawn.StateMachine.NextState = new PawnJob_Move(pawn, p);
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
