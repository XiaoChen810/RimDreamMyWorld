using ChenChen_AI;
using ChenChen_MapGenerator;
using ChenChen_Scene;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public class SelectionWindowPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Scene/SelectionWindowPanel";
        private GameManager gameManager;
        private Pawn p0;
        private Pawn p1;
        private Pawn p2;
        //private Vector3 vp1 = new Vector3(-5, 1.3f, 0);
        //private Vector3 vp2 = new Vector3(0, 1.3f, 0);
        //private Vector3 vp3 = new Vector3(5, 1.3f, 0);
        public SelectionWindowPanel() : base(new UIType(path))
        {
            gameManager = GameManager.Instance;
            gameManager.PawnGeneratorTool.StartSelect();
            p0 = gameManager.PawnGeneratorTool.PawnWhenStartList[0];
            p1 = gameManager.PawnGeneratorTool.PawnWhenStartList[1];
            p2 = gameManager.PawnGeneratorTool.PawnWhenStartList[2];
        }

        public override void OnEnter()
        {
            UITool.TryGetChildComponentByName<Button>("EnterGame").onClick.AddListener(() =>
            {
                Action onPreloadAnimation = () =>
                {
                    p0.gameObject.SetActive(false);
                    p1.gameObject.SetActive(false);
                    p2.gameObject.SetActive(false);

                    Debug.Log("EnterGame");
                };
                Action onPostLoadScene = () =>
                {
                    // 生成一个新场景
                    MapManager.Instance.LoadOrGenerateSceneMap(StaticDef.Map_Default_Name);
                    // 生成选择的棋子
                    GenerateSelectedPawn(p0);
                    GenerateSelectedPawn(p1);
                    GenerateSelectedPawn(p2);

                    void GenerateSelectedPawn(Pawn select)
                    {
                        select.Def.StopUpdate = false;
                        _ = GameManager.Instance.PawnGeneratorTool.GeneratePawn(new Vector3(UnityEngine.Random.Range(45, 55), UnityEngine.Random.Range(45, 55), 0),
                                                          select.Def,
                                                          select.Info,
                                                          select.Attribute);
                    }

                    gameManager.PawnGeneratorTool.EndSelect();
                };
                SceneSystem.Instance.SetScene(new MainScene(onPreloadAnimation, onPostLoadScene, 1f));
            });
            UITool.TryGetChildComponentByPath<Button>("PawnBox0/Refresh").onClick.AddListener(() =>
            {
                p0.Attribute = new PawnAttribute();
                UpdateAttributeValue(0, p0);
            });
            UITool.TryGetChildComponentByPath<Button>("PawnBox1/Refresh").onClick.AddListener(() =>
            {
                p1.Attribute = new PawnAttribute();
                UpdateAttributeValue(1, p1);
            });
            UITool.TryGetChildComponentByPath<Button>("PawnBox2/Refresh").onClick.AddListener(() =>
            {
                p2.Attribute = new PawnAttribute();
                UpdateAttributeValue(2, p2);
            });
            UpdateAttributeValue(0, p0);
            UpdateAttributeValue(1, p1);
            UpdateAttributeValue(2, p2);
            UITool.TryGetChildComponentByPath<InputField>("PawnBox0/Name").onValueChanged.AddListener((string content) =>
            {
                p0.Def.PawnName = content;
            });
            UITool.TryGetChildComponentByPath<InputField>("PawnBox1/Name").onValueChanged.AddListener((string content) =>
            {
                p1.Def.PawnName = content;
            });
            UITool.TryGetChildComponentByPath<InputField>("PawnBox2/Name").onValueChanged.AddListener((string content) =>
            {
                p2.Def.PawnName = content;
            });
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