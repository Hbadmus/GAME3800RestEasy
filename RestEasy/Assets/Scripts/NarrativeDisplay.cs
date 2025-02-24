using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class NarrativeDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image blackScreen;
    [SerializeField] private TextMeshProUGUI narrativeText;

    [Header("Settings")]
    [SerializeField] private bool useClickProgression = true;
    [SerializeField] private float autoProgressionDelay = 3f;
    [SerializeField] private float fadeInSpeed = 2f;

    [Header("Narrative Elements")]
    [SerializeField] private string[] narrativeElements;

    private int currentElementIndex = 0;
    private bool isDisplaying = false;
    private CanvasGroup textCanvasGroup;

    private void Start()
    {
        // Ensure we have a canvas group for fading
        textCanvasGroup = narrativeText.GetComponent<CanvasGroup>();
        if (textCanvasGroup == null)
            textCanvasGroup = narrativeText.gameObject.AddComponent<CanvasGroup>();

        // Start with text invisible
        textCanvasGroup.alpha = 0;
        narrativeText.text = "";

        // Start the narrative sequence
        StartCoroutine(BeginNarrative());
    }

    private IEnumerator BeginNarrative()
    {
        // Wait a brief moment before starting
        yield return new WaitForSeconds(1f);

        // Display first element
        DisplayNextElement();
    }

    private void Update()
    {
        if (useClickProgression && Input.GetMouseButtonDown(0))
        {
            if (!isDisplaying)
            {
                DisplayNextElement();
            }
        }
    }

    private void DisplayNextElement()
    {
        if (currentElementIndex >= narrativeElements.Length)
        {
            EndNarrative();
            return;
        }

        StartCoroutine(FadeInText(narrativeElements[currentElementIndex]));
        currentElementIndex++;

        if (!useClickProgression)
        {
            StartCoroutine(AutoProgressCoroutine());
        }
    }

    private IEnumerator FadeInText(string text)
    {
        isDisplaying = true;
        narrativeText.text = text;

        float currentAlpha = 0;
        textCanvasGroup.alpha = 0;

        while (currentAlpha < 1)
        {
            currentAlpha += Time.deltaTime * fadeInSpeed;
            textCanvasGroup.alpha = currentAlpha;
            yield return null;
        }

        isDisplaying = false;
    }

    private IEnumerator AutoProgressCoroutine()
    {
        yield return new WaitForSeconds(autoProgressionDelay);
        if (!useClickProgression && !isDisplaying)
        {
            DisplayNextElement();
        }
    }

    private void EndNarrative()
    {
        // Fade out black screen or transition to gameplay
        StartCoroutine(FadeOutBlackScreen());
    }

    private IEnumerator FadeOutBlackScreen()
    {
        float currentAlpha = 1;
        while (currentAlpha > 0)
        {
            currentAlpha -= Time.deltaTime * fadeInSpeed;
            blackScreen.color = new Color(0, 0, 0, currentAlpha);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}