using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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
        [Header("寻敌")]
        public Pawn pawn;
        public bool isBattling = false;
        public float findDuaration = 5f;
        [Header("攻击")]
        public float attackDamage = 0;
        public float attackRange = 2;
        public float attackDuaration = 2.5f;
        [Header("属性")]
        public float MaxHp = 100;
        public bool IsDie;

        private float Hp;
        private float _lastMoveTime; 
        private float _lastAttackTime;
        private float _lastFindTime;
        private float _diedTime;
        private ObjectPool<GameObject> _pool;

        public void Init(ObjectPool<GameObject> pool = null)
        {
            _pool = pool;
            isBattling = false;
            Hp = MaxHp;
            IsDie = false;
            _lastMoveTime = -_lastAttackTime;
            _lastAttackTime = -_lastAttackTime;
            _lastFindTime = -_lastFindTime;
            _diedTime = 0;
        }

        private void Start()
        {
            SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].sortingLayerName = "Above";
            }
            MoveController = GetComponent<MonsterController>();
            Animator = GetComponent<Animator>();
            Init();
        }
        private void Update()
        {
            if(IsDie)
            {
                // 死亡超过一定时间尸体消失，放回池中
                _diedTime += Time.deltaTime;
                if(_diedTime > 5)
                {
                    if(_pool != null)
                    {
                        _pool.Release(gameObject);
                        return;
                    }
                    Destroy(gameObject);
                }
                return;
            }

            // 每隔moveDuration进行一次移动判断
            if (Time.time > _lastMoveTime + moveDuration)
            {
                _lastMoveTime = Time.time;

                // 有攻击目标会走向攻击目标
                if (pawn != null)
                {
                    MoveController.GoToHere(pawn.transform.position, attackRange);
                }
                else
                {
                    // 在没有攻击目标和移动目标且不在战斗情况下
                    if (MoveController.ReachDestination && !isBattling)
                    {
                        // 没有攻击目标，随便走走，但会逐渐向中心点偏移
                        Vector2 p = transform.position;
                        p += new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
                        Vector2 center = new Vector3(ChenChen_Map.MapManager.Instance.CurMapWidth / 2, ChenChen_Map.MapManager.Instance.CurMapHeight / 2) - transform.position;
                        p += Random.value * 0.05f * center;
                        MoveController.GoToHere(p);
                    }
                }
            }
            // 每隔findDuaration进行一次寻敌判断
            if (Time.time > _lastFindTime + findDuaration)
            {
                _lastFindTime = Time.time;
                // 找一个视野范围内最近的目标
                FindOtherPawn();
            }
            // 当超过上一次攻击时间进行攻击判断
            if (Time.time > _lastAttackTime + attackDuaration)
            {
                // 战斗状态下，且目标不为空，面向目标，播放攻击动画
                if (isBattling && pawn != null)
                {
                    MoveController.FilpIt(pawn.transform.position.x);
                    Animator.SetTrigger("attack");
                    _lastAttackTime = Time.time;
                }
            }

            // 判断攻击目标有无进入攻击距离
            IsEnterAttackRange();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Pawn攻击盒
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
            // Bullet攻击盒
            if (collision.CompareTag("BulletAttackBox"))
            {
                Bullet b = collision.GetComponent<Bullet>();
                if (b != null)
                {
                    b.Hit();    // 打中后子弹消失
                    GetDamage(b.damage);
                }
                else
                {
                    Debug.LogWarning("没有组件");
                }
            }
        }
        private void IsEnterAttackRange()
        {
            if (pawn != null)
            {
                // 如果超出太远的距离则不追了
                if(!StaticFuction.CompareDistance(pawn.transform.position, this.transform.position, seeRange))
                {
                    pawn = null;
                    return;
                }
                // 如果进入攻击距离
                isBattling = StaticFuction.CompareDistance(pawn.transform.position, this.transform.position, attackRange);
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
            Hp -= damage;
            Animator.SetTrigger("hurt");
            if (Hp <= 0)
            {
                IsDie = true;
                Animator.SetBool("IsDie", true);
            }
        }
        private void OnDisable()
        {
            GameManager.Instance.MonsterGeneratorTool.MonstersList.Remove(this);
        }
    }
}