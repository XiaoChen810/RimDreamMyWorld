using UnityEngine;
using UnityEngine.UI;

public class ObjectPtr : MonoBehaviour
{
    public TargetPtr Target { get; private set; }

    [Header("移动到目标事件的按钮")]
    [SerializeField] private Button target_Btn;

    public virtual void Init(TargetPtr target)
    {
        if(target_Btn != null)
        {
            Target = target;
            target_Btn.onClick.AddListener(target.CameraMoveToTarget);
        }
        else
        {
            Debug.LogError("组件为空");
        }

    }
}
