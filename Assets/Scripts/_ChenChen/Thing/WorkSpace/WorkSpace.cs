using ChenChen_UI;
using UnityEngine;

namespace ChenChen_Thing
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class WorkSpace : PermissionBase, IDetailView
    {
        // Component
        public SpriteRenderer SR;
        public BoxCollider2D Coll;

        // Type
        public WorkSpaceType WorkSpaceType;

        // œ∏Ω⁄ ”Õº
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
            //transform.localScale = new Vector3(width, height, 1);
            transform.position = (oneV + twoV) / 2f;

            AfterSizeChange();
        }

        protected abstract void AfterSizeChange();
    }
}