using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{

    /// <summary>
    /// ���϶���
    /// </summary>
    [CreateAssetMenu(menuName = "����/����", fileName = "StuffDef", order = 3)]
    public class StuffDef : ScriptableObject
    {
        [Header("��������")]

        public string Name;

        public string Description;

        public Sprite Icon;

        [Tooltip("���Ա�����")]
        public bool CanMake;
        [Header("������Ҫ�Ĳ���")]
        public List<string> needs;
        [Header("������Ҫ�ĿƼ�")]
        public List<string> needsTechs;
    }
}