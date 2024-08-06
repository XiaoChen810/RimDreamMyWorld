using UnityEngine;
using ChenChen_UI;
using ChenChen_Core;
using ChenChen_AI;

namespace ChenChen_Thing
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class Thing : MonoBehaviour, IDetailView , IGrant
    {
        public BoxCollider2D ColliderSelf { get; protected set; }

        public SpriteRenderer SR {  get; protected set; }

        #region - 重要属性 - 

        // 物品耐久度
        public int MaxDurability
        {
            get { return 100; }
        }

        public int Durability { get; protected set; }

        #endregion


        protected virtual void Start()
        {
            name = name.Replace("(Clone)", "").Trim();
            name = name.Replace("_Prefab", "").Trim();
            tag = "Thing";
        }

        protected virtual void OnEnable()
        {
            ColliderSelf = GetComponent<BoxCollider2D>();
            ColliderSelf.isTrigger = true;

            SR = GetComponentInChildren<SpriteRenderer>();
            SR.sortingLayerName = "Middle";
            SR.sortingOrder = -(int)transform.position.y;

            Durability = MaxDurability;
        }

        protected virtual void OnDestroy()
        {
            if (!Application.isPlaying) return;
            ThingSystemManager.Instance.RemoveThing(this.gameObject);
        }

        #region - IDetailView -
        protected DetailView _detailView;
        public virtual DetailView DetailView
        {
            get
            {
                if (_detailView == null)
                {
                    if (!TryGetComponent<DetailView>(out _detailView))
                    {
                        _detailView = gameObject.AddComponent<DetailView_Thing>();
                    }
                }
                return _detailView;
            }
        }
        #endregion

        #region - IGrant -
        [Header("权限")]
        [SerializeField] private bool isLocked;
        [SerializeField] private Pawn userPawn;

        public bool IsFree
        {
            get => !isLocked && userPawn == null;
        }

        public bool IsLocked
        {
            get => isLocked;
            set => isLocked = value;
        }

        public Pawn UserPawn
        {
            get => userPawn;
            set => userPawn = value;
        }

        public void GetPermission(Pawn pawn)
        {
            if (!isLocked)
            {
                userPawn = pawn;
                isLocked = true;
                // 实现获取权限的逻辑
                Debug.Log($"{pawn.name} has been granted permission.");
            }
            else
            {
                Debug.Log($"{pawn.name} Permission is locked. Cannot grant permission.");
            }
        }

        public void RevokePermission(Pawn pawn)
        {
            if (userPawn == pawn)
            {
                userPawn = null;
                isLocked = false;
                // 实现撤销权限的逻辑
                Debug.Log($"{pawn.name} has had their permission revoked.");
            }
            else
            {
                Debug.Log("Permission not granted to this pawn.");
            }
        }
        #endregion
    }
}