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
            ChangeLifeState(initial_State);
            MapName = mapName;
        }
    }
}