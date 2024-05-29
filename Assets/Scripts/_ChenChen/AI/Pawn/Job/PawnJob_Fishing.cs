using ChenChen_Thing;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Fishing : PawnJob
    {
        private readonly static float tick = 50;
        private Thing_Building curTargetComponent;

        private float _time;
        private float animTime1 = 1.135f;
        private float animTime2 = 6.135f;
        private float animTime3 = 10f;
        private float animTime4 = 15f;

        /// <summary>
        /// 开始钓鱼，需要设置钓鱼位置
        /// </summary>
        /// <param name="characterMain"></param>
        /// <param name="fishPos"></param>
        public PawnJob_Fishing(Pawn pawn, GameObject target) : base(pawn, tick, new TargetPtr(target))
        {
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            curTargetComponent = target.GetComponent<Thing_Building>();
            if (curTargetComponent == null)
            {
                DebugLogDescription = ("尝试获取组件失败");
                return false;
            }

            // 设置目标点
            bool flag = pawn.MoveController.GoToHere(target.Positon - new Vector3(0, 0.4f), Urgency.Normal);
            if (!flag)
            {
                DebugLogDescription = ("无法移动到目标点");
                return false;
            }
            // 设置人物状态
            pawn.JobToDo(target.GameObject);

            return true;
        }   

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            // 到达后
            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();
                _time += Time.deltaTime;
                pawn.MoveController.FilpRight();
            }

            if (_time > 0 && _time <= animTime1) pawn.Animator.SetInteger("IsFishing", 1);
            if (_time > animTime1 && _time <= animTime2) pawn.Animator.SetInteger("IsFishing", 2);
            if (_time > animTime2 && _time <= animTime3) pawn.Animator.SetInteger("IsFishing", 3);

            if (_time > animTime3 && _time <= animTime4)
            {
                if (pawn.Animator.GetInteger("IsFishing") != 4)
                {
                    Debug.Log("钓上一条鱼");
                }
                pawn.Animator.SetInteger("IsFishing", 4);
            }

            if (_time > animTime4)
            {
                pawn.Animator.SetInteger("IsFishing", 0);
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

            pawn.Animator.SetInteger("IsFishing", 0);
        }
    }
}


