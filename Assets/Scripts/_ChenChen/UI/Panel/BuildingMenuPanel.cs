using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class BuildingMenuPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/BuildingMenuPanel";

        public BuildingMenuPanel() : base(new UIType(path)) { }

        /// <summary>
        /// ��Ӹ��ֲ˵���ť�ļ�������
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            // ѡ����ǽ�����͵İ�ť
            UITool.TryGetChildComponentByName<Button>("Btnǽ��").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_Thing.ThingType.Wall));
            });
            // ѡ����ذ����͵İ�ť
            UITool.TryGetChildComponentByName<Button>("Btn�ذ�").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_Thing.ThingType.Floor));
            });
            // ѡ�����������͵İ�ť
            UITool.TryGetChildComponentByName<Button>("Btn����").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_Thing.ThingType.Other));
            });
            // ѡ����Ҿ����͵İ�ť
            UITool.TryGetChildComponentByName<Button>("Btn�Ҿ�").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_Thing.ThingType.Furniture));
            });
            // ѡ���칤��̨���͵İ�ť
            UITool.TryGetChildComponentByName<Button>("Btn����̨").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_Thing.ThingType.ToolTable));
            });
            // ѡ���콨�����͵İ�ť
            UITool.TryGetChildComponentByName<Button>("Btn����").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_Thing.ThingType.Building));
            });
            // �رղ˵��İ�ť
            UITool.TryGetChildComponentByName<Button>("Btn�ر�").onClick.AddListener(() =>
            {
                PanelManager.RemoveTopPanel(this);
            });

        }
    }
}
