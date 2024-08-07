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
            content.Add($"�;ö�: {thing.Durability} / {thing.MaxDurability}");                     
            content.Add($"ʹ����: {(thing.UserPawn != null ? thing.UserPawn.name : null)}");

            panel.SetView(thing.name, content);
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