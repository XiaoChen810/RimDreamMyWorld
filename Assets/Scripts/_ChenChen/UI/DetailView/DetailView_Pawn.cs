using UnityEngine;
using ChenChen_AI;

namespace ChenChen_UI
{
    public class DetailView_Pawn : DetailView
    {
        [SerializeField] protected Pawn pawn;

        private void OnEnable()
        {
            pawn = GetComponent<Pawn>();
        }

        public override void OpenIndicator()
        {
            if (indicator == null)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("Views/Indicator"), gameObject.transform);
                indicator = go.GetComponent<Indicator>();
                indicator.gameObject.name = "Indicator";
            }
            IsIndicatorOpen = true;
            indicator.gameObject.SetActive(true);
            pawn.Info.IsSelect = true;
        }

        public override void CloseIndicator()
        {
            if (indicator == null)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("Views/Indicator"), gameObject.transform);
                indicator = go.GetComponent<Indicator>();
                indicator.gameObject.name = "Indicator";
            }
            IsIndicatorOpen = false;
            indicator.gameObject.SetActive(false);
            pawn.Info.IsSelect = false;
        }

        public override void OpenPanel()
        {
            PanelManager panel = DetailViewManager.Instance.PanelManager;
            panel.RemovePanel(panel.GetTopPanel());
            panel.AddPanel(new DetailViewPanel_Pawn(pawn, StartShow, EndShow));
        }

        protected override void UpdateShow(DetailViewPanel panel)
        {
            if (panel == null) return;
            if (pawn == null) return;
            content.Clear();

            if(pawn.Info.IsDead)
            {
                content.Add("死亡");
                panel.SetView(
                    pawn.name,
                    content
                    );
                return;
            }

            if (pawn.StateMachine.CurState != null)
            {
                content.Add("当前工作：" + pawn.StateMachine.CurState.Description);
            }         
            if (pawn.StateMachine.NextState != null)
            {
                content.Add("下一个工作" + pawn.StateMachine.NextState.Description);
            }
            foreach (var state in pawn.StateMachine.StateQueue)
            {
                content.Add("准备" + state.Description);
            }
            panel.SetView(
                pawn.name,
                content
                );
        }
    }
}