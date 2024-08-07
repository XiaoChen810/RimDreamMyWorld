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

        private object lockObj = new object();

        [Header("权限")]
        [SerializeField] private bool isLocked;
        [SerializeField] private Pawn userPawn;

        public bool UnLock => isLocked;

        public Pawn UserPawn => userPawn;

        public void GetPermission(Pawn pawn)
        {
            lock (lockObj)
            {
                if (!isLocked)
                {
                    isLocked = true;
                    userPawn = pawn;
                    Debug.Log($"Permission granted to {pawn.name}");
                }
                else
                {
                    Debug.Log($"Permission denied to {pawn.name}. Already locked by {userPawn.name}");
                }
            }
        }

        public void RevokePermission(Pawn pawn)
        {
            lock (lockObj)
            {
                if (isLocked && userPawn == pawn)
                {
                    isLocked = false;
                    Debug.Log($"Permission revoked from {pawn.name}");
                    userPawn = null;
                }
                else
                {
                    Debug.Log($"Permission revoke failed for {pawn.name}");
                }
            }
        }
        #endregion
    }
}