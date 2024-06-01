using ChenChen_Thing;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Building : PawnJob
    {
        private readonly static float tick = 50;

        private ThingBase targetComponent;
        private float _time;
        private float _timeOne;

        /// <summary>
        /// 创建一个新的建造任务，需要设置建筑坐标
        /// </summary>
        /// <param name="characterMain"></param>
        /// <param name="buildPos"></param>
        public PawnJob_Building(Pawn pawn, GameObject target) : base(pawn, tick, new TargetPtr(target))
        {
            float ability = pawn.Attribute.A_Construction.Value;
            float a = 1 - (ability / 20);
            _timeOne = Mathf.Lerp(0.1f, 1f, a);
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if(baseResult != true) return baseResult;

            // 尝试获取组件
            targetComponent = target.GetComponent<ThingBase>();
            if (targetComponent == null)
            {
                DebugLogDescription = ("尝试获取组件失败");
                return false;
            }

            // 设置人物目标点，前往目标，跑过去
            if (!pawn.MoveController.GoToHere(target.Positon, Urgency.Urge, pawn.WorkRange))
            {
                DebugLogDescription = ("无法移动到目标点");
                return false;
            }

            // 设置人物接取工作
            pawn.JobToDo(target.GameObject);
            this.Description = "前往建造" + target.GameObject.name;

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            // 判断是否到达目标点附近
            if (pawn.MoveController.ReachDestination)
            {
                // 设置人物正在工作
                pawn.JobDoing();
                this.Description = "正在建造" + target.GameObject.name;

                // 执行工作
                _time += Time.deltaTime;
                if(_time > _timeOne)
                {
                    targetComponent.OnBuild(1);
                    _time = 0;
                }

                // 播放动画
                pawn.Animator.SetBool("IsDoing", true);
            }

            // 如果完成了建造，状态机结束暂停，可以进入下一个状态
            if (targetComponent.Workload <= 0)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

            // 结束动画
            pawn.Animator.SetBool("IsDoing", false);
        }

        public override void OnInterrupt()
        {
            targetComponent.OnInterpretBuild();
            OnExit();
        }
    }
}
