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
                Debug.Log(1);
                pawn.EmotionController.RemoveEmotion(EmotionType.confused);
                return StateType.Success;
            }

            _time += Time.deltaTime;
            if (_time > _waitTime)
            {
                if (pawn.EmotionController.AddEmotion(EmotionType.confused))
                {
                    string content = $"{pawn.name}有点无聊，他现在没事可做";
                    ScenarioManager.Instance.Narrative(content, pawn.gameObject);
                }

                Vector2 p = pawn.transform.position;
                p += new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
                pawn.StateMachine.NextState = new PawnJob_Move(pawn, p);
                return StateType.Success;
            }
            return StateType.Doing;
        }
    }
}
