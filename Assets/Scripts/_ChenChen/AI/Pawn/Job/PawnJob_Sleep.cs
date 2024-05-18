using ChenChen_BuildingSystem;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Sleep : PawnJob
    {
        //�������ʱ��
        private readonly static float tick = 500;
        private GameObject target;
        private Thing_Bed bed;
        public PawnJob_Sleep(Pawn pawn, GameObject bed) : base(pawn, tick, null)
        {
            target = bed;
        }

        public override bool OnEnter()
        {
            if(!target.TryGetComponent<Thing_Bed>(out bed)) 
            {
                DebugLogDescription = ("û��Thing_Bed���");
                return false;
            }

            // ����ȡ��Ȩ�ޣ�Ԥ����ǰ���������Ŀ�걻ʹ��
            if (!bed.GetPermission(_pawn))
            {
                DebugLogDescription = ("Ŀ���Ѿ��������˱�ʹ��");
                return false;
            }

            // ��������Ŀ��㣬ǰ��Ŀ�꣬�߹�ȥ
            if (!_pawn.MoveController.GoToHere(bed.transform.position, Urgency.Normal, 0.01f))
            {
                DebugLogDescription = ("�޷��ƶ���Ŀ���");
                return false;
            }

            // ���������ȡ����
            _pawn.JobToDo(bed.gameObject);

            return true;
        }

        public override StateType OnUpdate()
        {
            if (target == null) return StateType.Failed;
            // �ж��Ƿ񵽴�Ŀ��㸽��
            if (_pawn.MoveController.ReachDestination)
            {
                // ������������˯��
                _pawn.JobDoing();

                // ���Ŷ���
                _pawn.Animator.SetBool("IsDie", true);

                // ˯��
                _pawn.Info.Sleepiness.CurValue += Time.deltaTime;
            }

            if(_pawn.Info.Sleepiness.IsMax)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            // �黹Ŀ��ʹ��Ȩ��
            bed.RevokePermission(_pawn);

            // ��������״̬
            _pawn.JobDone();

            // ��������
            _pawn.Animator.SetBool("IsDie", false);
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}