using ChenChen_Thing;
using UnityEngine;

namespace ChenChen_UI
{
    public class DetailView_Tree : DetailView
    {
        [SerializeField] protected Thing_Tree tree;
        private void OnEnable()
        {
            tree = GetComponent<Thing_Tree>();
        }

        public override void OpenPanel()
        {
            PanelManager panelManager = DetailViewManager.Instance.PanelManager;
            panelManager.RemoveTopPanel(panelManager.GetTopPanel());
            panelManager.AddPanel(new DetailViewPanel_Tree(tree, StartShow, EndShow));
        }

        protected override void UpdateShow(DetailViewPanel panel)
        {
            if (panel == null) return;
            if (tree == null) return;
            Content.Clear();
            Content.Add($"耐久度: {tree.CurDurability} / {tree.MaxDurability}");
            panel.SetView(
                tree.Def.DefName,
                Content);
            if (tree.IsMarkCut)
            {
                panel.RemoveAllButton();
                panel.SetButton("取消", () =>
                {
                    // 取消砍伐
                    tree.OnCanclCut();
                });
            }
            else
            {
                panel.RemoveAllButton();
                panel.SetButton("砍伐", () =>
                {
                    // 标记砍伐
                    tree.OnMarkCut();
                });
            }
        }


    }
}
