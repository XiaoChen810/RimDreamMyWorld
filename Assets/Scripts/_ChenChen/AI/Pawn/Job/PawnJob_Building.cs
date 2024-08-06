using ChenChen_Thing;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Building : PawnJob
    {
        private readonly static float tick = 50;

        private Building targetComponent;
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

            targetComponent = target.GetComponent<Building>();
            if (targetComponent == null)
            {
                DebugLogDescription = ("尝试获取组件失败");
                return false;
            }

            if (!pawn.MoveController.GoToHere(target.Positon, Urgency.Urge, pawn.WorkRange))
            {
                DebugLogDescription = ("无法移动到目标点");
                return false;
            }

            pawn.JobToDo(target.GameObject);
            this.Description = "前往建造" + target.GameObject.name;

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();
                this.Description = "正在建造" + target.GameObject.name;

                _time += Time.deltaTime;
                if (_time > _timeOne)
                {
                    targetComponent.OnBuild(1);
                    _time = 0;
                }
            }

            if (targetComponent.Workload <= 0)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnInterrupt()
        {
            targetComponent.OnInterpretBuild();
            OnExit();
        }
    }
}
