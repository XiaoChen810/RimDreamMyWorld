using ChenChen_AI;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace ChenChen_UI
{
    public class DetailView_Animal : DetailView
    {
        [SerializeField] protected Animal animal;

        private void OnEnable()
        {
            animal = GetComponent<Animal>();
        }

        public override void OpenPanel()
        {
            PanelManager panel = DetailViewManager.Instance.PanelManager;
            panel.RemovePanel(panel.GetTopPanel());
            panel.AddPanel(new DetailViewPanel_Animal(animal, StartShow, EndShow));
        }

        protected override void UpdateShow(DetailViewPanel panel)
        {
            if (panel == null) return;
            if (animal == null) return;
            content.Clear();
            content.Add(animal.Def.Description);
            panel.SetView(
                animal.name,
                content
                );
            if(!animal.Info.IsTrade && !animal.Info.IsFlagTrade)
            {
                panel.RemoveAllButton();
                panel.SetButton("驯服", onClick: () =>
                {
                    animal.FlagTrade();
                });
            }
            if (!animal.Info.IsTrade && animal.Info.IsFlagTrade)
            {
                panel.RemoveAllButton();
                panel.SetButton("取消驯服", onClick: () =>
                {
                    animal.CancelTrade();
                });
            }
            if (animal.Info.IsTrade)
            {
                panel.RemoveAllButton();
            }
        }
    }
}
