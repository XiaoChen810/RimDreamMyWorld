using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimePanel : MonoBehaviour
{
    public Text timeText;

    void Update()
    {
        timeText.text = GameManager.Instance.CurrentTime;
    }
}
