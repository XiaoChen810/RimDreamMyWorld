using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class Monster : MonoBehaviour
    {
        /// <summary>
        /// 人物移动的控制
        /// </summary>
        public MonsterController MoveController { get; protected set; }

        /// <summary>
        /// 人物动画状态控制
        /// </summary>
        public Animator Animator { get; protected set; }

        [Header("Index ID")]
        public int IndexId = -1;
        [Header("移动间隔")]
        public float moveDuration = 0.5f;
        [Header("视野范围")]
        public float seeRange = 10;
        [Header("攻击")]
        public Pawn pawn;
        public bool isBattling = false;
        public float attackRange = 2;
        public float attackWaitTime = 2.5f;
        private bool attackWait = false;
        [Header("血量")]
        public float HP = 100;

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
                yield return new WaitForSeconds(moveDuration);

                if (isBattling) continue;

                FindOtherPawn();

                if (MoveController.ReachDestination)
                {
                    if(pawn != null)
                    {
                        MoveController.GoToHere(pawn.transform.position, attackRange);
                    }
                    else
                    {
                        Vector2 p = transform.position;
                        p += new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
                        MoveController.GoToHere(p);
                    }
                }
            }
        }

        private void FindOtherPawn()
        {
            float minDistance = float.MaxValue;
            Pawn targetPawn = null;
            foreach (var p in GameManager.Instance.PawnGeneratorTool.PawnsList)
            {
                float distacne = (Vector2.Distance(transform.position, p.transform.position));
                if (distacne < seeRange)
                {
                    if (minDistance > distacne)
                    {
                        minDistance = distacne;
                        targetPawn = p.GetComponent<Pawn>();
                    }
                }
            }
            if (targetPawn != null)
            {
                pawn = targetPawn;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Pawn"))
            {
                isBattling = true;
            }
            if(collision.CompareTag("PawnAttackBox"))
            {
                GetDamage(10);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Pawn"))
            {
                isBattling = false;
            }
        }

        private void Update()
        {
            if (!attackWait && isBattling && pawn != null)
            {
                attackWait = true;
                Animator.SetTrigger("attack");
                pawn.GetDamage(this.gameObject, 0);
                StartCoroutine(AttackWaitCO());
            }
        }



        IEnumerator AttackWaitCO()
        {
            yield return new WaitForSeconds(attackWaitTime);
            attackWait = false;
        }

        public void GetDamage(float damage)
        {
            HP -= damage;
            Animator.SetTrigger("hurt");
            if (HP <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            GameManager.Instance.MonsterGeneratorTool.MonstersList.Remove(this);
        }
    }
}