using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLight : MonoBehaviour
{
    public Light2D globalLight;

    [Header("白天时间段")]
    public float dayIntensity;

    [Header("夜晚时间段")]
    public float nightIntensity;

    [Header("颜色")]
    public Color dayColor;
    public Color nightColor;

    [Header("过渡时间")]
    public float transitionDuration = 1.0f;

    private bool isDay;
    private bool isTransitioning;
    private float transitionProgress;

    private void Start()
    {
        // 初始化为白天或夜晚
        isDay = GameManager.Instance.IsDayTime;
        SetLighting(isDay ? dayIntensity : nightIntensity, isDay ? dayColor : nightColor);
    }

    private void Update()
    {
        // 检查是否在白天或夜晚
        bool currentIsDay = GameManager.Instance.IsDayTime; 

        if (currentIsDay != isDay && !isTransitioning)
        {
            // 开始过渡
            isTransitioning = true;
            StartCoroutine(TransitionLighting(currentIsDay));
        }
    }

    private IEnumerator TransitionLighting(bool toDay)
    {
        float startIntensity = globalLight.intensity;
        Color startColor = globalLight.color;
        float endIntensity = toDay ? dayIntensity : nightIntensity;
        Color endColor = toDay ? dayColor : nightColor;

        transitionProgress = 0.0f;

        while (transitionProgress < 1.0f)
        {
            transitionProgress += Time.deltaTime / transitionDuration;
            globalLight.intensity = Mathf.Lerp(startIntensity, endIntensity, transitionProgress);
            globalLight.color = Color.Lerp(startColor, endColor, transitionProgress);
            yield return null;
        }

        // 确保最终值正确
        globalLight.intensity = endIntensity;
        globalLight.color = endColor;

        isDay = toDay;
        isTransitioning = false;
    }

    private void SetLighting(float intensity, Color color)
    {
        globalLight.intensity = intensity;
        globalLight.color = color;
    }
}
