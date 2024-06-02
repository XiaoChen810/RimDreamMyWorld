using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class CC_Button : Button
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            // ���ŵ����Ч
            AudioManager.Instance.PlaySFX("ButtonClick");
            // ���û���ĵ���¼�����
            base.OnPointerClick(eventData);
        }
    }
}