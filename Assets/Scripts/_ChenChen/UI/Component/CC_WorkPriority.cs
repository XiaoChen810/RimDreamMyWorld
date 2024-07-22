using ChenChen_AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class CC_WorkPriority : MonoBehaviour
    {     
        private JobGiver myGiver;
        private HumanMain hm;

        [SerializeField] private Button myBtn;
        [SerializeField] private Text workNameText;
        [SerializeField] private Text workPriorityValueText;

        public void Init(JobGiver jobGiver,HumanMain humanMain)
        {
            myBtn.onClick.RemoveAllListeners();
            myBtn.onClick.AddListener(ChangeWorkPriority);
            myGiver = jobGiver;
            hm = humanMain;
            workNameText.text = jobGiver.JobName;
            workPriorityValueText.text = jobGiver.Priority.ToString();
        }

        private void ChangeWorkPriority()
        {
            int origin = myGiver.Priority;
            int changed = (origin + 1) % 4;
            hm.ChangeJobGiverPriority(myGiver, changed);
            workPriorityValueText.text = myGiver.Priority.ToString();
        }
    }
}