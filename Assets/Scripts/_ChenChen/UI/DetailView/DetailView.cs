using ChenChen_Thing;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UI
{
    /// <summary>
    /// ϸ����ͼ��չʾϸ��
    /// </summary>
    public abstract class DetailView : MonoBehaviour
    {
        public bool IsPanelOpen = false;
        public bool IsIndicatorOpen = false;
        protected Indicator indicator;

        /// <summary>
        /// ��ǰ��������������
        /// </summary>
        protected List<string> content = new List<string>();

        /// <summary>
        /// ��ʾָʾ��
        /// </summary>
        public virtual void OpenIndicator()
        {
            if (indicator == null)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("Views/Indicator"), gameObject.transform);
                indicator = go.GetComponent<Indicator>();
                indicator.gameObject.name = "Indicator";
            }
            IsIndicatorOpen = true;
            indicator.gameObject.SetActive(true);
            indicator.DoAnim();

            AudioManager.Instance.PlaySFX("OpenIndicator");
        }
        /// <summary>
        /// �ر�ָʾ��
        /// </summary>
        public virtual void CloseIndicator()
        {
            if (indicator == null)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("Views/Indicator"), gameObject.transform);
                indicator = go.GetComponent<Indicator>();
                indicator.gameObject.name = "Indicator";
            }
            IsIndicatorOpen = false;
            indicator.gameObject.SetActive(false);
        }

        /// <summary>
        /// ����壬���Լ�����壬������ʽһ��ΪDetailViewPanel_[Name]��
        /// </summary>
        public abstract void OpenPanel();

        /// <summary>
        /// ��ǰ��ʾ�����������Լ��ģ�����и��²���
        /// </summary>
        /// <param name="panel"></param>
        protected abstract void UpdateShow(DetailViewPanel panel);

        public virtual void ClosePanel()
        {
            if (IsPanelOpen)
            {
                DetailViewManager.Instance.PanelManager.RemoveTopPanel();
            }
        }

        protected virtual void Update()
        {
            if (IsPanelOpen)
            {
                if (DetailViewManager.Instance.PanelManager.GetTopPanel() is DetailViewPanel detail)
                {
                    UpdateShow(detail);
                }
            }
        }

        public virtual void StartShow()
        {
            IsPanelOpen = true;

        }

        public virtual void EndShow()
        {
            IsPanelOpen = false;
            CloseIndicator();
        }
    }
}