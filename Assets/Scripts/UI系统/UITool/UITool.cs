using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIϵͳ
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

            Debug.LogWarning("�Ҳ��������" + Name);
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