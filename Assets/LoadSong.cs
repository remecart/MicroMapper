using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSong : MonoBehaviour
{
    public string filePath; // Path to the Ogg Vorbis audio file

    public AudioSource audioSource;

    void Start() {
        StartCoroutine(GetAudio());
    }

    IEnumerator GetAudio() {
        string fileUrl = "file://" + filePath;
        using (var www = new WWW(fileUrl)) {
            yield return www;

            if (!string.IsNullOrEmpty(www.error)) {
                Debug.LogError("Error loading audio: " + www.error);
            }
            else {
                AudioClip clip = www.GetAudioClip(false, false, AudioType.OGGVORBIS);
                audioSource.clip = clip;
            }
        }
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