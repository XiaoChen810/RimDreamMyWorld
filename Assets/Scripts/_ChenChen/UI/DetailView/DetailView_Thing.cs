using ChenChen_Thing;
using UnityEngine;

namespace ChenChen_UI
{
    public class DetailView_Thing : DetailView
    {
        [SerializeField] protected Thing thing;

        private void OnEnable()
        {
            thing = GetComponent<Thing>();
        }

        public override void OpenPanel()
        {
            PanelManager panelManager = DetailViewManager.Instance.PanelManager;
            panelManager.RemovePanel(panelManager.GetTopPanel());
            panelManager.AddPanel(new DetailViewPanel_Thing(thing, StartShow, EndShow));
        }

        protected override void UpdateShow(DetailViewPanel panel)
        {
            if (panel == null) return;
            if (thing == null) return;
            content.Clear();
            content.Add($"�;ö�: {thing.CurDurability} / {thing.MaxDurability}");
            content.Add($"ʣ�๤����: {thing.Workload}");
            content.Add($"ʹ����: {(thing.TheUsingPawn != null ? thing.TheUsingPawn.name : null)}");
            panel.SetView(
                thing.Def.DefName,
                content
                );

            if (thing.LifeState == BuildingLifeStateType.MarkBuilding)
            {
                panel.RemoveAllButton();
                panel.SetButton("ȡ��", () =>
                {
                    thing.OnCancelBuild();
                });
            }
            if (thing.LifeState == BuildingLifeStateType.MarkDemolished)
            {
                panel.RemoveAllButton();
                panel.SetButton("ȡ��", () =>
                {
                    thing.OnCanclDemolish();
                });
            }
            if (thing.LifeState == BuildingLifeStateType.FinishedBuilding)
            {
                panel.RemoveAllButton();
                panel.SetButton("���", () =>
                {
                    thing.MarkToDemolish();
                });
            }
        }

        public override void StartShow()
        {
            base.StartShow();
        }

        public override void EndShow()
        {
            base.EndShow();
        }
    }
}