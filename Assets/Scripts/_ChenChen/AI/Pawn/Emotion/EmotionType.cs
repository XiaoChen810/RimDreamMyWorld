using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    [System.Serializable]
    public struct Emotion
    {
        public EmotionType type;
        public Sprite icon;
    }

    public enum EmotionType
    {
        alerted ,
        angry,
        attack,
        chat,
        confused,
        distressed,
        love,
        working,
        happiness_1,
        happiness_2,
        happiness_3,
        happiness_4
    }

    [CreateAssetMenu(fileName = "ÇéÐ÷ÁÐ±í", menuName = "EmotionList")]
    public class EmotionList : ScriptableObject
    {
        public List<Emotion> list;
    }
}