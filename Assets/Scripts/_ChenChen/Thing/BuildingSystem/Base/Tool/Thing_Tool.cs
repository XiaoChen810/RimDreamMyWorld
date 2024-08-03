using ChenChen_UI;
using UnityEngine;

namespace ChenChen_Thing
{
    public class Thing_Tool : Thing
    {
        public bool IsSuccess { get; private set; }

        public TargetPtr wookPos;

        public virtual void OpenMenu()
        {
            Debug.Log("未设置");
        }

        public override DetailView DetailView
        {
            get
            {
                if (_detailView == null)
                {
                    if (!TryGetComponent<DetailView>(out _detailView))
                    {
                        _detailView = gameObject.AddComponent<DetailView_Tool>();
                    }
                }
                return _detailView;
            }
        }
    }
}
