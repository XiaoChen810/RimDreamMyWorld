using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightRayEffect : MonoBehaviour
{
    public List<Light2D> rays;
    public float minIntensity = 0f;
    public float maxIntensity = 0.5f;
    public float speedIntensity = 1f;
    public float minYScale = 3f;
    public float maxYScale = 8f;
    public float speedYScale = 1f;
    public float minXScale = 50f;
    public float maxXScale = 100f;
    public float speedXScale = 1f;
    public Color dayLight;
    public Color nightLight;
    private List<float> phaseOffsetsIntensity;
    private List<float> phaseOffsetsXScale;
    private List<float> phaseOffsetsYScale;

    private void Start()
    {
        //��ÿ���������һ��ƫ��ֵ
        phaseOffsetsIntensity = new List<float>();
        phaseOffsetsXScale = new List<float>();
        phaseOffsetsYScale = new List<float>();

        foreach (Light2D light in rays)
        {
            phaseOffsetsIntensity.Add(Random.Range(0f, 2f * Mathf.PI));
            phaseOffsetsXScale.Add(Random.Range(0f, 2f * Mathf.PI));
            phaseOffsetsYScale.Add(Random.Range(0f, 2f * Mathf.PI));
        }
    }

    private void Update()
    {
        for (int i = 0; i < rays.Count; i++)
        {
            // ����ǿ��
            float sinValueIntensity = Mathf.Sin(Time.time * speedIntensity + phaseOffsetsIntensity[i]);
            sinValueIntensity = sinValueIntensity < 0 ? 0 : sinValueIntensity;
            float intensity = Mathf.Lerp(minIntensity, maxIntensity, sinValueIntensity);

            // ����y�����ţ����߿��
            float sinValueYScale = Mathf.Sin(Time.time * speedYScale + phaseOffsetsYScale[i]);
            float yScale = Mathf.Lerp(minYScale, maxYScale, Mathf.Abs(sinValueYScale));

            // ����x�����ţ����߳���
            float sinValueXScale = Mathf.Sin(Time.time * speedXScale + phaseOffsetsXScale[i]);
            float xScale = Mathf.Lerp(minXScale, maxXScale, Mathf.Abs(sinValueXScale));

            // ��ֵ
            rays[i].intensity = intensity;
            rays[i].transform.localScale = new Vector3(xScale, yScale, rays[i].transform.localScale.z);

            // �ı���ɫ
            bool isDayLight = GameManager.Instance.IsDayTime;
            rays[i].color = isDayLight ? dayLight : nightLight;
        }
    }
}
