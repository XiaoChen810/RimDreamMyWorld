using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class TimePanel : MonoBehaviour
    {
        public Text timeText;

        void Update()
        {
            timeText.text = GameManager.Instance.CurrentTime;
        }
    }
}