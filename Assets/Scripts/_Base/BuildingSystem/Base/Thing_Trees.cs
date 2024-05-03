using ChenChen_BuildingSystem;
using UnityEngine;

namespace ChenChen_BuildingSystem
{
    [RequireComponent(typeof(Collider2D))]
    public class Thing_Trees : ThingBase
    {
        private Collider2D coll;
        private SpriteRenderer sr;

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
            LifeState = initial_State;
            MapName = mapName;
        }

        public override void OnMarkBuild()
        {
            throw new System.NotImplementedException();
        }

        public override void OnBuild(int thisWorkload)
        {
            throw new System.NotImplementedException();
        }

        public override void OnComplete()
        {
            throw new System.NotImplementedException();
        }

        public override void OnCancel()
        {
            throw new System.NotImplementedException();
        }

        public override void OnInterpret()
        {
            throw new System.NotImplementedException();
        }

        public override void OnMarkDemolish()
        {
            throw new System.NotImplementedException();
        }

        public override void OnDemolish(int value)
        {
            throw new System.NotImplementedException();
        }

        public override void OnDemolished()
        {
            throw new System.NotImplementedException();
        }
    }
}