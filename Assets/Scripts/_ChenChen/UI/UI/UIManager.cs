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
        public Dictionary<UIType, GameObject> UIDict = new Dictionary<UIType, GameObject>();

        public UIManager() 
        {
            UIDict = new();
        }

        private GameObject canvas;

        private Dictionary<UIType, GameObject> cacheList = new Dictionary<UIType, GameObject>();

        /// <summary>
        /// ��ȡ������һ��UI����
        /// </summary>
        /// <param name="type"> UI������������� </param>
        /// <returns></returns>
        public GameObject GetOrGenerateSingleUI(UIType type)
        {
            if (canvas == null)
            {
                canvas = GameObject.Find("Canvas");
                if (canvas == null)
                {
                    Debug.LogError("�������󲻴���");
                    return null;
                }
            }

            if (UIDict.ContainsKey(type)) return UIDict[type];

            GameObject prefab = null;
            if (!cacheList.TryGetValue(type, out prefab))
            {
                prefab = Resources.Load<GameObject>(type.Path);
                cacheList.Add(type, prefab);
            }
            GameObject newUI = GameObject.Instantiate(prefab, canvas.transform);
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