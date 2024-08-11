using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using ChenChen_AI;
using System;


namespace ChenChen_Core
{
    public class RightCommandSystem : MonoBehaviour
    {
        [SerializeField] private Pawn selectPawn;
        [SerializeField] private GameObject menu;
        [SerializeField] private GameObject content;
        [SerializeField] private GameObject button;

        private RectTransform menuRectTransform;

        private List<string> colliderList = new List<string>();

        private void Start()
        {
            menuRectTransform = menu.GetComponent<RectTransform>();
        }

        private void Update()
        {
            InputUpdate();

            SelectPawnUpdate();

            Vector2 mousePositon = Input.mousePosition;
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePositon);
            Collider2D[] colliders = Physics2D.OverlapPointAll(worldPosition);
            colliderList.Clear();
            foreach (Collider2D collider in colliders)
            {
                colliderList.Add(collider.gameObject.name);
            }
            // 将colliderList内容打印在屏幕左下角
        }

        private void OnGUI()
        {
            // 创建一个新的GUIStyle
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;  // 设置字体大小
            style.normal.textColor = Color.white;  // 设置字体颜色

            // 定义显示文本的位置和大小
            Rect rect = new Rect(10, Screen.height - 100, 300, 100);
            string collidersText = "Colliders: \n" + string.Join("\n", colliderList);

            // 显示文本
            GUI.Label(rect, collidersText, style);
        }

        private void SelectPawnUpdate()
        {
            var colonyPawns = GameManager.Instance.PawnGeneratorTool.PawnList_Colony;
            int count = 0;
            foreach (var colony in colonyPawns)
            {
                if (colony.Info.IsSelect)
                {
                    count++;
                }
            }
            if (count == 1)
            {
                foreach (var colony in colonyPawns)
                {
                    if (colony.Info.IsSelect)
                    {
                        selectPawn = colony;
                        break;
                    }
                }
            }
            else
            {
                selectPawn = null;
            }
        }

        private void InputUpdate()
        {
            if (Input.GetMouseButtonDown(1) && selectPawn != null)
            {
                SetMenuCommand();

                if (content.transform.childCount != 0)
                {
                    SetMenuPosition();
                    menu.SetActive(true);
                }            
            }

            if (Input.GetMouseButtonUp(0) && menu.activeSelf)
            {
                CloseMenu(); 
            }
        }

        private void SetMenuCommand()
        {
            foreach(Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }
           
            Vector2 mousePosition = Input.mousePosition;
            Collider2D[] colliders = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(mousePosition));

            foreach(var collider in colliders)
            {
                if(collider.TryGetComponent<ICommand>(out ICommand cmd))
                {
                    SetAButton(cmd);
                }
            }
        }

        private void SetMenuPosition()
        {
            Vector2 mousePosition = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                menuRectTransform.parent as RectTransform,
                mousePosition,
                Camera.main,
                out Vector2 localPoint);

            menuRectTransform.localPosition = localPoint;        
        }

        private void SetAButton(ICommand command)
        {
            Button btn = Instantiate(button, content.transform).GetComponent<Button>();
            btn.GetComponentInChildren<Text>().text = command.CommandName;
            btn.onClick.AddListener(() =>
            {
                command.CommandFunc(selectPawn);
                CloseMenu();
            });
        }

        private void CloseMenu()
        {
            menu.SetActive(false);
        }
    }
}
