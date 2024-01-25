using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using µØÍ¼Éú³É;

public class DebugForMe : MonoBehaviour
{
    [SerializeField] private Text debugText;
    [SerializeField] private bool openDebug;
    [SerializeField] private MapCreator mapCreator;

    private void Start()
    {
        mapCreator = GameObject.Find("MapCreator").GetComponent<MapCreator>();
    }

    private void Update()
    {
        if (openDebug)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            string content = mapCreator.GetData(mouseWorldPos);

            debugText.text = content;
        }
    }
}
