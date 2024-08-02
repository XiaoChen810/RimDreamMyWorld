using ChenChen_AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class Panel_PawnInfo : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/PawnInfoPanel";
        static readonly string workPriorityPrefabPath = "UI/Component/WorkPriority";
        public Panel_PawnInfo(Pawn pawn) : base(new UIType(path))
        {
            this.pawn = pawn;
        }

        private Pawn pawn;

        private Slider healthSlider;
        private Slider emotionSlider;
        private Slider sleepSlider;

        public override void OnEnter()
        {
            healthSlider = UITool.TryGetChildComponentByName<Slider>("Health_Slider");
            emotionSlider = UITool.TryGetChildComponentByName<Slider>("Emotion_Slider");
            sleepSlider = UITool.TryGetChildComponentByName<Slider>("Sleep_Slider");
            UITool.TryGetChildComponentByName<Text>("PawnName").text = pawn.name;

            // 加载工作优先级
            Transform workTab = UITool.GetChildByName("WorkTab").transform;
            HumanMain main = pawn.gameObject.GetComponent<HumanMain>();
            if(main != null)
            {
                GameObject workPriorityPrefab = Resources.Load(workPriorityPrefabPath) as GameObject;
                foreach (var jobGiver in main.JobGivers)
                {
                    if(!string.IsNullOrEmpty(jobGiver.JobName))
                    {
                        CC_WorkPriority wp = Object.Instantiate(workPriorityPrefab, workTab).GetComponent<CC_WorkPriority>();
                        wp.Init(jobGiver, main);
                    }

                }
            }
        }

        public void UpdateSlider()
        {
            healthSlider.value = pawn.Info.HP.Percentage;
            emotionSlider.value = pawn.Info.Happiness.Percentage;
            sleepSlider.value = pawn.Info.Sleepiness.Percentage;
        }
    }
}