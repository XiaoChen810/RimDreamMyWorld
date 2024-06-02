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
            // 播放点击音效
            AudioManager.Instance.PlaySFX("ButtonClick");
            // 调用基类的点击事件处理
            base.OnPointerClick(eventData);
        }
    }
}