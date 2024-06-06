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
        public float moveDuration = 0.5f;
        [Header("��Ұ��Χ")]
        public float seeRange = 10;
        [Header("����")]
        public Pawn pawn;
        public bool isBattling = false;
        public float attackDamage = 0;
        public float attackRange = 2;
        public float attackWaitTime = 2.5f;
        [Header("����")]
        public float MaxHp = 100;
        public bool IsDie;

        private float Hp;
        private float _lastMoveTime; 
        private float _lastAttackTime;
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
                // ��������һ��ʱ��ʬ����ʧ���Żس���
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

            // ÿ��moveDuration����һ���ж�
            if (Time.time > _lastMoveTime + moveDuration)
            {
                _lastMoveTime = Time.time;

                // ����ս��ʱ�в�ͬ���ж�
                if (isBattling)
                {
                    if (pawn == null)
                    {
                        FindOtherPawn();
                    }
                }
                else
                {
                    FindOtherPawn();

                    if (MoveController.ReachDestination)
                    {
                        if (pawn != null)
                        {
                            MoveController.GoToHere(pawn.transform.position, attackRange);
                        }
                        else
                        {
                            // δ���ֵ��ˣ�������ߣ������������ĵ�ƫ��
                            Vector2 p = transform.position;
                            p += new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
                            Vector2 center = new Vector3(ChenChen_Map.MapManager.Instance.CurMapWidth / 2, ChenChen_Map.MapManager.Instance.CurMapHeight / 2) - transform.position;
                            p += Random.value * 0.05f * center ;
                            MoveController.GoToHere(p);
                        }
                    }
                }
            }

            // ��������һ�ι���ʱ������ж��ж�
            if (Time.time > _lastAttackTime + attackWaitTime)
            {
                // ս��״̬�½����ж�
                if (isBattling && pawn != null)
                {
                    MoveController.FilpIt(pawn.transform.position.x);
                    Animator.SetTrigger("attack");
                    _lastAttackTime = Time.time;
                }
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Pawn������
            if (collision.CompareTag("PawnAttackBox"))
            {
                Pawn p = collision.GetComponentInParent<Pawn>();
                if (p != null)
                {
                    GetDamage(p.AttackDamage);
                }
                else
                {
                    Debug.LogWarning("û�����");
                }
            }
            // Bullet������
            if (collision.CompareTag("BulletAttackBox"))
            {
                Bullet b = collision.GetComponent<Bullet>();
                if (b != null)
                {
                    GetDamage(b.damage);
                }
                else
                {
                    Debug.LogWarning("û�����");
                }
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            // �Ѿ���ս���У����أ����ټ���
            if (isBattling) return;
            if (collision.CompareTag("Pawn") && collision.TryGetComponent<Pawn>(out Pawn p))
            {
                // ��������ҵ�Ŀ�꣬����
                if (p != pawn) return;
                // ���δ���빥�����룬����
                if(Vector2.Distance(p.transform.position,this.transform.position) > attackRange) return;
                isBattling = true;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Pawn") && collision.TryGetComponent<Pawn>(out Pawn p))
            {
                // ��������ҵ�Ŀ�꣬����
                if (p != pawn) return;
                isBattling = false;
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