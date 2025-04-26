using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterBehavior : MonoBehaviour
{
    public GameObject finalCutscene;
    bool clicked;

    void Start()
    {
        if (!finalCutscene)
            Debug.LogWarning("No final cutscene assigned to play after letter is clicked");
    }

    void Update()
    {
        
    }

    void OnMouseDown()
    {
        if (clicked)
            return;
        
        // Time.timeScale = 0f;
        AudioManager.instance.ToggleSFX();
        
        finalCutscene.SetActive(true);
        
        EndDisplay display = finalCutscene.GetComponent<EndDisplay>();
        display.StartNarrative();

        this.enabled = false;
    }

}
