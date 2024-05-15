using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AnimalMoveController))]
    public abstract class Animal : MonoBehaviour
    {
        /// <summary>
        /// ����״̬��
        /// </summary>
        public StateMachine StateMachine { get; protected set; }

        /// <summary>
        /// �����ƶ��Ŀ���
        /// </summary>
        public AnimalMoveController MoveController { get; protected set; }

        /// <summary>
        /// ���ﶯ��
        /// </summary>
        public Animator Animator { get; protected set; }

        public AnimalDef Def;
        public AnimalInfo Info;

        [Header("��ǰ״̬")]
        public List<string> CurrentStateList = new List<string>();

        protected virtual void Start()
        {
            /* ������������ƶ���� */
            MoveController = GetComponent<AnimalMoveController>();

            /* ����������Ķ������ */
            Animator = GetComponent<Animator>();

            /* ����״̬�� */
            StateMachine = new StateMachine(this.gameObject, new AnimalState_Idle(this));

            /* ����ͼ��Pawn�ͱ�ǩ */
            gameObject.layer = 9;
            gameObject.tag = "Animal";
        }

        public void Init(AnimalDef def,AnimalInfo info)
        {
            Def = def;
            Info = info;
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            �����б�Debug();
#endif
            StateMachine.Update();
        }

        protected void �����б�Debug()
        {
            CurrentStateList.Clear();
            CurrentStateList.Add("���ڣ�" + StateMachine.CurState?.ToString());
            CurrentStateList.Add("��һ����" + StateMachine.NextState?.ToString());
            foreach (var task in StateMachine.StateQueue)
            {
                CurrentStateList.Add("׼��" + task.ToString());
            }
        }
    }
}