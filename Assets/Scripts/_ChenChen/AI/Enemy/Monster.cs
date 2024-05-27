using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class Monster : MonoBehaviour
    {
        /// <summary>
        /// �����ƶ��Ŀ���
        /// </summary>
        public MonsterController MoveController { get; protected set; }

        /// <summary>
        /// ���ﶯ��״̬����
        /// </summary>
        public Animator Animator { get; protected set; }

        [Header("Index ID")]
        public int IndexId = -1;
        [Header("�ƶ����")]
        public float moveDuration = 5;
        [Header("��Ұ��Χ")]
        public float seeRange = 10;

        private void Start()
        {
            SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].sortingLayerName = "Above";
            }
            MoveController = GetComponent<MonsterController>();
            Animator = GetComponent<Animator>();
            StartCoroutine(MoveCo());
        }

        IEnumerator MoveCo()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);

                // �й��ƶ�
                if (MoveController.ReachDestination)
                {
                    Vector2 p = transform.position;
                    p += new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
                    MoveController.GoToHere(p);
                }
                else
                {
                    // �����Χ�������ˣ��ߵ��������
                    if(FindOtherPawn()) continue; 

                    // �ȴ�moveDuration��ִ����һ���ж�
                    yield return new WaitForSeconds(moveDuration);
                }


            }
        }

        private bool FindOtherPawn()
        {
            float minDistance = float.MaxValue;
            Vector3 targetPosition = transform.position;
            foreach (var pawn in GameManager.Instance.PawnGeneratorTool.PawnsList)
            {
                float distacne = (Vector2.Distance(transform.position, pawn.transform.position));
                if (distacne < seeRange)
                {
                    if (minDistance > distacne)
                    {
                        minDistance = distacne;
                        targetPosition = pawn.transform.position;
                    }
                }
            }
            if (targetPosition != transform.position)
            {
                MoveController.GoToHere(targetPosition);
                return true;
            }
            return false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Pawn") && collision.TryGetComponent<Pawn>(out Pawn pawn))
            {
                pawn.GetDamage(5);
                MoveController.StopMove(3);
            }
        }
    }
}