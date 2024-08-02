using ChenChen_UI;
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
        public PawnJob_Idle(Pawn pawn) : base(pawn, tick, null)
        {
            this.pawn = pawn;
            this.Description = "发呆";
        }

        public override bool OnEnter()
        {
            _time = 0;
            return true;
        }

        public override StateType OnUpdate()
        {
            if (pawn.StateMachine.NextState != null || pawn.StateMachine.StateQueue.Count != 0)
            {              
                return StateType.Success;
            }

            _time += Time.deltaTime;
            if (_time > _waitTime)
            {
                pawn.EmotionController.AddEmotion(EmotionType.confused);

                Vector2 p = pawn.transform.position;
                p += new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));

                pawn.StateMachine.NextState = new PawnJob_Move(pawn, p);
                return StateType.Failed;
            }
            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

            if(IsSuccess)
            {
                pawn.EmotionController.RemoveEmotion(EmotionType.confused);
            }
        }

        public override void OnInterrupt()
        {
            pawn.EmotionController.RemoveEmotion(EmotionType.confused);
        }
    }
}
