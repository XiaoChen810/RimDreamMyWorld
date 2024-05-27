using System;
using UnityEngine;

namespace ChenChen_Thing
{
    [RequireComponent(typeof(Collider2D))]
    public class Thing_Door : Thing_Building
    {
        [SerializeField] private bool isOpen;
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

        protected void OpenDoor()
        {
            GetComponent<SpriteRenderer>().sprite = Def.PreviewSprite;
        }

        protected void CloseDoor()
        {
            GetComponent<SpriteRenderer>().sprite = doorOpenSprite;
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
