using ChenChen_Thing;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Operate : PawnJob
    {
        //�������ʱ��
        private readonly static float tick = 500;

        private IOperate operate = null;
        private float onceTime;
        private float timer = 0;

        public PawnJob_Operate(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            operate = target.TargetA.GetComponent<IOperate>();
        }

        public override bool OnEnter()
        {
            if (operate == null)
            {
                return false;
            }

            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            if (!pawn.MoveController.GoToHere(operate.OperationPosition))
            {
                DebugLogDescription = $"�޷��ƶ���������: {target.TargetA.name}";
                return false;
            }

            onceTime = operate.OnceTime;

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            //�߼�
            if (operate.IsCompleteOperate)
            {
                return StateType.Success;
            }

            if (pawn.MoveController.ReachDestination)
            {
                if (timer < onceTime)
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    operate.Operate();
                    timer = 0;
                }
            }

            return StateType.Doing;
        }
    }
}