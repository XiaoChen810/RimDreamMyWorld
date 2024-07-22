using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{
    public class Thing_灯 : Thing_Furniture
    {
        public Animator anim;
        public bool IsOpen;

        private void Update()
        {
            bool isDayLight = GameManager.Instance.IsDayTime;
            IsOpen = !isDayLight;

            anim.SetBool("IsOpen",IsOpen);
        }
    }
}