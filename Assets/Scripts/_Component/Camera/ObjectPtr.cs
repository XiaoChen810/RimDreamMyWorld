using UnityEngine;
using UnityEngine.UI;

public class ObjectPtr : MonoBehaviour
{
    public TargetPtr Target { get; private set; }

    private Button Button;

    public void Init(TargetPtr target)
    {
        Button = GetComponentInChildren<Button>();
        Target = target;
        Button.onClick.AddListener(target.CameraMoveToTarget);
    }
}
