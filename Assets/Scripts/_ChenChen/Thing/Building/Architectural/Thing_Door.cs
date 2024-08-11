using ChenChen_Core;
using System;
using UnityEngine;

namespace ChenChen_Thing
{
    [RequireComponent(typeof(Collider2D))]
    public class Thing_Door : Thing_Architectural
    {
        [Header("门")]
        [SerializeField] private bool isOpen;
        [SerializeField] private SpriteRenderer srDoor;
        [SerializeField] private Sprite doorCloseSprite;
        [SerializeField] private Sprite doorOpenSprite;

        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
            set
            {
                if (isOpen == value) return;
                if (isOpen)
                {
                    OpenDoor();
                }
                else
                {
                    CloseDoor();
                }
                isOpen = value;
            }
        }

        public override void OnCompleteBuild()
        {
            base.OnCompleteBuild();
            SR.sprite = null;
            srDoor.sprite = doorCloseSprite;
            RoomManager.Instance.AddWall(transform.position);
        }

        protected void OpenDoor()
        {
            if (LifeState == BuildingLifeStateType.FinishedBuilding)
            {
                srDoor.sprite = Def.PreviewSprite;
            }

        }

        protected void CloseDoor()
        {
            if (LifeState == BuildingLifeStateType.FinishedBuilding)
            {
                srDoor.sprite = doorOpenSprite;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Pawn"))
            {
                IsOpen = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Pawn"))
            {
                IsOpen = false;
            }
        }

        private void Update()
        {
            srDoor.color = SR.color;
        }
    }
}
