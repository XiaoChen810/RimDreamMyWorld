using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{
    public class Thing_灯 : Thing_Building
    {
        public Animator anim;
        public bool IsOpen;

        private void Update()
        {
            bool isDayLight = (GameManager.Instance.currentHour >= 6 && GameManager.Instance.currentHour <= 18);
            IsOpen = !isDayLight;

            anim.SetBool("IsOpen",IsOpen);
        }
    }
}