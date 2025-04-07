using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake() {
        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } 
        else 
        {
            Destroy(gameObject);
        }
    }

    /*private void Start() {
        PlayMusic("MainTheme");
    }*/

    public float GetLengthOfMusicClip(string name) {
        Sound s = Array.Find(musicSounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound " + name + " not found");
            return 0;
        }

        return s.clip.length;
    }

    public float GetLengthOfSFXClip(string name) {
        Sound s = Array.Find(sfxSounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound " + name + " not found");
            return 0;
        }

        return s.clip.length;
    }

    public void PlayMusic(string name, float delay = 0) {
        Sound s = Array.Find(musicSounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound " + name + " not found");
            return;
        }

        StartCoroutine(PlayMusicDelayed(s, delay));
    }

    private IEnumerator PlayMusicDelayed(Sound s, float delay) {
        yield return new WaitForSeconds(delay);
        musicSource.clip = s.clip;
        musicSource.Play();
    }

    public void PlaySFX(string name, float delay = 0) {
        Sound s = Array.Find(sfxSounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound " + name + " not found");
            return;
        }
        StartCoroutine(PlaySFXDelayed(s, delay));
    }

    private IEnumerator PlaySFXDelayed(Sound s, float delay) {
        yield return new WaitForSeconds(delay);
        sfxSource.PlayOneShot(s.clip);
    }

    public void ToggleMusic() {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX() {
        sfxSource.mute = !sfxSource.mute;
    }

    public void SetMusicVolume(float volume) {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume) {
        sfxSource.volume = volume;
    }
}
