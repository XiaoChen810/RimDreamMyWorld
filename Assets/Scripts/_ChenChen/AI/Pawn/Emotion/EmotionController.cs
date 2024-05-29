using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

namespace ChenChen_AI
{
    public class EmotionController : MonoBehaviour
    {
        /// <summary>
        /// 所有可能的情绪列表
        /// </summary>
        public EmotionList EmotionsList;

        public List<Emotion> CurEmotions = new();

        private SpriteRenderer sr;
        private Dictionary<EmotionType, Emotion> emotionsDict;

        private void OnEnable()
        {
            sr = GetComponent<SpriteRenderer>();
            InitializeEmotionsDict();
            StartCoroutine(EmotionCircleCo());
        }

        private void Update()
        {
            if (transform.parent.localScale.x > 0)
            {
                transform.localScale = Vector3.one;
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        //轮播情绪, 如果空了，则sr.sprite == null
        private IEnumerator EmotionCircleCo()
        {
            while (true)
            {
                if (CurEmotions.Count == 0)
                {
                    sr.sprite = null;
                }
                else
                {
                    var emotionsCopy = CurEmotions.ToList();
                    foreach (var emotion in emotionsCopy)
                    {
                        if (sr != null)
                        {
                            sr.DOFade(0, 0.9f);
                            yield return new WaitForSeconds(1);
                            sr.sprite = emotion.icon;
                            sr.DOFade(1, 0.9f);
                            yield return new WaitForSeconds(1);
                        }
                    }
                }
                yield return null;
            }
        }

        private void InitializeEmotionsDict()
        {
            emotionsDict = new Dictionary<EmotionType, Emotion>();
            foreach (Emotion e in EmotionsList.list)
            {
                if (!emotionsDict.ContainsKey(e.type))
                {
                    emotionsDict.Add(e.type, e);
                }
            }
        }

        /// <summary>
        /// 添加一个情绪，并返回是否添加成功。
        /// 如果已有该情绪不会再次添加。
        /// </summary>
        /// <param name="emotionType">要添加的情绪类型。</param>
        /// <returns>如果成功添加返回 true；如果情绪已存在返回 false。</returns>   
        public bool AddEmotion(EmotionType emotionType)
        {
            if (sr == null || !emotionsDict.ContainsKey(emotionType))
            {
                Debug.LogWarning("Emotion not found or SpriteRenderer not initialized.");
                return false;
            }

            if (CurEmotions.Any(e => e.type == emotionType)) return false;

            Emotion e = emotionsDict[emotionType];
            sr.sprite = e.icon;
            CurEmotions.Add(e);
            return true;
        }

        public void RemoveEmotion()
        {
            CurEmotions.Clear();
        }

        public void RemoveEmotion(EmotionType emotionType)
        {
            CurEmotions.RemoveAll(e => e.type == emotionType);
        }

        private void OnDestroy()
        {
            DOTween.Kill(sr);
        }

    }
}
