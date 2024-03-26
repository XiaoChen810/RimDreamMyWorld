using ChenChen_BuildingSystem;
using UnityEngine;

namespace PawnStates
{
    public class PawnState_Fishing : StateBase
    {
        private Pawn pawn;
        private GameObject fishing;
        private Building currentWorkObject;

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
        public PawnState_Fishing(Pawn pawn, GameObject fishing = null) : base(pawn.StateMachine, null)
        {
            this.pawn = pawn;
            this.fishing = fishing;
        }

        public override void OnEnter()
        {
            if (fishing != null)
            {
                currentWorkObject = fishing.GetComponent<Building>();
                if (currentWorkObject != null)
                {
                    // 尝试取得权限，预定当前工作，标记目标被使用
                    if (!currentWorkObject.GetPrivilege(pawn))
                    {
                        //未取得权限则退出
                        IsError = true;
                        return;
                    }

                    // 设置目标点
                    pawn.MoveControl.GoToHere(fishing.transform.position);

                    // 设置人物状态
                    pawn.JobToDo(fishing);
                }
                else
                {
                    Debug.LogWarning("The FishingPoint Don't Have Building Component");
                }
            }
        }

        public override StateType OnUpdate()
        {
            if (IsError) return StateType.Failed;

            // 到达后
            if (pawn.MoveControl.IsReach || Vector2.Distance(pawn.transform.position, fishing.transform.position) < 0.01)
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

            currentWorkObject.RevokePrivilege(pawn);
        }
    }
}


