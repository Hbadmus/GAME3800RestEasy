using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleFlicker : MonoBehaviour
{
    public GameObject flame;
    public GameObject light;
    public float flickerSpeed = 0.1f;
    public float targetIntensity;
    public float minIntensity = 0.6f;
    public float maxIntensity = 1f;
    public Color flameColor;

    /*
    // Start is called before the first frame update
    void Start()
    {
        if (!flame)
            Debug.LogWarning("Assign flame game object to CandleFlicker script");
    }

    // Update is called once per frame
    void Update()
    {
        if (Random.value < flickerSpeed)
            targetIntensity = Random.Range(minIntensity, maxIntensity);
        
        light.intensity = Mathf.SmoothDamp(
            candleLight.intensity,
            targetIntensity,
            ref currentVelocity,
            0.05f
        );
    }

    public void LightCandle()
    {
        flame.SetActive(true);
    }
    */

}
