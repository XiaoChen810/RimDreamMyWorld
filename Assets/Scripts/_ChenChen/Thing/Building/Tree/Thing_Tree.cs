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
                // 创建新的粒子系统物体
                if (particleEffect_WhenCut != null)
                {
                    // 在当前位置和旋转下实例化粒子系统预制体
                    ParticleSystem newParticleEffect = Instantiate(particleEffect_WhenCut, transform.position, transform.rotation);
                    newParticleEffect.Play();

                    // 销毁粒子系统物体以节省资源（延迟销毁，确保播放完毕）
                    Destroy(newParticleEffect.gameObject, newParticleEffect.main.duration + newParticleEffect.main.startLifetime.constantMax);
                }

                // 销毁游戏对象
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
                panel.SetButton("取消", () =>
                {
                    // 取消砍伐
                    this.OnCanclCut();
                });
            }
            else
            {
                panel.RemoveAllButton();
                panel.SetButton("砍伐", () =>
                {
                    // 标记砍伐
                    this.OnMarkCut();
                });
            }
        }
    }
}