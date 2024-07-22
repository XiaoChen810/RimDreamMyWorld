using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UI
{
    /// <summary>
    /// ����UI�Ĵ���������
    /// </summary>
    public class UIManager
    {
        public Dictionary<UIType, GameObject> UIDict;

        public UIManager() 
        {
            UIDict = new();
        }

        private GameObject canvas;

        /// <summary>
        /// ��ȡ������һ��UI����
        /// </summary>
        /// <param name="type"> UI������������� </param>
        /// <returns></returns>
        public GameObject GetOrGenerateSingleUI(UIType type)
        {
            if(canvas == null)
            {
                canvas = GameObject.Find("Canvas");
                if (canvas == null)
                {
                    Debug.LogError("�������󲻴���");
                    return null;
                }
            }

            if(UIDict.ContainsKey(type)) return UIDict[type];

            // ���������������һ��
            GameObject newUI = GameObject.Instantiate(Resources.Load<GameObject>(type.Path), canvas.transform);
            newUI.name = type.Name;
            UIDict.Add(type, newUI);
            return newUI;
        }

        /// <summary>
        /// ɾ��һ��UI����
        /// </summary>
        /// <param name="type">UI�������������</param>
        public void DestroyUI(UIType type)
        {
            if (UIDict.ContainsKey(type))
            {
                GameObject.Destroy(UIDict[type]);
                UIDict.Remove(type);
            }
            else
            {
                Debug.LogWarning("δ���ҵ���UI" +  type.Name);
            }
        }
    }
}