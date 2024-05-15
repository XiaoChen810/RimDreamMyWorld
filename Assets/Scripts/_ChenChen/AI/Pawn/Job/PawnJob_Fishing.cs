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
            this._pawn = pawn;
            this.target = target;
        }

        public override bool OnEnter()
        {
            if (target == null) return false;

            curTargetComponent = target.GetComponent<Thing_Building>();
            if (curTargetComponent == null)
            {
                DebugLogDescription = ("尝试获取组件失败");
                return false;
            }

            // 尝试取得权限，预定当前工作，标记目标被使用
            if (!curTargetComponent.GetPermission(_pawn))
            {
                DebugLogDescription = ("目标已经其他人被使用");
                return false;
            }

            // 设置目标点
            bool flag = _pawn.MoveController.GoToHere(target.transform.position, Urgency.Normal);
            if (!flag)
            {
                DebugLogDescription = ("无法移动到目标点");
                return false;
            }
            // 设置人物状态
            _pawn.JobToDo(target);

            return true;
        }   

        public override StateType OnUpdate()
        {
            // 到达后
            if (_pawn.MoveController.ReachDestination)
            {
                _pawn.JobDoing();
                _time += Time.deltaTime;
                _pawn.MoveController.FilpRight();
            }

            if (_time > 0 && _time <= animTime1) _pawn.Animator.SetInteger("IsFishing", 1);
            if (_time > animTime1 && _time <= animTime2) _pawn.Animator.SetInteger("IsFishing", 2);
            if (_time > animTime2 && _time <= animTime3) _pawn.Animator.SetInteger("IsFishing", 3);

            if (_time > animTime3 && _time <= animTime4)
            {
                if (_pawn.Animator.GetInteger("IsFishing") != 4)
                {
                    Debug.Log("钓上一条鱼");
                }
                _pawn.Animator.SetInteger("IsFishing", 4);
            }

            if (_time > animTime4)
            {
                _pawn.Animator.SetInteger("IsFishing", 0);
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            _pawn.JobDone();

            curTargetComponent.RevokePermission(_pawn);
        }
    }
}


