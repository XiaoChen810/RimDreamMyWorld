using ChenChen_BuildingSystem;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Demolished : JobBase
    {
        private GameObject building;
        private Building currentWorkObject;
        private float _time;

        public PawnJob_Demolished(Pawn pawn, GameObject building = null) : base(pawn)
        {
            this.building = building;
        }

        public override bool OnEnter()
        {
            if (building == null) return false;

            // ���Ի�ȡ��ͼ
            currentWorkObject = building.GetComponent<Building>();
            if (currentWorkObject == null)
            {
                Debug.LogWarning("The Building Don't Have Building Component");
                return false;
            }

            // ����ȡ��Ȩ�ޣ�Ԥ����ǰ���������Ŀ�걻ʹ��
            if (!currentWorkObject.GetPrivilege(pawn))
            {
                return false;
            }

            // ��������Ŀ��㣬ǰ��Ŀ�꣬�ܹ�ȥ
            pawn.MoveControl.GoToHere(building.transform.position, true);
            pawn.MoveControl.IsRun = true;
            // ���������޷���ȡ����
            pawn.JobToDo(building);

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
            if (Vector2.Distance(pawn.transform.position, building.transform.position) < pawn.WorkRange)
            {
                pawn.MoveControl.StopMove();

                // �����������ڹ���
                pawn.JobDoing();

                // ִ�й���
                _time += Time.deltaTime;
                if (_time > 2)
                {
                    currentWorkObject.Demolish(pawn.Attribute.A_Construction.Value);
                    _time = 0;
                }

                // ���Ŷ���
                pawn.Animator.SetBool("IsDoing", true);
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            // ��������״̬
            pawn.JobDone();
            pawn.MoveControl.IsRun = false;

            // ��������
            pawn.Animator.SetBool("IsDoing", false);
        }

        public override void OnInterrupt()
        {
            // �黹Ŀ��ʹ��Ȩ��
            currentWorkObject.RevokePrivilege(pawn);

            OnExit();
        }
    }
}