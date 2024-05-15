using System;
using UnityEngine;

namespace ChenChen_AI
{
    /// <summary>
    /// 动物定义
    /// </summary>
    [CreateAssetMenu(menuName = "定义/动物", fileName = "AnimalDef", order = 2)]
    public class AnimalDef : ScriptableObject
    {
        [Header("名字")]
        public string Name;
        [Header("描述")]
        public string Description;
        [Header("含肉量")]
        public int Meat;
        [Header("最大生命值")]
        public int MaxHealth;
    }
}