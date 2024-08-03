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
            SR.color = new Color(1, 1, 1, 0);
            RoomManager.Instance.AddWall(transform.position);
        }

        protected void OpenDoor()
        {
            srDoor.sprite = Def.PreviewSprite;
        }

        protected void CloseDoor()
        {
            srDoor.sprite = doorOpenSprite;
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
    }
}
