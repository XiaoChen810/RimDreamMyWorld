using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Idle : PawnJob
    {
        private readonly static float tick = 10;
        float _time;
        float _waitTime = 5;
        /// <summary>
        /// 闲置
        /// </summary>
        public PawnJob_Idle(Pawn pawn) : base(pawn, tick)
        {
            this._pawn = pawn;
        }

        public override bool OnEnter()
        {
            _time = 0;
            return true;
        }

        public override StateType OnUpdate()
        {
            if (_pawn.StateMachine.NextState != null || !(_pawn.StateMachine.StateQueue.Count == 0))
            {
                return StateType.Success;
            }

            _time += Time.deltaTime;
            if (_time > _waitTime)
            {
                Vector2 p = _pawn.transform.position;
                p += new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
                _pawn.StateMachine.NextState = new PawnJob_Move(_pawn, p);
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
