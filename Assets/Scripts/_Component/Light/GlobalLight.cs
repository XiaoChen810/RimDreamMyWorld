using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLight : MonoBehaviour
{
    public Light2D globalLight;

    [Header("����ʱ���")]
    public float dayIntensity;

    [Header("ҹ��ʱ���")]
    public float nightIntensity;

    [Header("��ɫ")]
    public Color dayColor;
    public Color nightColor;

    [Header("����ʱ��")]
    public float transitionDuration = 1.0f;

    private bool isDay;
    private bool isTransitioning;
    private float transitionProgress;

    private void Start()
    {
        // ��ʼ��Ϊ�����ҹ��
        isDay = GameManager.Instance.IsDayTime;
        SetLighting(isDay ? dayIntensity : nightIntensity, isDay ? dayColor : nightColor);
    }

    private void Update()
    {
        // ����Ƿ��ڰ����ҹ��
        bool currentIsDay = GameManager.Instance.IsDayTime; 

        if (currentIsDay != isDay && !isTransitioning)
        {
            // ��ʼ����
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

        // ȷ������ֵ��ȷ
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
