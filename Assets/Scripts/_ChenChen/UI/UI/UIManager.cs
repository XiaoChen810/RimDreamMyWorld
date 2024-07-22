using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UI
{
    /// <summary>
    /// 管理UI的创建与销毁
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
        /// 获取单个的一个UI对象
        /// </summary>
        /// <param name="type"> UI对象的类型数据 </param>
        /// <returns></returns>
        public GameObject GetOrGenerateSingleUI(UIType type)
        {
            if(canvas == null)
            {
                canvas = GameObject.Find("Canvas");
                if (canvas == null)
                {
                    Debug.LogError("画布对象不存在");
                    return null;
                }
            }

            if(UIDict.ContainsKey(type)) return UIDict[type];

            // 如果不存在则生成一个
            GameObject newUI = GameObject.Instantiate(Resources.Load<GameObject>(type.Path), canvas.transform);
            newUI.name = type.Name;
            UIDict.Add(type, newUI);
            return newUI;
        }

        /// <summary>
        /// 删除一个UI对象
        /// </summary>
        /// <param name="type">UI对象的类型数据</param>
        public void DestroyUI(UIType type)
        {
            if (UIDict.ContainsKey(type))
            {
                GameObject.Destroy(UIDict[type]);
                UIDict.Remove(type);
            }
            else
            {
                Debug.LogWarning("未能找到此UI" +  type.Name);
            }
        }
    }
}