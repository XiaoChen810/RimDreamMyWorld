using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{

    /// <summary>
    /// 材料定义
    /// </summary>
    [CreateAssetMenu(menuName = "定义/材料", fileName = "StuffDef", order = 3)]
    public class StuffDef : ScriptableObject
    {
        [Header("必须属性")]

        public string Name;

        public string Description;

        public Sprite Icon;

        [Tooltip("可以被制作")]
        public bool CanMake;
        [Header("制作需要的材料")]
        public List<string> needs;
        [Header("制作需要的科技")]
        public List<string> needsTechs;
    }
}