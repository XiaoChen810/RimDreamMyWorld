using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLight : MonoBehaviour
{
    public Light2D globalLight;
    public AnimationCurve aDayLight;
    public float minINtensity;
    public float maxINtensity;

    private void Update()
    {
        float hout = GameManager.Instance.currentHour; 
        float min = GameManager.Instance.currentMinute;
        float time = hout * 60 + min;
        float aday = 24 * 60;
        globalLight.intensity = Mathf.Lerp(minINtensity, maxINtensity, aDayLight.Evaluate(time / aday));
    }
}
