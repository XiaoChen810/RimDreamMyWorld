using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class PawnListPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/PawnListPanel";

        public PawnListPanel() : base(new UIType(path))
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

            string pawnInfomationPrefabPath = "UI/Component/PawnInfomation";
            GameObject pawnInfomationPrefab = Resources.Load(pawnInfomationPrefabPath) as GameObject;
            if (pawnInfomationPrefab == null)
            {
                Debug.LogError("预制件为空, 检查位置: " + pawnInfomationPrefabPath);
                PanelManager.RemovePanel(this);
                return;
            }

            foreach (var pawn in GameManager.Instance.PawnGeneratorTool.PawnsList)
            {
                GameObject pawnInfomation = Object.Instantiate(pawnInfomationPrefab);
                pawnInfomation.name = $"BtnBlueprint{pawn.name}";
                pawnInfomation.transform.Find("NameText").GetComponent<Text>().text = pawn.name;
                pawnInfomation.transform.SetParent(content.transform, false);
                ObjectPtr pawnPtr = pawnInfomation.GetComponent<ObjectPtr>();
                pawnPtr.Init(new TargetPtr(pawn.gameObject));
            }
        }
    }
}
