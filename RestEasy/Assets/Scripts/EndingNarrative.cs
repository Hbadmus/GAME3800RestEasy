using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

/*
public class EndingNarrative : MonoBehaviour
{
    public GameObject canvas;

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
    private GameManager gameManager;
    private bool narrativeComplete = false;
    private void Awake()
    {
        // Find game manager
        gameManager = FindObjectOfType<GameManager>();

        // Ensure we have a canvas group for fading
        textCanvasGroup = narrativeText.GetComponent<CanvasGroup>();
        if (textCanvasGroup == null)
            textCanvasGroup = narrativeText.gameObject.AddComponent<CanvasGroup>();

        // Start with text invisible but keep GameObject active
        textCanvasGroup.alpha = 0;
        narrativeText.text = "";

        // Instead of deactivating the whole object, just hide the text
        narrativeText.gameObject.SetActive(false);

        // Debug log to verify this script is running at start
        Debug.Log("EndingNarrative script initialized and ready");
    }

    public void StartNarrative()
    {
        // Reset index
        currentElementIndex = 0;

        // make other canvas elements invisible
        if (canvas != null)
        {
            canvas.SetActive(false);
        }

        // Make sure this object is active
        gameObject.SetActive(true);

        // Make text visible over black background
        if (narrativeText != null)
        {
            narrativeText.gameObject.SetActive(true);
        }

        // Start the narrative sequence
        StartCoroutine(BeginNarrative());
    }

    private IEnumerator BeginNarrative()
    {
        Debug.Log("Beginning ending narrative");

        // Wait a brief moment before starting
        yield return new WaitForSeconds(1f);

        // Display first element
        DisplayNextElement();
    }

    private void Update()
    {
        //Skip function
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Skipping ending narrative");
            canvas.SetActive(true);
            EndNarrative();
            return;
        }

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
            // We've reached the end of all narrative elements
            narrativeComplete = true;
            canvas.SetActive(true);
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
        // Only notify GameManager when we've gone through all narrative elements
        if (narrativeComplete && gameManager != null)
        {
            Debug.Log("All narrative elements complete - ending game");
            gameManager.OnEndingNarrativeComplete();
        }
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
*/