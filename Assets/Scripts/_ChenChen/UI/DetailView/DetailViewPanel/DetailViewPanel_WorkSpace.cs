using ChenChen_Thing;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class DetailViewPanel_WorkSpace : DetailViewPanel
    {
        private WorkSpace workSpace;

        public DetailViewPanel_WorkSpace(WorkSpace workSpace, Callback onEnter, Callback onExit) : base(onEnter, onExit)
        {
            this.workSpace = workSpace;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            SetButton("删除", () =>
            {
                Debug.Log($"删除工作区: {workSpace.name}, 但还没做");
            });

        }
    }
}