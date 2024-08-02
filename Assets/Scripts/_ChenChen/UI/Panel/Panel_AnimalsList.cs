using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class Panel_AnimalsList : PanelBase
    {
        private static readonly string path = "UI/Panel/Menus/AnimalsListPanel";
        private static readonly string animalInfomationPrefabPath = "UI/Component/AnimalInfomation";

        public Panel_AnimalsList() : base(new UIType(path))
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            InitContent();
        }

        private void InitContent()
        {
            GameObject content = UITool.GetChildByName("Content");
            foreach (Transform child in content.transform)
            {
                Object.Destroy(child);
            }

            GameObject animalInfomationPrefab = Resources.Load(animalInfomationPrefabPath) as GameObject;
            if (animalInfomationPrefab == null)
            {
                Debug.LogError("预制件为空, 检查位置: " + animalInfomationPrefabPath);
                PanelManager.RemovePanel(this);
                return;
            }

            foreach (var animal in GameManager.Instance.AnimalGenerateTool.AnimalList)
            {
                GameObject animalInfomation = Object.Instantiate(animalInfomationPrefab);
                animalInfomation.name = $"BtnBlueprint{animal.name}";
                animalInfomation.transform.Find("NameText").GetComponent<Text>().text = animal.name;
                animalInfomation.transform.SetParent(content.transform, false);
                ObjectPtr pawnPtr = animalInfomation.GetComponent<ObjectPtr>();
                pawnPtr.Init(new TargetPtr(animal.gameObject));
            }
        }
    }
}