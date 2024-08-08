using ChenChen_Thing;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Building : PawnJob
    {
        private readonly static float tick = 50;

        private Building building;
        private float _timer;
        private float _timeOne;

        public PawnJob_Building(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            building = target.TargetA.GetComponent<Building>();
            _timeOne = Mathf.Lerp(0.1f, 1f, 1 - (pawn.Attribute.A_Construction.Value / 20));
        }

        public override bool OnEnter()
        {
            if (building == null)
            {
                return false;
            }

            var baseResult = base.OnEnter();
            if(baseResult != true) return baseResult;

            if (!pawn.MoveController.GoToHere(building.gameObject, Urgency.Urge, pawn.WorkRange))
            {
                DebugLogDescription = ("无法移动到目标");
                return false;
            }

            pawn.JobToDo(target);
            Description = "前往建造" + building.name;

            return true;
        }

        public override StateType OnUpdate()
        {
            if (building == null)
            {
                return StateType.Failed;
            }

            if (building.Workload <= 0)
            {
                return StateType.Success;
            }

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();
                Description = "正在建造" + target.TargetA.name;

                _timer += Time.deltaTime;
                if (_timer > _timeOne)
                {
                    building.OnBuild(1);
                    _timer = 0;
                }
            }

            return StateType.Doing;
        }

        public override void OnInterrupt()
        {
            building.OnInterpretBuild();
            OnExit();
        }
    }
}
