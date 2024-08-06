using ChenChen_AI;
using ChenChen_Core;
using ChenChen_UI;
using UnityEngine;

namespace ChenChen_Thing
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class WorkSpace : MonoBehaviour, IDetailView , IGrant
    {
        // Component
        public SpriteRenderer SR;
        public BoxCollider2D Coll;

        // Type
        public WorkSpaceType WorkSpaceType;

        // 细节视图
        protected DetailView _detailView;
        public DetailView DetailView
        {
            get
            {
                if (_detailView == null)
                {
                    if (!TryGetComponent(out _detailView))
                    {
                        _detailView = gameObject.AddComponent<DetailView_WorkSpace>();
                    }
                }
                return _detailView;
            }
        }

        protected virtual void OnEnable()
        {
            SR = GetComponent<SpriteRenderer>();
            Coll = GetComponent<BoxCollider2D>();
        }

        public void SetSize(Vector2 oneV, Vector2 twoV)
        {
            float width = Mathf.Abs(oneV.x - twoV.x);
            float height = Mathf.Abs(oneV.y - twoV.y);
            SR.size = new Vector2(width, height);
            Coll.size = new Vector2(width, height);
            transform.position = (oneV + twoV) / 2f;

            AfterSizeChange();
        }

        protected abstract void AfterSizeChange();

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
                Debug.Log("Permission is locked. Cannot grant permission.");
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