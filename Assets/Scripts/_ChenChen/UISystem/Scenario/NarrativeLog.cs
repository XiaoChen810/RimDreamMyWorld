using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public class NarrativeLog : MonoBehaviour
    {
        public TargetPtr Target { get; private set; }

        private Button Button;

        private Text Text;

        public void Init(string text, TargetPtr target)
        {
            Button = GetComponentInChildren<Button>();
            Text = GetComponentInChildren<Text>();
            Text.text = text;
            Target = target;
            Button.onClick.AddListener(target.CameraMoveToTarget);
        }
    }
}