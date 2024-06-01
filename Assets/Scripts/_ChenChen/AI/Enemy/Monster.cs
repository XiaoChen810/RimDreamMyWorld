using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    [RequireComponent(typeof(MonsterController))]
    [RequireComponent(typeof(Animator))]
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
        public float attackDamage = 0;
        public float attackRange = 2;
        public float attackWaitTime = 2.5f;
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
            StartCoroutine(AttackCo());
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("PawnAttackBox"))
            {
                Pawn p = collision.GetComponentInParent<Pawn>();
                if (p != null)
                {
                    GetDamage(p.AttackDamage);
                }
                else
                {
                    Debug.LogWarning("没有组件");
                }
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            // 已经在战斗中，返回，减少计算
            if (isBattling) return;
            if (collision.CompareTag("Pawn") && collision.TryGetComponent<Pawn>(out Pawn p))
            {
                // 如果不是我的目标，返回
                if (p != pawn) return;
                // 如果未进入攻击距离，返回
                if(Vector2.Distance(p.transform.position,this.transform.position) > attackRange) return;
                isBattling = true;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Pawn") && collision.TryGetComponent<Pawn>(out Pawn p))
            {
                // 如果不是我的目标，返回
                if (p != pawn) return;
                isBattling = false;
            }
        }
        IEnumerator MoveCo()
        {
            while (true)
            {
                yield return new WaitForSeconds(moveDuration);

                // 正在战斗时有不同的判断
                if (isBattling)
                {
                    if(pawn == null)
                    {
                        FindOtherPawn();
                    }
                    else
                    {
                        continue;
                    }
                }

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
        IEnumerator AttackCo()
        {
            while(true)
            {
                yield return null;

                if (isBattling && pawn != null)
                {
                    MoveController.FilpIt(pawn.transform.position.x);
                    Animator.SetTrigger("attack");
                    // 攻击后等待一段时间再判断
                    yield return new WaitForSeconds(attackWaitTime);
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
        private void GetDamage(float damage)
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