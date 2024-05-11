using ChenChen_BuildingSystem;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Demolished : PawnJob
    {
        private readonly static float tick = 50;
        private GameObject building;
        private Thing_Building currentWorkObject;
        private float _time;

        public PawnJob_Demolished(Pawn pawn, GameObject building = null) : base(pawn, tick)
        {
            this.building = building;
        }

        public override bool OnEnter()
        {
            if (building == null) return false;

            // ���Ի�ȡ��ͼ
            currentWorkObject = building.GetComponent<Thing_Building>();
            if (currentWorkObject == null)
            {
                DebugLogDescription = ("���Ի�ȡ���ʧ��");
                return false;
            }

            // ����ȡ��Ȩ�ޣ�Ԥ����ǰ���������Ŀ�걻ʹ��
            if (!currentWorkObject.GetPermission(_pawn))
            {
                DebugLogDescription = ("Ŀ���Ѿ������˱�ʹ��");
                return false;
            }

            // ��������Ŀ��㣬ǰ��Ŀ�꣬�ܹ�ȥ
            bool flag = _pawn.MoveControl.GoToHere(building.transform.position, Urgency.Urge, _pawn.WorkRange);
            if (!flag)
            {
                DebugLogDescription = ("�޷��ƶ���Ŀ���");
                return false;
            }

            // ���������޷���ȡ����
            _pawn.JobToDo(building);

            return true;
        }

        public override StateType OnUpdate()
        {
            // �������˲����״̬��������ͣ�����Խ�����һ��״̬
            if (currentWorkObject == null)
            {
                return StateType.Success;
            }

            if (currentWorkObject.MaxDurability <= 0)
            {
                return StateType.Success;
            }

            // �ж��Ƿ񵽴�Ŀ��㸽��
            if (_pawn.MoveControl.ReachDestination)
            {
                // �����������ڹ���
                _pawn.JobDoing();

                // ִ�й���
                _time += Time.deltaTime;
                if (_time > 2)
                {
                    currentWorkObject.OnDemolish(_pawn.Attribute.A_Construction.Value);
                    _time = 0;
                }

                // ���Ŷ���
                _pawn.Animator.SetBool("IsDoing", true);
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            // ��������״̬
            _pawn.JobDone();

            // ��������
            _pawn.Animator.SetBool("IsDoing", false);
        }

        public override void OnInterrupt()
        {
            // �黹Ŀ��ʹ��Ȩ��
            currentWorkObject.RevokePermission(_pawn);

            OnExit();
        }
    }
}