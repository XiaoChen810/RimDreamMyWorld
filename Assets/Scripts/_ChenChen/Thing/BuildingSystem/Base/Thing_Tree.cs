using ChenChen_Thing;
using ChenChen_UI;
using UnityEngine;

namespace ChenChen_Thing
{
    [RequireComponent(typeof(Collider2D))]
    public class Thing_Tree : ThingBase, ICut
    {
        private Collider2D coll;
        private SpriteRenderer sr;

        public bool IsMarkCut;

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

        public ParticleSystem particleEffect_WhenCut;

        private void Start()
        {
            coll = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();
            coll.isTrigger = true;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Pawn"))
            {
                sr.color = new Color(1, 1, 1, 0.5f);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Pawn"))
            {
                sr.color = new Color(1, 1, 1, 1);
            }
        }

        public override void OnPlaced(BuildingLifeStateType initial_State, string mapName)
        {
            ChangeLifeState(initial_State);
            CurDurability = MaxDurability;
            MapName = mapName;
        }

        public void OnMarkCut()
        {
            IsMarkCut = true;
        }

        public void OnCut(int value)
        {
            CurDurability -= value;
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
    }
}