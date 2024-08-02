using ChenChen_UI;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Socialize : PawnJob
    {
        //�������ʱ��
        private readonly static float tick = 500;

        public PawnJob_Socialize(Pawn pawn, GameObject target) : base(pawn, tick, new TargetPtr(target))
        {
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            if (!target.GameObject.TryGetComponent<Pawn>(out Pawn targetPawn))
            {
                DebugLogDescription = ("������Pawn");
                return false;
            }

            if (!pawn.MoveController.GoToHere(targetPawn.gameObject, Urgency.Normal, 2))
            {
                DebugLogDescription = ("�޷�ǰ��Ŀ��λ��");
                return false;
            }

            pawn.JobToDo(targetPawn.gameObject);
            this.Description = $"׼����{targetPawn.name}һ����";

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();

                string him = target.GetComponent<Pawn>().Def.PawnName;
                this.Description = $"���ں�{him}һ����";

                pawn.EmotionController.RemoveEmotion(EmotionType.distressed);

                return StateType.Success;
            }

            //�߼�
            return StateType.Doing;
        }
    }
}