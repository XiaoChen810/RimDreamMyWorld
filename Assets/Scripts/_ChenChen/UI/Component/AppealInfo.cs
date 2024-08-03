using ChenChen_Thing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class AppealInfo : CC_Button
    {
        [SerializeField] private Text Name;
        [SerializeField] private Text Description;
        [SerializeField] private Text Int_CostFabric;
        [SerializeField] private Text Int_Workload;
        [SerializeField] private Image Icon;

        public AppealDef Def;

        public void Set(AppealDef def)
        {
            this.Def = def;
            Name.text = def.name;
            Description.text = def.description; 
            Int_CostFabric.text = def.costFabric.ToString();
            Int_Workload.text = def.workload.ToString();
            Icon.sprite = def.icon;
        }
    }
}