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
    private List<float> phaseOffsetsIntensity;
    private List<float> phaseOffsetsXScale;
    private List<float> phaseOffsetsYScale;

    private void Start()
    {
        // Initialize phase offsets for each light for intensity and scales
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
            // Calculate intensity
            float sinValueIntensity = Mathf.Sin(Time.time * speedIntensity + phaseOffsetsIntensity[i]);
            sinValueIntensity = sinValueIntensity < 0 ? 0 : sinValueIntensity;
            float intensity = Mathf.Lerp(minIntensity, maxIntensity, sinValueIntensity);
            rays[i].intensity = intensity;

            // Calculate Y-axis scale
            float sinValueYScale = Mathf.Sin(Time.time * speedYScale + phaseOffsetsYScale[i]);
            float yScale = Mathf.Lerp(minYScale, maxYScale, Mathf.Abs(sinValueYScale));

            // Calculate X-axis scale
            float sinValueXScale = Mathf.Sin(Time.time * speedXScale + phaseOffsetsXScale[i]);
            float xScale = Mathf.Lerp(minXScale, maxXScale, Mathf.Abs(sinValueXScale));

            // Apply scales
            rays[i].transform.localScale = new Vector3(xScale, yScale, rays[i].transform.localScale.z);
        }
    }
}
