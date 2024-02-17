using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyUISystem
{
    /// <summary>
    ///  ����UI���������������
    /// </summary>
    public class UITool
    {
        // ��ǰ�����
        GameObject currentPanel;

        public UITool(GameObject currentPanel)
        {
            this.currentPanel = currentPanel;
        }

        /// <summary>
        /// ��ȡ����������
        /// </summary>
        /// <typeparam name="T">������ͣ�����Button</typeparam>
        /// <returns></returns>
        public T GetOrAddComponent<T>() where T : Component
        {
            if (currentPanel.GetComponent<T>() == null) currentPanel.AddComponent<T>();

            return currentPanel.GetComponent<T>();
        }

        /// <summary>
        ///  ͨ�����ֻ�ȡ������
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

            Debug.LogWarning("�Ҳ��������" + Name);
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
        ///  ͨ�����ֻ�ȡһ�������������������ȡ<see href="Button"/>
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