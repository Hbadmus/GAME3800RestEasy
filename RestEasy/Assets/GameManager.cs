using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour
{
    [Header("Vignette Settings")]
    [SerializeField] private PostProcessVolume postProcessVolume;
    [SerializeField] private float transitionDuration = 2.0f;
    [SerializeField] private float maxVignetteIntensity = 1.0f;

    [Header("Cutscene Settings")]
    [SerializeField] private PlayableDirector[] cutsceneTimelines;
    [SerializeField] private float delayBeforeCutscene = 0.5f;

    private Vignette vignette;
    private int currentCutsceneIndex = 0;
    private bool isTransitioning = false;

    private void Awake()
    {
        // Get the vignette effect from post-processing volume
        postProcessVolume.profile.TryGetSettings(out vignette);
        if (vignette == null)
        {
            Debug.LogError("Vignette effect not found in PostProcessVolume! Please add it to your profile.");
        }

        // Initialize vignette to zero intensity
        if (vignette != null)
        {
            vignette.intensity.value = 0f;
        }
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

        // Disable player input here if needed
        // PlayerController.instance.DisableInput();

        // Gradually increase vignette intensity to create blackout effect
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            float normalizedTime = elapsedTime / transitionDuration;
            vignette.intensity.value = Mathf.Lerp(0f, maxVignetteIntensity, normalizedTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure vignette is at maximum intensity
        vignette.intensity.value = maxVignetteIntensity;

        // Wait a short moment before starting cutscene
        yield return new WaitForSeconds(delayBeforeCutscene);

        // Play the appropriate cutscene
        PlayCutscene();
    }

    private void PlayCutscene()
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

        // Start fading the vignette back out
        StartCoroutine(FadeOutVignette());
    }

    private IEnumerator FadeOutVignette()
    {
        // Gradually decrease vignette intensity
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            float normalizedTime = elapsedTime / transitionDuration;
            vignette.intensity.value = Mathf.Lerp(maxVignetteIntensity, 0f, normalizedTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure vignette is reset to zero
        vignette.intensity.value = 0f;

        EndCutsceneSequence();
    }

    private void EndCutsceneSequence()
    {
        // Re-enable player input if needed
        // PlayerController.instance.EnableInput();

        isTransitioning = false;

        // Perform any additional actions needed after the sequence
    }
}