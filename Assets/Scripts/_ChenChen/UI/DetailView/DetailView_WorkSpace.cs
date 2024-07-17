using ChenChen_Thing;
using UnityEngine;

namespace ChenChen_UI
{
    public class DetailView_WorkSpace : DetailView
    {
        [SerializeField] protected WorkSpace workSpace;
        [SerializeField] protected WorkSpace_Farm workSpace_Farm = null;

        private void OnEnable()
        {
            workSpace = GetComponent<WorkSpace>();
            TryGetComponent(out workSpace_Farm);
        }

        public override void OpenPanel()
        {
            PanelManager panel = DetailViewManager.Instance.PanelManager;
            panel.RemoveTopPanel(panel.GetTopPanel());
            panel.AddPanel(new DetailViewPanel_WorkSpace(workSpace, StartShow, EndShow));
        }

        protected override void UpdateShow(DetailViewPanel panel)
        {
            if (panel == null) return;
            if (workSpace == null) return;
            content.Clear();
            // 如果工作区是农业类型的特殊情况
            if (workSpace_Farm != null)
            {
                content.Add($"当前种植作物: {workSpace_Farm.CurCrop.CropName}");
            }
            //Content.Add($"使用者: {(workSpace.TheUsingPawn != null ? workSpace.TheUsingPawn.name : null)}");
            panel.SetView(
                workSpace.name,
                content
                );
        }
    }
}