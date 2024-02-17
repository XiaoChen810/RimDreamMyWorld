using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyUISystem
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

        /// <summary>
        ///  通过名字获取子物体
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public GameObject GetChildByName(string Name)
        {
            Transform child = FindChildRecursively(currentPanel.transform, Name);
            if (child != null)
            {
                return child.gameObject;
            }

            Debug.LogWarning("找不到该组件" + Name);
            return null;
        }

        private Transform FindChildRecursively(Transform parent, string childName)
        {
            Transform child = parent.Find(childName);
            if (child != null)
            {
                return child;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform foundChild = FindChildRecursively(parent.GetChild(i), childName);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }

            return null;
        }

        /// <summary>
        ///  通过名字获取一个子物体的组件，例如获取<see href="Button"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="componentName"></param>
        /// <returns></returns>
        public T GetOrAddChildComponent<T>(string componentName) where T : Component
        {
            GameObject child = GetChildByName(componentName);
            if (child != null)
            {
                if (child.GetComponent<T>() == null) child.AddComponent<T>();

                return child.GetComponent<T>();
            }

            return null;
        }
    }
}