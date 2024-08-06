using ChenChen_Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class ApparelInfo : CC_Button
    {
        [Header("Info")]
        [SerializeField] private Text Name;
        [SerializeField] private Text Description;
        [SerializeField] private Text Int_Cost;
        [SerializeField] private Text Int_Workload;
        [SerializeField] private Image Icon;

        [HideInInspector] public ApparelDef Def;

        public void Set(ApparelDef def)
        {
            Def = def;
            Name.text = def.name;
            Description.text = def.description; 
            Int_Workload.text = def.workload.ToString();
            Icon.sprite = def.sprite;
            string costContent = string.Empty;
            var cost = def.requiredMaterials;
            foreach (var mat in cost)
            {
                costContent += mat.ToString() + ", ";
            }
            Int_Cost.text = costContent;
        }
    }
}