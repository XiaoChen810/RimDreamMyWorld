using ChenChen_UI;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Socialize : PawnJob
    {
        //�������ʱ��
        private readonly static float tick = 500;
        private readonly Pawn targetPawn = null;

        public PawnJob_Socialize(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            targetPawn = target.TargetA.GetComponent<Pawn>();
        }

        public override bool OnEnter()
        {
            if(targetPawn == null)
            {
                return false;
            }

            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            if (!pawn.MoveController.GoToHere(targetPawn.gameObject, Urgency.Normal, 2))
            {
                DebugLogDescription = ("�޷�ǰ��Ŀ��λ��");
                return false;
            }

            pawn.JobToDo(target);
            Description = $"׼����{targetPawn.name}�罻";

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();

                Description = $"���ں�{targetPawn.Def.PawnName}�罻";

                pawn.EmotionController.RemoveEmotion(EmotionType.distressed);

                return StateType.Success;
            }

            //�߼�
            return StateType.Doing;
        }
    }
}