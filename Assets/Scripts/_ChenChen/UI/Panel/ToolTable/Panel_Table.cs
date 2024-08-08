using ChenChen_Core;
using ChenChen_Thing;
using Pathfinding.RVO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class Panel_Table : PanelBase
    {
        private static readonly string path = "UI/TableMenu/TablePanel";
        private static readonly string path_info = "UI/TableMenu/Info/MadeInfo";

        private readonly Thing_Tool table;

        public Panel_Table(Thing_Tool table, Callback onExit) : base(new UIType(path), null, onExit)
        {
            this.table = table;
        }

        public override void OnEnter()
        {
            InitList();

            UITool.TryGetChildComponentByName<Button>("CancelDoing").onClick.AddListener(() =>
            {
                table.CancelCurrentWork();
                UITool.GetChildByName("CurrentDoing").SetActive(false);
            });

            UITool.TryGetChildComponentByName<Button>("PlusDemand").onClick.AddListener(() =>
            {
                table.NumberOfWaitingToMade++;
                UITool.TryGetChildComponentByName<Text>("NumberOfWaitingToMade").text = table.NumberOfWaitingToMade.ToString();
            });

            UITool.TryGetChildComponentByName<Button>("ReduceDemand").onClick.AddListener(() =>
            {
                table.NumberOfWaitingToMade--;
                UITool.TryGetChildComponentByName<Text>("NumberOfWaitingToMade").text = table.NumberOfWaitingToMade.ToString();
            });

            UITool.TryGetChildComponentByName<Button>("Btn关闭").onClick.AddListener(() =>
            {
                PanelManager.RemovePanel(this);
            });
          
            if (table.NumberOfWaitingToMade != 0)
            {
                UITool.TryGetChildComponentByPath<Image>("CurrentDoing/iconBG/Icon").sprite = table.CurrentWorkSprite;
                UITool.TryGetChildComponentByName<Text>("NumberOfWaitingToMade").text = table.NumberOfWaitingToMade.ToString();
                UITool.GetChildByName("CurrentDoing").SetActive(true);
            }
            else
            {
                UITool.GetChildByName("CurrentDoing").SetActive(false);
            }
        }

        private void InitList()
        {    
            Transform content = UITool.GetChildByName("Content").transform;
            GameObject infoPrefab = Resources.Load(path_info) as GameObject;
            var defs = table.AllCanDo;

            foreach (var def in defs)
            {
                var obj = GameObject.Instantiate(infoPrefab);
                obj.transform.SetParent(content);

                var info = obj.GetComponent<MadeInfo>();
                var cost = def.requiredMaterials;
                string costContent = string.Empty;
                foreach (var mat in cost)
                {
                    costContent += mat.ToString() + ", ";
                }
                info.Set(def.name, costContent, def.sprite);

                info.onClick.AddListener(() =>
                {
                    SetCurrnetDoing(def);
                });
            }
        }

        // 设置当前任务
        private void SetCurrnetDoing(StuffDef def)
        {
            table.CurrentMadeDef = def;
            table.NumberOfWaitingToMade = 1;
            UITool.TryGetChildComponentByPath<Image>("CurrentDoing/iconBG/Icon").sprite = def.sprite;
            UITool.TryGetChildComponentByName<Text>("NumberOfWaitingToMade").text = table.NumberOfWaitingToMade.ToString();
            UITool.GetChildByName("CurrentDoing").SetActive(true);
        }
    }
}
