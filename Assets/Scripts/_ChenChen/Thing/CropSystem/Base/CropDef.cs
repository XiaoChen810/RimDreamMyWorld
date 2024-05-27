using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{

    /// <summary>
    /// ���ﶨ��
    /// </summary>
    [CreateAssetMenu(menuName = "����/����", fileName = "CropDef", order = 0)]
    public class CropDef : ScriptableObject
    {
        [Header("����")]
        public string CropName;
        [Header("����")]
        public string CropDescription;
        [Header("ͼƬ")]
        public Sprite CropIcon;
        [Header("���������һ������Ӫ��ֵ")]
        public float GroupNutrientRequiries;
        [Header("�����ٶȣ�ÿ�����Ӷ���Ӫ��ֵ")]
        public float GroupSpeed;
        [Header("���н׶ε�ͼ��")]
        public List<Sprite> CropsSprites;
    }
}