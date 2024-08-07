using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Trade : PawnJob
    {
        //持续最大时间
        private readonly static float tick = 120;

        private Animal animal;
        private float tradeDuration = 5;
        private float timer = 0;

        public PawnJob_Trade(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            if(!target.TryGetComponent<Animal>(out animal))
            {
                Debug.LogError("错误！传入参数不正确");
            }
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            if (animal == null)
            {
                DebugLogDescription = "对应参数错误，该目标动物组件不存在";
                return false;
            }

            if (!animal.WaitToTrade)
            {
                DebugLogDescription = "该动物并不需要自己去驯服";
                return false;
            }

            if (!pawn.MoveController.GoToHere(target.TargetA,endReachedDistance: pawn.WorkRange))
            {
                DebugLogDescription = "无法移动到目标";
                return false;
            }

            animal.Trade();

            pawn.JobToDo(target);
            this.Description = "前往驯服" + target.TargetA.name;
            
            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            if (target.TargetA == null)
            {
                return StateType.Failed;
            }

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();
                timer += Time.deltaTime;
                pawn.ChangeMyBar (1 - timer / tradeDuration);

                if(timer >= tradeDuration)
                {
                    animal.CompleteTrade();
                    return StateType.Success;
                }
            }
            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

            pawn.ChangeMyBar(0);
            animal.StopTrade();
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}