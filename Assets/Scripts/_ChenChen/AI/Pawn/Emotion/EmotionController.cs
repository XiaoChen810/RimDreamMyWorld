using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

namespace ChenChen_AI
{
    public class EmotionController : MonoBehaviour
    {
        public EmotionList EmotionsList;

        private List<Emotion> curEmotionsList = new();
        private Emotion curEmotion;

        private Pawn pawn;
        private SpriteRenderer sr;
        private Dictionary<EmotionType, Emotion> emotionsDict = new();

        private void OnEnable()
        {
            pawn = GetComponentInParent<Pawn>();
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

        private IEnumerator EmotionCircleCo()
        {
            while (true)
            {
                if (curEmotionsList.Count == 0)
                {
                    sr.color = new Color(1, 1, 1, 0);
                }
                else
                {
                    int index = 0;
                    while(curEmotionsList.Count > 0 && index < curEmotionsList.Count)
                    {
                        sr.DOFade(0, 0.5f);
                        yield return new WaitForSeconds(1);
                        if (index < curEmotionsList.Count)
                        {
                            sr.sprite = curEmotionsList[index].icon;
                            curEmotion = curEmotionsList[index];
                            index++;
                        }
                        else
                        {
                            break;
                        }
                        sr.DOFade(1, 0.5f);
                        yield return new WaitForSeconds(1);
                    }
                }
                yield return null;
            }
        }

        private void InitializeEmotionsDict()
        {
            emotionsDict = new Dictionary<EmotionType, Emotion>();
            if (EmotionsList != null)
            {
                foreach (Emotion e in EmotionsList.list)
                {
                    if (!emotionsDict.ContainsKey(e.type))
                    {
                        emotionsDict.Add(e.type, e);
                    }
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

            if (pawn.Faction != GameManager.PLAYER_FACTION) return false;

            if (curEmotionsList.Any(e => e.type == emotionType)) return false;

            Emotion e = emotionsDict[emotionType];
            sr.sprite = e.icon;
            curEmotionsList.Add(e);
            return true;
        }

        public void RemoveEmotion()
        {
            sr.sprite = null;
            curEmotionsList.Clear();
        }

        public void RemoveEmotion(EmotionType emotionType)
        {
            if(curEmotion.type == emotionType)
            {
                sr.sprite = null;
            }
            curEmotionsList.RemoveAll(e => e.type == emotionType);
        }

        private void OnDestroy()
        {
            DOTween.Kill(sr);
        }

    }
}
