using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI系统
{
    /// <summary>
    ///  管理UI面板里面的组件工具
    /// </summary>
    public class UITool
    {
        // 当前的面板
        GameObject currentPanel;

        public UITool(GameObject currentPanel)
        {
            this.currentPanel = currentPanel;
        }

        /// <summary>
        /// 获取或者添加组件
        /// </summary>
        /// <typeparam name="T">组件类型，例如Button</typeparam>
        /// <returns></returns>
        public T GetOrAddComponent<T>() where T : Component
        {
            if (currentPanel.GetComponent<T>() == null) currentPanel.AddComponent<T>();

            return currentPanel.GetComponent<T>();
        }

        public GameObject GetChildByName(string Name)
        {          
            Transform[] childs = currentPanel.GetComponentsInChildren<Transform>();

            foreach (Transform child in childs)
            {
                if (child.name == Name)
                {
                    return child.gameObject;
                }
            }

            Debug.LogWarning("找不到该组件" + Name);
            return null;
        }

        public T GetOrAddChildComponent<T>(string componentName) where T : Component
        {
            GameObject child = GetChildByName(componentName);
            if(child != null)
            {
                if(child.GetComponent<T>() == null) child.AddComponent<T>();

                return child.GetComponent<T>();
            }

            return null;
        }
    }
}