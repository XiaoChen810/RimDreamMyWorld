using ChenChen_Thing;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UI
{
    /// <summary>
    /// 细节视图，展示细节
    /// </summary>
    public abstract class DetailView : MonoBehaviour
    {
        public bool IsPanelOpen = false;
        public bool IsIndicatorOpen = false;
        protected Indicator indicator;

        /// <summary>
        /// 当前面板上输出的内容
        /// </summary>
        public List<string> Content = new List<string>();

        /// <summary>
        /// 显示指示器
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
        /// 关闭指示器
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
        /// 打开显示面板
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