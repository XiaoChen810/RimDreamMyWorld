using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public class BuildingMenuPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/BuildingMenuPanel";

        public BuildingMenuPanel(Callback onEnter, Callback onExit) : base(new UIType(path), onEnter, onExit) { }

        /// <summary>
        /// ��Ӹ��ֲ˵���ť�ļ�������
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            // ѡ����ǽ�����͵İ�ť
            UITool.TryGetChildComponentByName<Button>("Btnǽ��").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_BuildingSystem.ThingType.Wall));
            });
            // ѡ����ذ����͵İ�ť
            UITool.TryGetChildComponentByName<Button>("Btn�ذ�").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_BuildingSystem.ThingType.Floor));
            });
            // ѡ�����������͵İ�ť
            UITool.TryGetChildComponentByName<Button>("Btn����").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_BuildingSystem.ThingType.Other));
            });
            // ѡ����Ҿ����͵İ�ť
            UITool.TryGetChildComponentByName<Button>("Btn�Ҿ�").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_BuildingSystem.ThingType.Furniture));
            });
            // �رղ˵��İ�ť
            UITool.TryGetChildComponentByName<Button>("Btn�ر�").onClick.AddListener(() =>
            {
                PanelManager.RemovePanel(this);
            });

        }
    }
}
