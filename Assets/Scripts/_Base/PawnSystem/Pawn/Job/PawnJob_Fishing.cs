using ChenChen_BuildingSystem;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Fishing : PawnJob
    {
        private readonly static float tick = 50;
        private GameObject target;
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
        public PawnJob_Fishing(Pawn pawn, GameObject target = null) : base(pawn, tick)
        {
            this.pawn = pawn;
            this.target = target;
        }

        public override bool OnEnter()
        {
            if (target == null) return false;

            curTargetComponent = target.GetComponent<Thing_Building>();
            if (curTargetComponent == null)
            {
                Debug.LogWarning("The FishingPoint Don't Have Building Component");
                return false;
            }

            // 尝试取得权限，预定当前工作，标记目标被使用
            if (!curTargetComponent.GetPrivilege(pawn))
            {
                //未取得权限则退出
                return false;
            }

            // 设置目标点
            pawn.MoveControl.GoToHere(target.transform.position, Urgency.Normal);

            // 设置人物状态
            pawn.JobToDo(target);

            return true;
        }   

        public override StateType OnUpdate()
        {
            // 到达后
            if (pawn.MoveControl.ReachDestination)
            {
                pawn.JobDoing();
                _time += Time.deltaTime;
                pawn.MoveControl.FilpRight();
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
            pawn.JobDone();

            curTargetComponent.RevokePrivilege(pawn);
        }
    }
}


