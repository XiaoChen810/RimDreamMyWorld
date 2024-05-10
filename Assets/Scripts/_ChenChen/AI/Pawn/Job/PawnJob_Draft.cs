using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Draft : PawnJob
    {
        private readonly static float tick = 50;

        public PawnJob_Draft(Pawn pawn,bool openOrclose) : base(pawn, tick)
        {
            pawn.Info.IsDrafted = openOrclose;
        }

        public override bool OnEnter()
        {
            _pawn.JobDoing();
            _pawn.JobCanGet();
            return true;
        }

        public override StateType OnUpdate()
        {
            if (_pawn.Info.IsDrafted)
            {
                return StateType.Doing;
            }
            return StateType.Success;
        }

        public override void OnExit()
        {
            _pawn.JobDone();
        }

        public override void OnInterrupt()
        {
            // ��������£���������״̬ʱ����ӽ�����ǰ�棬��״̬��ɣ���������״̬
            if (_pawn.Info.IsDrafted)
            {
                _stateMachine.StateQueue.Enqueue(new PawnJob_Draft(_pawn, true));
            }
        }
    }
}