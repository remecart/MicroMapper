using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSoundManager : MonoBehaviour
{
    public static HitSoundManager instance;
    public List<Sound> hitSound;
    public GameObject soundSpawner;
    public int soundIndex;
    public int preloadSoundValue;
    private int index = 0;
    public float time;
    public List<float> soundTimestamps;

    private Queue<AudioSource> audioSourcePool;

    void Start()
    {
        instance = this;
        InitializeAudioSourcePool();
    }

    void Update()
    {
        // Preload sound timestamps if needed
        while (soundTimestamps.Count < preloadSoundValue && index < ObjectManager.instance.beats.Count)
        {
            List<colorNotes> notes = ObjectManager.instance.beats[index].colorNotes;
            foreach (var noteData in notes)
            {
                float timestamp = BeatCalculator.instance.GetRealTimeFromBeat(noteData.b);
                if (!soundTimestamps.Contains(timestamp))  // Avoid duplicate timestamps
                {
                    soundTimestamps.Add(timestamp);
                }
            }
            index++;
        }

        // Update current time based on beat time
        time = BeatCalculator.instance.GetRealTimeFromBeat(BeatCalculator.instance.currentBeatTime);

        // Check if it's time to play the next hit sound
        if (soundTimestamps.Count != 0 && time + hitSound[soundIndex].offset > soundTimestamps[0])
        {
            float playTime = soundTimestamps[0];
            soundTimestamps.RemoveAt(0);
            HitSound(playTime);

            // Remove any duplicate timestamps that match the current playTime
            soundTimestamps.RemoveAll(timestamp => timestamp == playTime);
        }
    }

    public void ClearSounds(int beat) {
        soundTimestamps.Clear();
        index = beat;
    }

    void InitializeAudioSourcePool()
    {
        audioSourcePool = new Queue<AudioSource>();
        for (int j = 0; j < preloadSoundValue; j++)
        {
            GameObject go = Instantiate(soundSpawner);
            go.transform.SetParent(this.transform);
            go.transform.localPosition = Vector3.zero;
            AudioSource audioSource = go.GetComponent<AudioSource>();
            audioSourcePool.Enqueue(audioSource);
            go.SetActive(false);
        }
    }

    public void HitSound(float playTime)
    {
        if (audioSourcePool.Count > 0)
        {
            AudioSource audioSource = audioSourcePool.Dequeue();
            audioSource.gameObject.SetActive(true);
            audioSource.clip = hitSound[soundIndex].sound;
            audioSource.PlayScheduled(AudioSettings.dspTime + (playTime - time));
            StartCoroutine(ReturnAudioSourceToPool(audioSource, audioSource.clip.length));
        }
        else
        {
            Debug.LogWarning("No available audio sources in the pool.");
        }
    }

    private IEnumerator ReturnAudioSourceToPool(AudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.gameObject.SetActive(false);
        audioSourcePool.Enqueue(audioSource);
    }
}

[System.Serializable]
public class Sound
{
    public AudioClip sound;
    public float offset;
}