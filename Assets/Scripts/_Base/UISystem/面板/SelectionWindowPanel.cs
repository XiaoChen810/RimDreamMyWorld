using ChenChen_Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem {
    public class SelectionWindowPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Init/SelectionWindowPanel";
        public SelectionWindowPanel() : base(new UIType(path))
        {
            p0 = GameObject.Find("SelectionPawn0").GetComponent<Pawn>();
            p1 = GameObject.Find("SelectionPawn1").GetComponent<Pawn>();
            p2 = GameObject.Find("SelectionPawn2").GetComponent<Pawn>();
            p0.Attribute.InitPawnAttribute();
            p1.Attribute.InitPawnAttribute();
            p2.Attribute.InitPawnAttribute();
        }

        private Pawn p0;
        private Pawn p1;
        private Pawn p2;

        public override void OnEnter()
        {
            UITool.TryGetChildComponentByName<Button>("EnterGame").onClick.AddListener(() =>
            {
                SceneSystem.Instance.SetScene(new MainScene(() =>
                {
                    GameManager.Instance.GeneratePawn(new PawnKindDef("unname", "殖民地", "", null),
                        new Vector3(50, 50, 0),
                        p0.Attribute);
                    GameManager.Instance.GeneratePawn(new PawnKindDef("unname", "殖民地", "", null),
                        new Vector3(51, 51, 0),
                        p1.Attribute);
                    GameManager.Instance.GeneratePawn(new PawnKindDef("unname", "殖民地", "", null),
                        new Vector3(49, 49, 0),
                        p2.Attribute);
                }));
            });
            UITool.TryGetChildComponentByPath<Button>("PawnBox0/Refresh").onClick.AddListener(() =>
            {
                p0.Attribute.InitPawnAttribute();
                UpdateAttributeValue(0, p0);
            });
            UITool.TryGetChildComponentByPath<Button>("PawnBox1/Refresh").onClick.AddListener(() =>
            {
                p1.Attribute.InitPawnAttribute();
                UpdateAttributeValue(1, p1);
            });
            UITool.TryGetChildComponentByPath<Button>("PawnBox2/Refresh").onClick.AddListener(() =>
            {
                p2.Attribute.InitPawnAttribute();
                UpdateAttributeValue(2, p2);
            });
            UpdateAttributeValue(0, p0);
            UpdateAttributeValue(1, p1);
            UpdateAttributeValue(2, p2);
        }

        private void UpdateAttributeValue(int index,Pawn p)
        {
            string panelPath = $"PawnBox{index}/Attribute/Content/";

            UITool.TryGetChildComponentByPath<Text>(panelPath + "Combat").text =
                "战斗能力：" + p.Attribute.A_Combat.Value.ToString();
            UITool.TryGetChildComponentByPath<Text>(panelPath + "Culinary").text =
                "烹饪能力：" + p.Attribute.A_Culinary.Value.ToString();
            UITool.TryGetChildComponentByPath<Text>(panelPath + "Construction").text =
                "建造能力：" + p.Attribute.A_Construction.Value.ToString();
            UITool.TryGetChildComponentByPath<Text>(panelPath + "Survival").text =
                "生存能力：" + p.Attribute.A_Survival.Value.ToString();
            UITool.TryGetChildComponentByPath<Text>(panelPath + "Craftsmanship").text =
                "工艺能力：" + p.Attribute.A_Craftsmanship.Value.ToString();
            UITool.TryGetChildComponentByPath<Text>(panelPath + "Medical").text =
                "医护能力：" + p.Attribute.A_Medical.Value.ToString();
            UITool.TryGetChildComponentByPath<Text>(panelPath + "Carrying").text =
                "搬运能力：" + p.Attribute.A_Carrying.Value.ToString();
            UITool.TryGetChildComponentByPath<Text>(panelPath + "Research").text =
                "科研能力：" + p.Attribute.A_Research.Value.ToString();
        }
    }
}