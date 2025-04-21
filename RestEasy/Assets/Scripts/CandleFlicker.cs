using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleFlicker : MonoBehaviour
{
    public GameObject flame;
    public Light light;
    public float flickerSpeed = 0.1f;
    public float targetIntensity;
    public float minIntensity = 0.6f;
    public float maxIntensity = 1f;
    public Color flameColor;
    float currentVelocity;

    void Start()
    {
        if (!flame)
            Debug.LogWarning("Assign flame game object to CandleFlicker script");
    }

    void Update()
    {
        if (!flame.activeInHierarchy)
            return;
        
        if (Random.value < flickerSpeed)
            targetIntensity = Random.Range(minIntensity, maxIntensity);
        
        light.intensity = Mathf.SmoothDamp(
            light.intensity,
            targetIntensity,
            ref currentVelocity,
            0.05f
        );
    }

    public void LightCandle()
    {
        flame.SetActive(true);
    }
}
