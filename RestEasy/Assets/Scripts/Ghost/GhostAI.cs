using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAI : MonoBehaviour
{
    [System.Serializable]
    public class EmotionalTagData
    {
        public string tag;
        public float emotionalValue;
        [Tooltip("Multiplier for force when in defensive state")]
        public float defensiveForceMultiplier = 1.5f;
    }

    [Header("References")]
    [SerializeField] private AudioSource ghostAudioSource;
    [SerializeField] private GameManager gameManager;

    [Header("Ghost State Variables")]
    [SerializeField] private GhostState currentState = GhostState.Suspicious;
    [SerializeField] private float stateChangeTimer;
    [SerializeField] private float stateChangeInterval = 10f;
    [SerializeField] private float activityRadius = 8f;

    [Header("Interaction")]
    [SerializeField] private float smallObjectPushForce = 5f;
    [SerializeField] private float largeObjectPushForce = 10f;
    [SerializeField] private float maxMovableObjectMass = 20f;
    [SerializeField] private float flickeringLightIntensity = 0.3f;
    [SerializeField] private float windIntensity = 1f;
    [SerializeField] private float disturbanceDecayRate = 0.05f;

    [Header("Audio")]
    [SerializeField] private AudioClip[] suspiciousSounds;
    [SerializeField] private AudioClip[] defensiveSounds;
    [SerializeField] private AudioClip[] revelatoryDialogue;
    [SerializeField] private AudioClip[] acceptingSounds;
    [SerializeField] private AudioClip[] objectMovementSounds;
    [SerializeField] private AudioClip windSound;
    [SerializeField] private AudioClip heartbeatSound;

    [Header("Key Progression")]
    [SerializeField] private int keysCollected = 0;
    [SerializeField] private int keysForPartialAcceptance = 3;
    [SerializeField] private int keysForFullAcceptance = 4;
    [SerializeField] private GameObject sparkleEffectPrefab;
    [SerializeField] private float sparkleChance = 0.3f;
    

    [Header("Emotional Tags")]
    [SerializeField] private List<EmotionalTagData> emotionalTags = new List<EmotionalTagData>();

    [Header("Sound Control")]
    [SerializeField] private float minTimeBetweenSounds = 5f; // Minimum time between ghost sounds
    [SerializeField] private float soundChance = 0.5f;
    private Transform player;
    private Light[] lightsInScene;
    private GhostState previousState;
    private float timeSinceLastActivity = 0f;
    private bool isPossessingPlayer = false;
    private int storyProgressionStage = 0;
    private float playerDisturbanceLevel = 0f;
    private Dictionary<string, float> emotionalTagValues = new Dictionary<string, float>();
    private Dictionary<string, float> defensiveForceMultipliers = new Dictionary<string, float>();
    private float disturbanceMultiplier = 1.0f;
    private List<GameObject> activeSparkleEffects = new List<GameObject>();
    public static GhostAI Instance { get; private set; }
    private bool isActivated = false;
    private float activationTimer = 180f;
    private float timeSinceLastSound = 0f;
    private bool inCutscene = false;

    public enum GhostState
    {
        Suspicious,
        Defensive,
        Revelatory,
        Accepting
    }

    private void Awake()
    {
        // Keep the singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initial setup but don't start behaviors yet
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager not found in the scene. Required for ghost interactions.");
            }
        }

        lightsInScene = FindObjectsOfType<Light>();
        InitializeEmotionalTagValues();

        // Set to inactive at start - we'll activate after the timer
        isActivated = false;
        Debug.Log("Ghost AI will activate in 20 seconds...");
    }

    private void InitializeEmotionalTagValues()
    {
        emotionalTagValues.Clear();
        defensiveForceMultipliers.Clear();

        foreach (var tagData in emotionalTags)
        {
            if (!string.IsNullOrEmpty(tagData.tag))
            {
                emotionalTagValues[tagData.tag] = Mathf.Clamp01(tagData.emotionalValue);
                defensiveForceMultipliers[tagData.tag] = tagData.defensiveForceMultiplier;
            }
        }

        Debug.Log("Initialized Emotional Tags:");
        foreach (var tag in emotionalTagValues)
        {
            Debug.Log($"Tag: {tag.Key}, Emotional Value: {tag.Value:F2}");
        }
    }

    private void Update()
    {
        // Check if we're in a cutscene - don't run ghost behaviors
        if (inCutscene)
            return;

        // Update sound timer
        timeSinceLastSound += Time.deltaTime;

        // Check if we need to activate the ghost
        if (!isActivated)
        {
            activationTimer -= Time.deltaTime;
            if (activationTimer <= 0f)
            {
                isActivated = true;
                SetGhostState(GhostState.Suspicious);
                Debug.Log("Ghost AI has been activated!");
            }
            return; // Skip the rest of the update until activated
        }

        // Regular update code - only runs after activation
        timeSinceLastActivity += Time.deltaTime;

        stateChangeTimer -= Time.deltaTime;
        if (stateChangeTimer <= 0f)
        {
            if (currentState != GhostState.Revelatory)
            {
                ConsiderStateChange();
            }

            stateChangeTimer = stateChangeInterval;
        }

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

        if (timeSinceLastActivity > Random.Range(15f, 30f))
        {
            PerformRandomGhostActivity();
        }

        playerDisturbanceLevel = Mathf.Max(playerDisturbanceLevel - (Time.deltaTime * disturbanceDecayRate), 0f);
    }

    private void SetGhostState(GhostState newState)
    {
        Debug.Log($"Attempting to change state. Current: {currentState}, Requested: {newState}, Disturbance: {playerDisturbanceLevel}");

        if (newState == currentState)
        {
            Debug.Log("State change blocked - already in this state");
            return;
        }

        previousState = currentState;
        currentState = newState;

        PlayRandomStateSound();

        Debug.Log("Ghost state changed from " + previousState + " to " + currentState);
    }

    private void ConsiderStateChange()
    {
        // If fully accepting (all keys), stay in accepting state
        if (keysCollected >= keysForFullAcceptance)
        {
            SetGhostState(GhostState.Accepting);
            return;
        }

        // If partially accepting but disturbance is high, become defensive
        if (keysCollected >= keysForPartialAcceptance && playerDisturbanceLevel > 0.7f)
        {
            SetGhostState(GhostState.Defensive);
            return;
        }

        // If partially accepting and disturbance is moderate, maintain accepting
        if (keysCollected >= keysForPartialAcceptance && playerDisturbanceLevel < 0.5f)
        {
            SetGhostState(GhostState.Accepting);
            return;
        }

        // Otherwise use normal logic
        if (playerDisturbanceLevel > 0.7f)
        {
            SetGhostState(GhostState.Defensive);
            return;
        }

        // If no specific condition is met, default to Suspicious
        SetGhostState(GhostState.Suspicious);
    }

    private void HandleSuspiciousState()
    {
        if (Random.value < 0.01f)
        {
            MoveRandomObjectsNearPlayer(false);
        }

        if (Random.value < 0.005f)
        {
            FlickerNearbyLight(false);
        }

        if (Random.value < 0.002f)
        {
            StartCoroutine(CreateWindEffect(1.5f, false));
        }
    }

    private void HandleDefensiveState()
    {
        if (Random.value < 0.03f)
        {
            MoveRandomObjectsNearPlayer(true);
        }

        if (Random.value < 0.01f)
        {
            FlickerNearbyLight(true);
        }

        if (Random.value < 0.008f)
        {
            MoveObjectInPlayerPath();
        }

        if (Random.value < 0.01f)
        {
            AffectNearbyDoor();
        }

    }

    private void HandleRevelatoryState()
    {
        if (!isPossessingPlayer)
        {
            StartCoroutine(PossessionSequence());
        }
    }

private IEnumerator PossessionSequence()
    {
        isPossessingPlayer = true;

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.BecomePossessed();
        }

        // Play heartbeat sound when getting possessed
        AudioManager.instance.PlaySFX("heartbeat");

        yield return new WaitForSeconds(2.5f);

        // Hide cursor for cutscene
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Stop any sounds
        if (ghostAudioSource != null)
        {
            ghostAudioSource.Stop();
        }

        gameManager.OnPuzzleCompleted();

        timeSinceLastActivity = 0f;
        Debug.Log("Ghost has possessed the player and triggered cutscene");
    }
    private void HandleAcceptingState()
    {
        // Clean up any old sparkle effects
        CleanupSparkleEffects();

        if (Random.value < 0.01f)
        {
            CreateSubtlePresence();
        }

        // Add chance to create sparkle effects on interactables
        if (Random.value < sparkleChance)
        {
            SparkleRandomInteractableObject();
        }
    }
    private void SparkleRandomInteractableObject()
    {
        // Find objects tagged as interactable near the player
        Collider[] nearbyObjects = Physics.OverlapSphere(player.position, activityRadius * 1.5f);
        List<GameObject> interactableObjects = new List<GameObject>();

        foreach (Collider col in nearbyObjects)
        {
            // Check for tags that indicate interactable objects
            if (col.CompareTag("Collectible") || col.CompareTag("EmotionalObject"))
            {
                interactableObjects.Add(col.gameObject);
            }
        }

        if (interactableObjects.Count == 0) return;

        // Choose a random object to highlight
        GameObject objectToSparkle = interactableObjects[Random.Range(0, interactableObjects.Count)];

        // Create sparkle effect
        if (sparkleEffectPrefab != null)
        {
            // Calculate position: Use the object's position as a base
            Vector3 sparklePosition = objectToSparkle.transform.position;

            // Add an upward offset (in world space)
            sparklePosition.y += Random.Range(0.3f, 0.5f);

            // Add small random horizontal offsets (in world space)
            sparklePosition.x += Random.Range(-0.2f, 0.2f);
            sparklePosition.z += Random.Range(-0.2f, 0.2f);

            // Create the sparkle effect
            GameObject sparkle = Instantiate(sparkleEffectPrefab, sparklePosition, Quaternion.Euler(-90, 0, 0));

            // Ensure the rotation is locked and doesn't change
            if (sparkle.GetComponent<Rigidbody>())
            {
                sparkle.GetComponent<Rigidbody>().freezeRotation = true;
            }
            // Add to list for tracking
            activeSparkleEffects.Add(sparkle);

            // Let the particle system play naturally and destroy when done
            ParticleSystem ps = sparkle.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                float duration = ps.main.duration + ps.main.startLifetimeMultiplier;
                StartCoroutine(RemoveSparkleWhenDone(sparkle, duration));
            }

            Debug.Log($"Ghost created sparkle effect on {objectToSparkle.name}");
        }
    }

    // Cleanup method for sparkle effects
    private void CleanupSparkleEffects()
    {
        // Just remove null entries from the list
        activeSparkleEffects.RemoveAll(effect => effect == null);
    }


    // Coroutine to remove sparkle effects after a delay
    private IEnumerator RemoveSparkleEffect(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (effect != null)
        {
            activeSparkleEffects.Remove(effect);
            Destroy(effect);
        }
    }

    private IEnumerator RemoveSparkleWhenDone(GameObject effect, float duration)
    {
        // Wait for the particle system to finish
        yield return new WaitForSeconds(duration);

        if (effect != null)
        {
            activeSparkleEffects.Remove(effect);
            Destroy(effect);
        }
    }

    private void MoveRandomObjectsNearPlayer(bool intense)
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(player.position, activityRadius);
        List<Rigidbody> movableObjects = new List<Rigidbody>();

        foreach (Collider col in nearbyObjects)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null && !rb.isKinematic && rb.mass <= maxMovableObjectMass)
            {
                movableObjects.Add(rb);
            }
        }

        if (movableObjects.Count == 0) return;

        int objectsToAffect = intense ? Mathf.Min(3, movableObjects.Count) : Mathf.Min(1, movableObjects.Count);

        for (int i = 0; i < objectsToAffect; i++)
        {
            if (movableObjects.Count > 0)
            {
                int index = Random.Range(0, movableObjects.Count);
                Rigidbody rb = movableObjects[index];

                string objectTag = movableObjects[index].gameObject.tag;

                float force = intense ? largeObjectPushForce : smallObjectPushForce;
                force = force * (1f - (rb.mass / maxMovableObjectMass) * 0.8f);

                ApplyForceToObject(rb, force, null, objectTag);

                movableObjects.RemoveAt(index);
            }
        }

        timeSinceLastActivity = 0f;
        Debug.Log("Ghost moved objects near player");
    }

    private void ApplyForceToObject(Rigidbody rb, float force, Vector3? customDirection = null, string tag = null)
    {
        if (rb == null) return;

        if (currentState == GhostState.Defensive && !string.IsNullOrEmpty(tag) &&
            defensiveForceMultipliers.TryGetValue(tag, out float multiplier))
        {
            force *= multiplier;
            Debug.Log($"Applying defensive force multiplier for tag {tag}. New force: {force:F2}");
        }

        Vector3 forceDirection;
        if (customDirection.HasValue)
        {
            forceDirection = customDirection.Value;
        }
        else
        {
            forceDirection = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.1f, 0.5f),
                Random.Range(-1f, 1f)
            ).normalized;
        }

        rb.AddForce(forceDirection * force, ForceMode.Impulse);

        // Play object movement sound if not in cutscene and sound timers allow
        if (!inCutscene && ShouldPlaySound())
        {
            AudioManager.instance.PlaySFX("object-movement");
        }
    }

    public void OnPlayerInteractWithEmotionalObject(string objectTag)
    {
        if (emotionalTagValues.ContainsKey(objectTag))
        {
            float emotionalValue = emotionalTagValues[objectTag] * disturbanceMultiplier;

            Debug.Log($"Interacted with Emotional Object: {objectTag}");
            Debug.Log($"Emotional Value (after multiplier): {emotionalValue:F2}");

            // Only get upset if disturbance multiplier isn't zero
            if (disturbanceMultiplier > 0)
            {
                // High emotional value objects trigger defensive state and increase disturbance
                if (emotionalValue > 0.8f)
                {
                    Debug.Log("High Emotional Value Detected - Becoming Defensive");
                    SetGhostState(GhostState.Defensive);
                    playerDisturbanceLevel = Mathf.Min(playerDisturbanceLevel + emotionalValue, 1f);
                }
                // Medium emotional value objects create moderate disturbance
                else if (emotionalValue > 0.5f)
                {
                    Debug.Log("Medium Emotional Value Detected - Moderate Disturbance");
                    playerDisturbanceLevel = Mathf.Min(playerDisturbanceLevel + emotionalValue, 1f);
                }
                // Low emotional value objects have minimal impact
                else
                {
                    Debug.Log("Low Emotional Value Detected - Minimal Disturbance");
                    playerDisturbanceLevel = Mathf.Min(playerDisturbanceLevel + (emotionalValue), 1f);
                }

                Debug.Log($"Updated Disturbance Level: {playerDisturbanceLevel:F2}");
            }
            else
            {
                Debug.Log("Ghost is fully accepting - no disturbance caused");
            }
        }
    }

    public void TriggerRevelatoryMoment(int progressStage)
    {
        // Set the progression stage
        storyProgressionStage = progressStage;

        Debug.Log($"Entering Revlatory");


        // Force Revelatory state - this happens BEFORE the cutscene
        SetGhostState(GhostState.Revelatory);

        // Immediately trigger the cutscene
        gameManager.OnPuzzleCompleted();
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
        List<DoorBehavior> nearbyDoors = new List<DoorBehavior>();

        foreach (Collider col in nearbyObjects)
        {
            // Check if this is a door (has DoorBehavior component)
            DoorBehavior doorBehavior = col.GetComponent<DoorBehavior>();
            if (doorBehavior == null)
            {
                doorBehavior = col.GetComponentInParent<DoorBehavior>();
            }

            if (doorBehavior != null)
            {
                nearbyDoors.Add(doorBehavior);
            }
        }

        if (nearbyDoors.Count == 0)
        {
            Debug.Log("No doors found nearby to slam");
            return;
        }

        // Choose a random door to slam
        DoorBehavior doorToSlam = nearbyDoors[Random.Range(0, nearbyDoors.Count)];
        doorToSlam.SlamDoor();

        // Play sound (optional, as DoorBehavior already plays a sound)
        if (ghostAudioSource != null && objectMovementSounds.Length > 0)
        {
            AudioClip clip = objectMovementSounds[Random.Range(0, objectMovementSounds.Length)];
            ghostAudioSource.PlayOneShot(clip, 0.7f);
        }

        timeSinceLastActivity = 0f;
        Debug.Log("Ghost slammed a door: " + doorToSlam.name);
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
        if (!inCutscene && ShouldPlaySound())
        {
            AudioManager.instance.PlaySFX("wind");
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

    public void ProcessCutsceneComplete()
    {
        // End possession
        EndPossession();

        // Return cursor to previous state (if you want it locked but visible)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

        // Determine next state based on progression
        if (storyProgressionStage >= 3)
        {
            // If all major puzzles are completed, become accepting
            SetGhostState(GhostState.Accepting);
        }
        else
        {
            // Otherwise, return to suspicious state
            SetGhostState(GhostState.Suspicious);
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

        // Return cursor to previous state
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

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

    private bool ShouldPlaySound()
    {
        // Check if enough time has passed since the last sound
        if (timeSinceLastSound < minTimeBetweenSounds)
            return false;

        // Use soundChance to determine if sound should play
        if (Random.value > soundChance)
            return false;

        // Reset the sound timer when we decide to play a sound
        timeSinceLastSound = 0f;
        return true;
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

    // Update this method to track key collection
    public void OnKeyCollected()
    {
        keysCollected++;
        Debug.Log($"Ghost senses {keysCollected} keys collected");

        // Check for state progression
        if (keysCollected >= keysForFullAcceptance)
        {
            // Fully accepting - no more getting upset
            disturbanceMultiplier = 0f;
            SetGhostState(GhostState.Accepting);
        }
        else if (keysCollected >= keysForPartialAcceptance)
        {
            // Keep the disturbance multiplier at 1.0 (unchanged)
            // This means emotional objects will have the same impact
            disturbanceMultiplier = 1.0f;
            SetGhostState(GhostState.Accepting);
        }
    }
}