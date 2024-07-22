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
        protected PanelBase myPanel;

        /// <summary>
        /// ��ǰ��������������
        /// </summary>
        protected List<string> content = new List<string>();

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

        public virtual void CloseIndicator()
        {
            IsIndicatorOpen = false;
            if (indicator != null)
            {
                indicator.gameObject.SetActive(false);
            }            
        }

        public abstract void OpenPanel();

        public virtual void ClosePanel()
        {
            DetailViewManager.Instance.PanelManager.RemovePanel(DetailViewManager.Instance.PanelManager.GetTopPanel());
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