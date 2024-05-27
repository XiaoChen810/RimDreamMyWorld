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
        public List<string> Content = new List<string>();

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
        /// ����ʾ���
        /// </summary>
        public abstract void OpenPanel();

        public virtual void ClosePanel()
        {
            if (IsPanelOpen)
            {
                DetailViewManager.Instance.PanelManager.RemoveTopPanel();
            }
        }

        protected abstract void UpdateShow(DetailViewPanel panel);

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