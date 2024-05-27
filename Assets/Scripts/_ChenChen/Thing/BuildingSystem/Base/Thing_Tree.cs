using ChenChen_Thing;
using ChenChen_UI;
using UnityEngine;

namespace ChenChen_Thing
{
    [RequireComponent(typeof(Collider2D))]
    public class Thing_Tree : ThingBase, ICut
    {
        private Collider2D coll;
        private SpriteRenderer sr;

        public bool IsMarkCut;

        //protected new DetailView _detailView;
        public override DetailView DetailView
        {
            get
            {
                if (_detailView == null)
                {
                    if (!TryGetComponent<DetailView>(out _detailView))
                    {
                        _detailView = gameObject.AddComponent<DetailView_Tree>();
                    }
                }
                return _detailView;
            }
        }

        private void Start()
        {
            coll = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();
            coll.isTrigger = true;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Pawn"))
            {
                sr.color = new Color(1, 1, 1, 0.5f);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Pawn"))
            {
                sr.color = new Color(1, 1, 1, 1);
            }
        }

        public override void OnPlaced(BuildingLifeStateType initial_State, string mapName)
        {
            ChangeLifeState(initial_State);
            MapName = mapName;
        }

        public void OnMarkCut()
        {
            IsMarkCut = true;
        }

        public void OnCut(int value)
        {
            CurDurability -= value;
            if (CurDurability <= 0)
            {
                ThingSystemManager.Instance.RemoveThing(this.gameObject);

                Destroy(gameObject, 0.1f);
            }
        }

        public void OnCanclCut()
        {
            IsMarkCut = false;
        }
    }
}