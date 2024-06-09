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
        [Header("必须属性")]

        public string Name;

        public string Description;

        public GameObject Prefab;

        [Header("含肉量")]
        public int Meat = 0;
        [Header("最大生命值")]
        public int MaxHealth;
        [Header("是否是水生动物")]
        public bool IsAquaticAnimals;
    }
}