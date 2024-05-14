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

        protected virtual void Start()
        {
            /* ������������ƶ���� */
            MoveController = GetComponent<AnimalMoveController>();

            /* ����������Ķ������ */
            Animator = GetComponent<Animator>();

            /* ����״̬�� */
            StateMachine = new StateMachine(this.gameObject);

            /* ����ͼ��Pawn�ͱ�ǩ */
            gameObject.layer = 9;
            gameObject.tag = "Animal";
        }
    }
}