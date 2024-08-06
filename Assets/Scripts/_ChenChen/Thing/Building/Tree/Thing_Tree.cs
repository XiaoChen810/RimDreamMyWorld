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
        private float shakeStrength = 3f;
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

        protected override Action<DetailViewPanel> DetailViewOverrideContentAction()
        {
            return (DetailViewPanel panel) =>
            {
                List<string> content = new List<string>();
                if (panel == null) return;
                if (this == null) return;
                content.Clear();
                content.Add($"�;ö�: {this.Durability} / {this.MaxDurability}");
                if (this.UserPawn != null) content.Add($"ʹ����: {this.UserPawn.name}");
                panel.SetView(
                    this.Def.DefName,
                    content);
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
            };
        }
    }
}