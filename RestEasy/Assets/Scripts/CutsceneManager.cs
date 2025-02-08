using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    public GameObject player;
    [SerializeField] private PlayableDirector timeline;

    public void OnCutsceneComplete()
    {
        timeline.Stop();
        player.GetComponent<CharacterController>().enabled = true;
    }
}
