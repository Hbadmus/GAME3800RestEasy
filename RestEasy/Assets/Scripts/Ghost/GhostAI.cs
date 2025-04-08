using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Controls Donna's haunting influence throughout the house. 
/// This system represents the ghost's emotional presence within the entire house, affecting objects
/// and creating environmental effects based on the ghost's current state.
public class GhostAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource ghostAudioSource;
    [SerializeField] private GameManager gameManager;

    [Header("Ghost State Variables")]
    [SerializeField] private GhostState currentState = GhostState.Suspicious;
    [SerializeField] private float stateChangeTimer;
    [SerializeField] private float stateChangeInterval = 10f;
    [SerializeField] private float activityRadius = 8f; // Radius around player where ghost activity occurs

    [Header("Interaction")]
    [SerializeField] private float smallObjectPushForce = 5f;
    [SerializeField] private float largeObjectPushForce = 10f;
    [SerializeField] private float maxMovableObjectMass = 20f; // Objects heavier than this are considered "fixed"
    [SerializeField] private float flickeringLightIntensity = 0.3f;
    [SerializeField] private float windIntensity = 1f;
    [SerializeField] private float disturbanceDecayRate = 0.05f; // How quickly disturbance level decreases per second

    [Header("Audio")]
    [SerializeField] private AudioClip[] suspiciousSounds;
    [SerializeField] private AudioClip[] defensiveSounds;
    [SerializeField] private AudioClip[] revelatoryDialogue;
    [SerializeField] private AudioClip[] acceptingSounds;
    [SerializeField] private AudioClip[] objectMovementSounds;
    [SerializeField] private AudioClip windSound;
    [SerializeField] private AudioClip heartbeatSound;

    [Header("Emotional Tags")]
    [SerializeField] private string[] highEmotionalTags; // Tags for objects with high emotional value (0.8-1.0)
    [SerializeField] private string[] mediumEmotionalTags; // Tags for objects with medium emotional value (0.5-0.7)
    [SerializeField] private string[] lowEmotionalTags; // Tags for objects with low emotional value (0.2-0.4)

    // Internal tracking variables
    private Transform player;
    private Light[] lightsInScene;
    private GhostState previousState;
    private float timeSinceLastActivity = 0f;
    private bool isPossessingPlayer = false;
    private int storyProgressionStage = 0;
    private float playerDisturbanceLevel = 0f;
    private Dictionary<string, float> emotionalTagValues = new Dictionary<string, float>();

    // Singleton instance for easy access
    public static GhostAI Instance { get; private set; }

    // Enum for ghost states
    public enum GhostState
    {
        Suspicious,  // Default state, mild activity
        Defensive,   // Actively tries to block or scare player
        Revelatory,  // Shares memories/possession state
        Accepting    // Helps the player subtly
    }

    private void Awake()
    {
        // Setup singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Find the player in the scene
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Find Game Manager if not assigned
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager not found in the scene. Required for ghost interactions.");
            }
        }

        // Cache all lights in the scene
        lightsInScene = FindObjectsOfType<Light>();

        // Initialize emotional tag values
        InitializeEmotionalTagValues();

        // Set up the ghost's initial state
        SetGhostState(GhostState.Suspicious);
    }

    private void InitializeEmotionalTagValues()
    {
        // Set high emotional value tags (0.8-1.0)
        foreach (string tag in highEmotionalTags)
        {
            emotionalTagValues[tag] = Random.Range(0.8f, 1.0f);
        }

        // Set medium emotional value tags (0.5-0.7)
        foreach (string tag in mediumEmotionalTags)
        {
            emotionalTagValues[tag] = Random.Range(0.5f, 0.7f);
        }

        // Set low emotional value tags (0.2-0.4)
        foreach (string tag in lowEmotionalTags)
        {
            emotionalTagValues[tag] = Random.Range(0.2f, 0.4f);
        }
    }

    private void Update()
    {
        // Track time since last ghost activity
        timeSinceLastActivity += Time.deltaTime;

        // Check if we should change state based on timer
        stateChangeTimer -= Time.deltaTime;
        if (stateChangeTimer <= 0f)
        {
            // Only change state randomly if not in Revelatory state
            if (currentState != GhostState.Revelatory)
            {
                ConsiderStateChange();
            }

            // Reset the timer
            stateChangeTimer = stateChangeInterval;
        }

        // Handle state-specific behaviors
        switch (currentState)
        {
            case GhostState.Suspicious:
                HandleSuspiciousState();
                break;

            case GhostState.Defensive:
                HandleDefensiveState();
                break;

            case GhostState.Revelatory:
                HandleRevelatoryState();
                break;

            case GhostState.Accepting:
                HandleAcceptingState();
                break;
        }

        // If we've been inactive for too long, do something
        if (timeSinceLastActivity > Random.Range(15f, 30f))
        {
            PerformRandomGhostActivity();
        }

        // Gradually decrease disturbance level over time
        playerDisturbanceLevel = Mathf.Max(playerDisturbanceLevel - (Time.deltaTime * disturbanceDecayRate), 0f);
    }

    private void SetGhostState(GhostState newState)
    {
        if (newState == currentState) return;

        previousState = currentState;
        currentState = newState;

        // Play a sound for the new state
        PlayRandomStateSound();

        // Log state change for debugging
        Debug.Log("Ghost state changed from " + previousState + " to " + currentState);
    }

    private void ConsiderStateChange()
    {
        // Check if player has triggered a state change through interaction
        if (playerDisturbanceLevel > 0.7f)
        {
            SetGhostState(GhostState.Defensive);
            return;
        }

        // Consider story progression for state changes
        if (storyProgressionStage >= 3 && Random.value < 0.3f)
        {
            SetGhostState(GhostState.Accepting);
            return;
        }

        // Otherwise, mostly stay in Suspicious state with occasional Defensive moments
        float stateRoll = Random.value;

        if (stateRoll < 0.7f || currentState == GhostState.Revelatory)
        {
            SetGhostState(GhostState.Suspicious);
        }
        else if (stateRoll < 0.9f)
        {
            SetGhostState(GhostState.Defensive);
        }
        else if (storyProgressionStage >= 2)
        {
            SetGhostState(GhostState.Accepting);
        }
    }

    private void HandleSuspiciousState()
    {
        // In suspicious state, the ghost mostly watches and creates minor disturbances

        // Occasionally create environmental effects near the player (subtle)
        if (Random.value < 0.01f)
        {
            MoveRandomObjectsNearPlayer(false);
        }

        // Sometimes flicker lights
        if (Random.value < 0.005f)
        {
            FlickerNearbyLight(false);
        }

        // Very occasionally create wind effects
        if (Random.value < 0.002f)
        {
            StartCoroutine(CreateWindEffect(1.5f, false));
        }
    }

    private void HandleDefensiveState()
    {
        // In defensive state, the ghost actively tries to block or scare the player

        // More frequently create environmental effects (intense)
        if (Random.value < 0.03f)
        {
            MoveRandomObjectsNearPlayer(true);
        }

        // More aggressive light flickering
        if (Random.value < 0.01f)
        {
            FlickerNearbyLight(true);
        }

        // Move objects in player's path
        if (Random.value < 0.008f)
        {
            MoveObjectInPlayerPath();
        }

        // Affect doors
        if (Random.value < 0.01f)
        {
            AffectNearbyDoor();
        }

        // Create wind disturbance
        if (Random.value < 0.007f)
        {
            StartCoroutine(CreateWindEffect(3f, true));
        }
    }

    private void HandleRevelatoryState()
    {
        // This state usually happens during scripted events when Donna is possessing the player

        if (!isPossessingPlayer)
        {
            // Initiate possession sequence
            StartCoroutine(PossessionSequence());
        }
    }

    private IEnumerator PossessionSequence()
    {
        isPossessingPlayer = true;

        // Signal to the player controller that possession is happening
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.BecomePossessed();
        }

        // Play possession audio
        if (ghostAudioSource != null && heartbeatSound != null)
        {
            ghostAudioSource.PlayOneShot(heartbeatSound);

            // Wait for audio to finish or a specific duration
            yield return new WaitForSeconds(2.5f); // Adjust time as needed
        }

        // THEN trigger the cutscene via Game Manager
        gameManager.OnPuzzleCompleted();

        timeSinceLastActivity = 0f;
        Debug.Log("Ghost has possessed the player and triggered cutscene");
    }

    private void HandleAcceptingState()
    {
        // In accepting state, the ghost subtly tries to help the player

        // Subtle light effects near important objects
        if (Random.value < 0.01f)
        {
            HighlightClueObject();
        }

        // Sometimes just create a subtle presence so player knows ghost is there
        if (Random.value < 0.01f)
        {
            CreateSubtlePresence();
        }
    }

    private void MoveRandomObjectsNearPlayer(bool intense)
    {
        // Find objects near the player to affect
        Collider[] nearbyObjects = Physics.OverlapSphere(player.position, activityRadius);
        List<Rigidbody> movableObjects = new List<Rigidbody>();

        // Filter for objects with Rigidbodies
        foreach (Collider col in nearbyObjects)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null && !rb.isKinematic && rb.mass <= maxMovableObjectMass)
            {
                movableObjects.Add(rb);
            }
        }

        if (movableObjects.Count == 0) return;

        // Affect some of these objects
        int objectsToAffect = intense ? Mathf.Min(3, movableObjects.Count) : Mathf.Min(1, movableObjects.Count);

        for (int i = 0; i < objectsToAffect; i++)
        {
            if (movableObjects.Count > 0)
            {
                int index = Random.Range(0, movableObjects.Count);
                Rigidbody rb = movableObjects[index];

                // Move the object
                float force = intense ? largeObjectPushForce : smallObjectPushForce;
                // Adjust force based on mass (lighter objects move more)
                force = force * (1f - (rb.mass / maxMovableObjectMass) * 0.8f);

                ApplyForceToObject(rb, force);

                // Remove from list so we don't affect it again
                movableObjects.RemoveAt(index);
            }
        }

        timeSinceLastActivity = 0f;
        Debug.Log("Ghost moved objects near player");
    }

    private void FlickerNearbyLight(bool intense)
    {
        // Find lights near the player
        List<Light> nearbyLights = new List<Light>();

        foreach (Light light in lightsInScene)
        {
            if (Vector3.Distance(light.transform.position, player.position) < activityRadius)
            {
                nearbyLights.Add(light);
            }
        }

        if (nearbyLights.Count == 0) return;

        // Choose lights to flicker
        int lightCount = intense ? Mathf.Min(3, nearbyLights.Count) : 1;

        for (int i = 0; i < lightCount; i++)
        {
            if (nearbyLights.Count > 0)
            {
                int index = Random.Range(0, nearbyLights.Count);
                Light lightToFlicker = nearbyLights[index];

                StartCoroutine(FlickerLight(lightToFlicker,
                                          intense ? flickeringLightIntensity * 1.5f : flickeringLightIntensity,
                                          Random.Range(1f, intense ? 4f : 2f)));

                nearbyLights.RemoveAt(index);
            }
        }

        timeSinceLastActivity = 0f;
        Debug.Log("Ghost flickered lights near player");
    }

    private IEnumerator FlickerLight(Light light, float intensity, float duration)
    {
        if (light == null) yield break;

        float originalIntensity = light.intensity;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Randomly vary the intensity
            light.intensity = originalIntensity * Random.Range(0.1f, 1f);

            // Wait a random short interval
            float interval = Random.Range(0.02f, 0.1f);
            yield return new WaitForSeconds(interval);

            elapsedTime += interval;
        }

        // Restore original intensity
        light.intensity = originalIntensity;
    }

    private IEnumerator PulsateLight(Light light, float intensity, float duration)
    {
        if (light == null) yield break;

        float originalIntensity = light.intensity;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Smoothly vary the intensity using a sine wave
            float intensityFactor = 1f + (Mathf.Sin(elapsedTime * 5f) * 0.5f + 0.5f) * intensity;
            light.intensity = originalIntensity * intensityFactor;

            yield return null;
            elapsedTime += Time.deltaTime;
        }

        // Restore original intensity
        light.intensity = originalIntensity;
    }

    private void MoveObjectInPlayerPath()
    {
        // Calculate player's forward path
        Vector3 playerForward = player.forward;
        Vector3 predictedPosition = player.position + playerForward * 3f;

        // Find objects in the predicted path
        Collider[] objectsInPath = Physics.OverlapSphere(predictedPosition, 2f);
        List<Rigidbody> movableObjects = new List<Rigidbody>();

        foreach (Collider col in objectsInPath)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null && !rb.isKinematic && rb.mass <= maxMovableObjectMass)
            {
                movableObjects.Add(rb);
            }
        }

        if (movableObjects.Count == 0) return;

        // Find closest object to predicted path
        Rigidbody closestObject = null;
        float closestDistance = float.MaxValue;

        foreach (Rigidbody rb in movableObjects)
        {
            float distance = Vector3.Distance(rb.position, predictedPosition);
            if (distance < closestDistance)
            {
                closestObject = rb;
                closestDistance = distance;
            }
        }

        if (closestObject != null)
        {
            // Move it into player's path more aggressively
            Vector3 directionToPlayer = (player.position - closestObject.position).normalized;
            float force = largeObjectPushForce * (1f - (closestObject.mass / maxMovableObjectMass) * 0.5f);

            ApplyForceToObject(closestObject, force, directionToPlayer);

            timeSinceLastActivity = 0f;
            Debug.Log("Ghost moved object into player's path: " + closestObject.name);
        }
    }

    private void AffectNearbyDoor()
    {
        // Find doors near the player
        Collider[] nearbyObjects = Physics.OverlapSphere(player.position, activityRadius * 1.5f);

        foreach (Collider col in nearbyObjects)
        {
            // Check if this might be a door (has a hinge joint)
            HingeJoint hinge = col.GetComponent<HingeJoint>(); //may need to change
            Rigidbody rb = col.GetComponent<Rigidbody>();

            if (hinge != null && rb != null)
            {
                // Determine a good direction to push the door
                Vector3 pushDirection = Random.insideUnitSphere.normalized;

                // Apply force to move door
                rb.AddForce(pushDirection * largeObjectPushForce, ForceMode.Impulse);

                // Play sound
                if (ghostAudioSource != null && objectMovementSounds.Length > 0)
                {
                    ghostAudioSource.PlayOneShot(objectMovementSounds[Random.Range(0, objectMovementSounds.Length)]);
                }

                // Only affect one door at a time
                timeSinceLastActivity = 0f;
                Debug.Log("Ghost affected a door: " + col.name);
                break;
            }
        }
    }

    private void HighlightClueObject()
    {
        // Find objects tagged as clues near the player
        Collider[] nearbyObjects = Physics.OverlapSphere(player.position, activityRadius * 1.5f);
        List<GameObject> clueObjects = new List<GameObject>();

        foreach (Collider col in nearbyObjects)
        {
            if (col.CompareTag("Clue") || col.CompareTag("Key"))
            {
                clueObjects.Add(col.gameObject);
            }
        }

        if (clueObjects.Count == 0) return;

        // Choose a random clue to highlight
        GameObject clueToHighlight = clueObjects[Random.Range(0, clueObjects.Count)];

        // Find the nearest light to this object
        Light nearestLight = null;
        float nearestLightDistance = float.MaxValue;

        foreach (Light light in lightsInScene)
        {
            float distance = Vector3.Distance(light.transform.position, clueToHighlight.transform.position);
            if (distance < nearestLightDistance && distance < 5f)
            {
                nearestLight = light;
                nearestLightDistance = distance;
            }
        }

        // If we found a light, make it pulsate subtly to draw attention
        if (nearestLight != null)
        {
            StartCoroutine(PulsateLight(nearestLight, 0.5f, 3f));

            timeSinceLastActivity = 0f;
            Debug.Log("Ghost highlighted clue object through light: " + clueToHighlight.name);
        }
    }

    private void CreateSubtlePresence()
    {
        // Play a subtle ambient sound
        if (ghostAudioSource != null && acceptingSounds.Length > 0)
        {
            AudioClip clip = acceptingSounds[Random.Range(0, acceptingSounds.Length)];
            ghostAudioSource.PlayOneShot(clip, 0.4f);
        }

        // Subtle light effect near player
        Light[] nearbyLights = GetNearbyLights(activityRadius * 0.7f);
        if (nearbyLights.Length > 0)
        {
            Light light = nearbyLights[Random.Range(0, nearbyLights.Length)];
            StartCoroutine(PulsateLight(light, 0.3f, 2f));
        }

        timeSinceLastActivity = 0f;
        Debug.Log("Ghost created a subtle presence near player");
    }

    private Light[] GetNearbyLights(float radius)
    {
        List<Light> nearbyLights = new List<Light>();

        foreach (Light light in lightsInScene)
        {
            if (Vector3.Distance(light.transform.position, player.position) < radius)
            {
                nearbyLights.Add(light);
            }
        }

        return nearbyLights.ToArray();
    }

    private IEnumerator CreateWindEffect(float duration, bool intense)
    {
        // Play wind sound
        if (ghostAudioSource != null && windSound != null)
        {
            ghostAudioSource.PlayOneShot(windSound, intense ? windIntensity : windIntensity * 0.7f);
        }

        // Find objects near the player
        Collider[] nearbyObjects = Physics.OverlapSphere(player.position, activityRadius);

        foreach (Collider col in nearbyObjects)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null && !rb.isKinematic && rb.mass < 5f) // Only affect light objects
            {
                Vector3 windDirection = Random.insideUnitSphere.normalized;
                windDirection.y = Mathf.Abs(windDirection.y) * 0.2f; // Slight upward component

                float windForce = intense ? smallObjectPushForce * 0.7f : smallObjectPushForce * 0.3f;
                rb.AddForce(windDirection * windForce, ForceMode.Impulse);
            }
        }

        yield return new WaitForSeconds(duration);

        timeSinceLastActivity = 0f;
        Debug.Log("Ghost created a wind effect around player");
    }

    private void ApplyForceToObject(Rigidbody rb, float force, Vector3? customDirection = null)
    {
        if (rb == null) return;

        // If we have a custom direction, use it; otherwise use a random direction
        Vector3 forceDirection;
        if (customDirection.HasValue)
        {
            forceDirection = customDirection.Value;
        }
        else
        {
            forceDirection = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.1f, 0.5f), // Slight upward force
                Random.Range(-1f, 1f)
            ).normalized;
        }

        // Apply the force
        rb.AddForce(forceDirection * force, ForceMode.Impulse);

        // Play a sound for object movement
        if (ghostAudioSource != null && objectMovementSounds.Length > 0)
        {
            AudioClip clip = objectMovementSounds[Random.Range(0, objectMovementSounds.Length)];
            ghostAudioSource.PlayOneShot(clip, Mathf.Min(1.0f, rb.mass / 10f));
        }
    }

    private void EndPossession()
    {
        isPossessingPlayer = false;

        // Signal to the player controller that possession is over
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.EndPossession();
        }

        // Return to suspicious state
        SetGhostState(GhostState.Suspicious);

        Debug.Log("Ghost has ended possession of the player");
    }

    private void PerformRandomGhostActivity()
    {
        // Choose a random activity based on current state
        float activityRoll = Random.value;

        switch (currentState)
        {
            case GhostState.Suspicious:
                if (activityRoll < 0.4f)
                    FlickerNearbyLight(false);
                else if (activityRoll < 0.7f)
                    MoveRandomObjectsNearPlayer(false);
                else
                    StartCoroutine(CreateWindEffect(1.5f, false));
                break;

            case GhostState.Defensive:
                if (activityRoll < 0.3f)
                    MoveObjectInPlayerPath();
                else if (activityRoll < 0.6f)
                    AffectNearbyDoor();
                else if (activityRoll < 0.9f)
                    FlickerNearbyLight(true);
                else
                    MoveRandomObjectsNearPlayer(true);
                break;

            case GhostState.Accepting:
                if (activityRoll < 0.5f)
                    HighlightClueObject();
                else
                    CreateSubtlePresence();
                break;
        }
    }

    private void PlayRandomStateSound()
    {
        if (ghostAudioSource == null) return;

        AudioClip[] currentStateSounds;

        // Choose sound array based on current state
        switch (currentState)
        {
            case GhostState.Suspicious:
                currentStateSounds = suspiciousSounds;
                break;

            case GhostState.Defensive:
                currentStateSounds = defensiveSounds;
                break;

            case GhostState.Revelatory:
                currentStateSounds = revelatoryDialogue;
                break;

            case GhostState.Accepting:
                currentStateSounds = acceptingSounds;
                break;

            default:
                return;
        }

        // If we have sounds for this state, play a random one
        if (currentStateSounds != null && currentStateSounds.Length > 0)
        {
            AudioClip clip = currentStateSounds[Random.Range(0, currentStateSounds.Length)];
            ghostAudioSource.PlayOneShot(clip);
        }
    }

    // Public methods for external access


    /// Call this when the player interacts with an emotionally significant object

    public void OnPlayerInteractWithEmotionalObject(string objectTag)
    {
        // Check if we know the emotional value of this tag
        if (emotionalTagValues.ContainsKey(objectTag))
        {
            float emotionalValue = emotionalTagValues[objectTag];

            // React based on emotional value
            if (emotionalValue > 0.8f)
            {
                // Highly significant - might trigger Revelatory state
                if (Random.value < emotionalValue * 0.7f && storyProgressionStage >= 1)
                {
                    SetGhostState(GhostState.Revelatory);
                }
                else
                {
                    // Otherwise be defensive
                    SetGhostState(GhostState.Defensive);
                }
            }
            else if (emotionalValue > 0.5f)
            {
                // Moderately significant
                if (currentState != GhostState.Defensive)
                {
                    SetGhostState(GhostState.Defensive);
                }

                // Create some wind or flickering
                if (Random.value < 0.5f)
                    StartCoroutine(CreateWindEffect(2f, false));
                else
                    FlickerNearbyLight(false);
            }
            else
            {
                // Low significance - minor reaction
                PlayRandomStateSound();
            }

            // Increment disturbance level, but cap it
            playerDisturbanceLevel = Mathf.Min(playerDisturbanceLevel + (emotionalValue * 0.2f), 1f);

            Debug.Log("Player interacted with emotional object: " + objectTag);
        }
    }


    /// Called when a puzzle is completed to trigger revelatory state and cutscene

    public void TriggerRevelatoryMoment(int progressStage)
    {
        storyProgressionStage = progressStage;
        SetGhostState(GhostState.Revelatory);
    }


    /// Called by the GameManager when a cutscene is complete

    public void ProcessCutsceneComplete()
    {
        // Called when a cutscene is finished
        EndPossession();

        // Advance ghost behavior based on story progression
        if (storyProgressionStage >= 3)
        {
            // After multiple revelations, ghost becomes more accepting
            SetGhostState(GhostState.Accepting);
        }
    }
}