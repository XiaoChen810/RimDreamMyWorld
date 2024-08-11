using ChenChen_Core;
using ChenChen_Thing;
using System;
using System.Collections.Generic;


namespace ChenChen_UI
{
    public class DetailView_Room : DetailView
    {
        Room room;

        public void Init(Room room)
        {
            this.room = room;
        }

        public override void OpenPanel()
        {
            PanelManager panelManager = DetailViewManager.Instance.PanelManager;
            panelManager.RemovePanel(panelManager.GetTopPanel());
            panelManager.AddPanel(new DetailViewPanel_Room(StartShow, EndShow));
        }

        protected override void UpdateShow(DetailViewPanel panel)
        {
            if (panel == null) return;
            if (room == null) return;

            content.Clear();
            content.Add($"大小: {room.size}");
            panel.SetView(room.type, content);
        }
    }
}
