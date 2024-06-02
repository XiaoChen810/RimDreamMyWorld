using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    [CreateAssetMenu(fileName = "EmotionList", menuName = "情绪列表")]
    public class EmotionList : ScriptableObject
    {
        public List<Emotion> list;
    }

}