using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWhisper : MonoBehaviour
{
    public float intervalMax;
    public float intervalMin;

    float timeSinceLastPlay;
    float currentInterval;

    void Start()
    {
        timeSinceLastPlay = 0f;
        currentInterval = Random.Range(intervalMax, intervalMin);
    }

    void Update()
    {
        timeSinceLastPlay += Time.deltaTime;

        if (timeSinceLastPlay >= currentInterval)
            PlayAndReset();
    }

    void PlayAndReset()
    {
        AudioManager.instance.PlaySFX("ghost-whisper", 0f);
        currentInterval = Random.Range(intervalMax, intervalMin);
        timeSinceLastPlay = 0f;
    }
}
