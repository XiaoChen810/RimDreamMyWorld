using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{

    /// <summary>
    /// 作物定义
    /// </summary>
    [CreateAssetMenu(menuName = "定义/作物", fileName = "CropDef", order = 0)]
    public class CropDef : ScriptableObject
    {
        [Header("名字")]
        public string CropName;
        [Header("描述")]
        public string CropDescription;
        [Header("图片")]
        public Sprite CropIcon;
        [Header("生长到最后一共所需营养值")]
        public float GroupNutrientRequiries;
        [Header("生长速度，每天增加多少营养值")]
        public float GroupSpeed;
        [Header("所有阶段的图像")]
        public List<Sprite> CropsSprites;
    }
}