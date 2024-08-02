using ChenChen_Thing;
using ChenChen_UI;
using UnityEngine;
using DG.Tweening;

namespace ChenChen_Thing
{
    [RequireComponent(typeof(Collider2D))]
    public class Thing_Tree : Thing, ICut
    {  
        private float shakeDuraion = 0.5f;
        private float shakeStrength = 3f;

        public bool IsMarkCut;
        public ParticleSystem particleEffect_WhenCut;

        public override DetailView DetailView
        {
            get
            {
                if (_detailView == null)
                {
                    if (!TryGetComponent<DetailView>(out _detailView))
                    {
                        _detailView = gameObject.AddComponent<DetailView_Tree>();
                    }
                }
                return _detailView;
            }
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

        public void OnMarkCut()
        {
            IsMarkCut = true;
        }

        public void OnCut(int value)
        {
            CurDurability -= value;

            transform.DOShakeRotation(shakeDuraion, shakeStrength);

            if (CurDurability <= 0)
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

        protected override void OnDestroy()
        {
            DOTween.KillAll();
            base.OnDestroy();
        }
    }
}