using ChenChen_Thing;
using ChenChen_UI;
using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;

namespace ChenChen_Thing
{
    [RequireComponent(typeof(Collider2D))]
    public class Thing_Tree : Building, ICut
    {
        #region - Cut -
        [Header("Cut")]
        private float shakeDuraion = 0.5f;
        private float shakeStrength = 5f;
        private bool isMarkCut;
        [SerializeField] private ParticleSystem particleEffect_WhenCut;
        [SerializeField] private GameObject markIcon;

        public bool IsMarkCut
        {
            get
            {
                return isMarkCut;
            }
            set
            {
                isMarkCut = value;  
                markIcon.SetActive(value);
            }
        }

        public void OnMarkCut()
        {
            IsMarkCut = true;
        }

        public void OnCut(int value)
        {
            Durability -= value;

            transform.DOShakeRotation(shakeDuraion, shakeStrength);

            if (Durability <= 0)
            {
                // �����µ�����ϵͳ����
                if (particleEffect_WhenCut != null)
                {
                    // �ڵ�ǰλ�ú���ת��ʵ��������ϵͳԤ����
                    ParticleSystem newParticleEffect = Instantiate(particleEffect_WhenCut, transform.position, transform.rotation);
                    newParticleEffect.Play();

                    // ��������ϵͳ�����Խ�ʡ��Դ���ӳ����٣�ȷ��������ϣ�
                    Destroy(newParticleEffect.gameObject, newParticleEffect.main.duration + newParticleEffect.main.startLifetime.constantMax);
                }

                // ������Ϸ����
                Destroy(gameObject);
            }
        }

        public void OnCanclCut()
        {
            IsMarkCut = false;
        }

        #endregion

        protected override void Start()
        {
            base.Start();
            DestroyOutputs.Add(("wood", 3));
        }

        private void OnTriggerStay2D(Collider2D collision)
        {          
            if (collision.CompareTag("Pawn"))
            {
                SR.color = new Color(1, 1, 1, 0.5f);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Pawn"))
            {
                SR.color = new Color(1, 1, 1, 1);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DOTween.KillAll();
        }

        protected override void DetailViewOverrideContentAction(DetailViewPanel panel)
        {
            base.DetailViewOverrideContentAction(panel);
            if (this.IsMarkCut)
            {
                panel.RemoveAllButton();
                panel.SetButton("ȡ��", () =>
                {
                    // ȡ������
                    this.OnCanclCut();
                });
            }
            else
            {
                panel.RemoveAllButton();
                panel.SetButton("����", () =>
                {
                    // ��ǿ���
                    this.OnMarkCut();
                });
            }
        }
    }
}