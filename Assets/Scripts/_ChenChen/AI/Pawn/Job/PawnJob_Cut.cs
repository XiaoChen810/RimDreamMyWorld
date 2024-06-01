using ChenChen_Thing;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Cut : PawnJob
    {
        //�������ʱ��
        private readonly static float tick = 500;

        private Thing_Tree tree;
        private float _time = 0;
        private float _timeOne = 0.76f; // һ�ζ�����ʱ��

        public PawnJob_Cut(Pawn pawn, GameObject target) : base(pawn, tick, new TargetPtr(target))
        {
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            //�߼�
            if(!target.TryGetComponent<Thing_Tree>(out tree))
            {
                DebugLogDescription = "��ȡ���ʧ��";
                return false;
            }

            // �ж���߽������ұ߽�
            Vector3 position;
            if (pawn.transform.position.x < target.Positon.x)
            {               
                 position = target.Positon + new Vector3(-1, 0.3f); 
            }
            else
            {
                 position = target.Positon + new Vector3(2, 0.3f);
            }

            if (!pawn.MoveController.GoToHere(position))
            {
                DebugLogDescription = "ǰ��Ŀ���ʧ��";
                return false;
            }

            // ���������ȡ����
            pawn.JobToDo(target.GameObject);
            this.Description = "ǰ������" + target.GameObject.name;

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            //�߼�
            if (pawn.MoveController.ReachDestination)
            {
                // �����������ڹ���
                pawn.JobDoing();
                this.Description = "���ڿ���" + target.GameObject.name;

                // ִ�й���
                _time += Time.deltaTime;
                if (_time > _timeOne)
                {
                    tree.OnCut(20);
                    _time = 0;
                }

                // ���Ŷ���
                pawn.MoveController.FilpIt(target.Positon.x);
                pawn.Animator.SetBool("IsLumbering", true);
            }

            // �������˽��죬״̬��������ͣ�����Խ�����һ��״̬
            if (tree.CurDurability <= 0)
            {
                return StateType.Success;
            }

            // ���ȡ��
            if (!tree.IsMarkCut)
            {
                return StateType.Interrupt;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

            // ��������
            pawn.Animator.SetBool("IsLumbering", false);
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}