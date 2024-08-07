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
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            //�߼�
            if (!target.TargetA.TryGetComponent<IOperate>(out operate))
            {
                DebugLogDescription = $"��Ŀ���� IOperate �ӿ�: {target.TargetA.name}";
                return false;
            }

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
            if (pawn.MoveController.ReachDestination)
            {
                if(timer < onceTime)
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    operate.Operate();
                    return StateType.Success;
                }            
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

            //�߼�
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}