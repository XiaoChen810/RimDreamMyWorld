using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Trade : PawnJob
    {
        //持续最大时间
        private readonly static float tick = 120;

        private readonly Animal animal = null;
        private readonly float tradeTime = 5;
        private float timer = 0;

        public PawnJob_Trade(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            animal = target.TargetA.GetComponent<Animal>();
        }

        public override bool OnEnter()
        {
            if (animal == null)
            {
                return false;
            }

            if (!animal.WaitToTrade)
            {
                return false;
            }

            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            if (!pawn.MoveController.GoToHere(target.TargetA,endReachedDistance: pawn.WorkRange))
            {
                DebugLogDescription = "无法移动到目标";
                return false;
            }

            animal.Trade();
            pawn.JobToDo(target);
            Description = "前往驯服" + animal.name;
            
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
                pawn.ChangeMyBar (1 - timer / tradeTime);

                if(timer >= tradeTime)
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
    }
}