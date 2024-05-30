using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadSong : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        MapManager.instance.SetAudioClip(audioSource);
    }


    public void Offset(float offset) {
        if (audioSource.clip != null) {
            if (offset >= 0 && offset <= audioSource.clip.length) {
                audioSource.time = offset;
                audioSource.Play();
            }
            else {
                Debug.LogWarning("Invalid Offset");
            }
        }
        else {
            Debug.LogWarning("No audio clip loaded.");
        }
    }

    public void StopSong() {
        audioSource.Stop();
    }
}