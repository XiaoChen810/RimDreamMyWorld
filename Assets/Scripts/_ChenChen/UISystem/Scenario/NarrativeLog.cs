using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public class NarrativeLog : MonoBehaviour
    {
        public GameObject Target { get; private set; }

        private Button Button;

        private Text Text;

        public void Init(string text, GameObject target)
        {
            Button = GetComponentInChildren<Button>();
            Text = GetComponentInChildren<Text>();
            Text.text = text;
            Target = target;
            Button.onClick.AddListener(CameraMoveToTarget);
        }

        private void CameraMoveToTarget()
        {
            if (Target != null)
            {
                Vector3 go = Target.transform.position;
                go.z = Camera.main.transform.position.z;
                Camera.main.transform.DOMove(go, 1);
            }
        }
    }
}