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

            //�߼�
            if (!target.GameObject.TryGetComponent<Pawn>(out Pawn targetPawn))
            {
                DebugLogDescription = ("������Pawn");
                return false;
            }

            // ��������Ŀ��㣬ǰ��Ŀ�꣬�߹�ȥ
            if (!pawn.MoveController.GoToHere(targetPawn.gameObject, Urgency.Normal, 2))
            {
                DebugLogDescription = ("�޷�ǰ��Ŀ��λ��");
                return false;
            }

            // ���������ȡ����
            pawn.JobToDo(targetPawn.gameObject);

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            // �ж��Ƿ񵽴�Ŀ��㸽��
            if (pawn.MoveController.ReachDestination)
            {
                // �������������罻
                pawn.JobDoing();

                string me = pawn.Def.PawnName;
                string him = target.GetComponent<Pawn>().Def.PawnName;
                string narrative = $"{me} ���ں� {him} ��������";
                ScenarioManager.Instance.Narrative(narrative, pawn.gameObject);

                // �Ƴ�����
                pawn.EmotionController.RemoveEmotion(EmotionType.distressed);

                return StateType.Success;
            }

            //�߼�
            return StateType.Doing;
        }
    }
}