using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Post Processing Settings")]
    [SerializeField] private PostProcessVolume postProcessVolume;
    [SerializeField] private float transitionDuration = 2.0f;
    [SerializeField] private float maxVignetteIntensity = 1.0f;

    [Header("UI Blackout")]
    [SerializeField] private Image blackoutImage;
    [SerializeField] private float finalBlackoutAlpha = 1.0f;

    [Header("Camera Settings")]
    [SerializeField] private Camera gameplayCamera;
    [SerializeField] private Camera cutsceneCamera;

    [Header("Cutscene Settings")]
    [SerializeField] private PlayableDirector[] cutsceneTimelines;
    [SerializeField] private float delayBeforeCutscene = 0.5f;

    private Vignette vignette;
    private DepthOfField depthOfField;
    private int currentCutsceneIndex = 0;
    private bool isTransitioning = false;

    private void Awake()
    {
        // Setup post-processing effects
        SetupPostProcessing();

        // Initialize cameras
        SetupCameras();
    }

    private void SetupPostProcessing()
    {
        // Get the vignette effect from post-processing volume
        postProcessVolume.profile.TryGetSettings(out vignette);
        if (vignette == null)
        {
            Debug.LogError("Vignette effect not found in PostProcessVolume! Please add it to your profile.");
        }
        else
        {
            // Initialize vignette to zero intensity
            vignette.intensity.value = 0f;
        }

        // Get the depth of field effect from post-processing volume
        postProcessVolume.profile.TryGetSettings(out depthOfField);
        if (depthOfField == null)
        {
            Debug.LogError("Depth of Field effect not found in PostProcessVolume! Please add it to your profile.");
        }
        else
        {
            // Initialize depth of field (no blur)
            depthOfField.active = false;
        }

        // Initialize blackout image if assigned
        if (blackoutImage != null)
        {
            Color color = blackoutImage.color;
            color.a = 0f;
            blackoutImage.color = color;
            blackoutImage.gameObject.SetActive(false);
        }
    }

    private void SetupCameras()
    {
        if (gameplayCamera != null && cutsceneCamera != null)
        {
            // Ensure gameplay camera is active and cutscene camera is inactive at start
            gameplayCamera.gameObject.SetActive(true);
            cutsceneCamera.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("One or both cameras are not assigned!");
        }
    }

    private void Start()
    {
        Invoke("OnPuzzleCompleted", 10f);  // First cutscene at 10 seconds
        //Invoke("OnPuzzleCompleted", 60f);  // Second cutscene at 60 seconds
    }

    // Call this method when a puzzle is completed (key is grabbed)
    public void OnPuzzleCompleted()
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionToCutscene());
        }
    }

    private IEnumerator TransitionToCutscene()
    {
        isTransitioning = true;

        // Disable player input here
        // PlayerController.instance.DisableInput();

        // Step 1: Fade to black with gameplay camera still active
        yield return StartCoroutine(FadeToBlack(true));

        // Step 2: Switch cameras while completely black
        gameplayCamera.gameObject.SetActive(false);
        cutsceneCamera.gameObject.SetActive(true);

        // Step 3: Start the cutscene BEFORE fading in
        StartPlayingCutscene();

        // Step 4: Wait a short moment for the cutscene to initialize
        yield return new WaitForSeconds(0.3f);

        // Step 5: Fade in from black with cutscene camera
        yield return StartCoroutine(FadeFromBlack());
    }

    private IEnumerator FadeToBlack(bool enableBlur = true)
    {
        // Activate blackout image
        if (blackoutImage != null)
        {
            Color color = blackoutImage.color;
            color.a = 0f;
            blackoutImage.color = color;
            blackoutImage.gameObject.SetActive(true);
        }

        // Gradually increase vignette intensity and blur to create blackout effect
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            float normalizedTime = elapsedTime / transitionDuration;

            // Update vignette
            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(0f, maxVignetteIntensity, normalizedTime);
            }

            // Update depth of field if enabled
            if (depthOfField != null && enableBlur)
            {
                depthOfField.active = true;
                depthOfField.focusDistance.value = Mathf.Lerp(10f, 0.1f, normalizedTime);
                depthOfField.aperture.value = Mathf.Lerp(0.1f, 32f, normalizedTime);
            }

            // Update blackout image
            if (blackoutImage != null)
            {
                Color color = blackoutImage.color;
                color.a = Mathf.Lerp(0f, finalBlackoutAlpha, normalizedTime);
                blackoutImage.color = color;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure everything is at maximum intensity
        if (vignette != null)
        {
            vignette.intensity.value = maxVignetteIntensity;
        }

        if (depthOfField != null && enableBlur)
        {
            depthOfField.focusDistance.value = 0.1f;
            depthOfField.aperture.value = 32f;
        }

        if (blackoutImage != null)
        {
            Color color = blackoutImage.color;
            color.a = finalBlackoutAlpha;
            blackoutImage.color = color;
        }
    }

    private IEnumerator FadeFromBlack()
    {
        // Gradually decrease blackout intensity
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            float normalizedTime = elapsedTime / transitionDuration;

            // Update vignette
            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(maxVignetteIntensity, 0f, normalizedTime);
            }

            // Update depth of field
            if (depthOfField != null)
            {
                depthOfField.focusDistance.value = Mathf.Lerp(0.1f, 10f, normalizedTime);
                depthOfField.aperture.value = Mathf.Lerp(32f, 0.1f, normalizedTime);
            }

            // Update blackout image
            if (blackoutImage != null)
            {
                Color color = blackoutImage.color;
                color.a = Mathf.Lerp(finalBlackoutAlpha, 0f, normalizedTime);
                blackoutImage.color = color;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure everything is reset
        if (vignette != null)
        {
            vignette.intensity.value = 0f;
        }

        if (depthOfField != null)
        {
            depthOfField.active = false;
        }

        if (blackoutImage != null)
        {
            Color color = blackoutImage.color;
            color.a = 0f;
            blackoutImage.color = color;
            blackoutImage.gameObject.SetActive(false);
        }
    }

    private void StartPlayingCutscene()
    {
        if (cutsceneTimelines == null || cutsceneTimelines.Length == 0)
        {
            Debug.LogError("No cutscene timelines assigned to GameManager!");
            EndCutsceneSequence();
            return;
        }

        // Ensure index is within bounds
        if (currentCutsceneIndex >= cutsceneTimelines.Length)
        {
            Debug.LogWarning("Cutscene index out of range! Resetting to 0.");
            currentCutsceneIndex = 0;
        }

        // Get the current timeline to play
        PlayableDirector currentTimeline = cutsceneTimelines[currentCutsceneIndex];

        if (currentTimeline != null)
        {
            // Subscribe to timeline completion event
            currentTimeline.stopped += OnCutsceneComplete;

            // Play the timeline
            currentTimeline.Play();
        }
        else
        {
            Debug.LogError($"Timeline at index {currentCutsceneIndex} is null!");
            EndCutsceneSequence();
        }
    }

    private void OnCutsceneComplete(PlayableDirector director)
    {
        // Unsubscribe from the event
        director.stopped -= OnCutsceneComplete;

        // Increment cutscene index for next time
        currentCutsceneIndex++;

        // Start transition back to gameplay
        StartCoroutine(TransitionToGameplay());
    }

    private IEnumerator TransitionToGameplay()
    {
        // Step 1: Fade to black with cutscene camera still active
        yield return StartCoroutine(FadeToBlack(false));

        // Step 2: Switch cameras while completely black
        cutsceneCamera.gameObject.SetActive(false);
        gameplayCamera.gameObject.SetActive(true);

        // Step 3: Wait a moment
        yield return new WaitForSeconds(0.5f);

        // Step 4: Fade in from black with gameplay camera
        yield return StartCoroutine(FadeFromBlack());

        // Step 5: Complete the sequence
        EndCutsceneSequence();
    }

    private void EndCutsceneSequence()
    {
        // Re-enable player input if needed
        // PlayerController.instance.EnableInput();

        isTransitioning = false;

    }
}