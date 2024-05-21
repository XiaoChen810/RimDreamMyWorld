using ChenChen_BuildingSystem;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPanel : MonoBehaviour
{
    public Button U;
    public Button T;

    private void Start()
    {
        if (U != null) U.onClick.AddListener(() =>
        {
            ThingSystemManager.Instance.OpenBuildingMenuPanel();
        });
        if (T != null) T.onClick.AddListener(() =>
        {
            GameManager.Instance.WorkSpaceTool.AddOneFarmWorkSpace();
        });
    }
}
