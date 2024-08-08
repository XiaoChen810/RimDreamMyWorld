using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class MadeInfo : CC_Button
    {
        [Header("Info")]
        [SerializeField] private Text Name;
        [SerializeField] private Text Int_Cost;
        [SerializeField] private Image Icon;

        public void Set(string name, string cost, Sprite icon)
        {
            Name.text = name;
            Int_Cost.text = cost;
            Icon.sprite = icon;
        }
    }
}
