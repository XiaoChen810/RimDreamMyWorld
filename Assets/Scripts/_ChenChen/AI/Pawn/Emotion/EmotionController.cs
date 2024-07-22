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
        /// ���п��ܵ������б�
        /// </summary>
        public EmotionList EmotionsList;

        private List<Emotion> curEmotionsList = new();
        private Emotion curEmotion;

        private SpriteRenderer sr;
        private Dictionary<EmotionType, Emotion> emotionsDict = new();

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

        //�ֲ�����, ������ˣ���sr.sprite == null
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
        /// ���һ���������������Ƿ���ӳɹ���
        /// ������и����������ٴ���ӡ�
        /// </summary>
        /// <param name="emotionType">Ҫ��ӵ��������͡�</param>
        /// <returns>����ɹ���ӷ��� true����������Ѵ��ڷ��� false��</returns>   
        public bool AddEmotion(EmotionType emotionType)
        {
            if (sr == null || !emotionsDict.ContainsKey(emotionType))
            {
                Debug.LogWarning("Emotion not found or SpriteRenderer not initialized.");
                return false;
            }

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
