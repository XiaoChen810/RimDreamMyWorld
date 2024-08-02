using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class Panel_Time : MonoBehaviour
    {
        public Text timeText;

        void Update()
        {
            timeText.text = GameManager.Instance.CurrentTime;
        }
    }
}