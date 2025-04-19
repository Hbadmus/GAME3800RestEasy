using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineSignalReceiver : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private GameManager gameManager; // Reference to your GameManager

    private bool isPaused = false;

    private void OnEnable()
    {
        if (director == null)
            director = GetComponent<PlayableDirector>();

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    // This will be called whenever a Signal Emitter in the timeline is reached
    public void OnTimelinePausePoint()
    {
        if (director != null && director.state == PlayState.Playing)
        {
            // Pause the timeline
            director.Pause();
            isPaused = true;
            Debug.Log("Timeline paused at signal point");

            // Transition back to gameplay
            if (gameManager != null)
            {
                gameManager.StartCoroutine(gameManager.TransitionToGameplay());
            }
        }
    }

    // Method to resume the timeline
    public void ResumeTimeline()
    {
        if (director != null && isPaused)
        {
            director.Play();
            isPaused = false;
            Debug.Log("Timeline resumed");
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}