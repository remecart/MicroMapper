using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeatCalculator : MonoBehaviour
{
    public static BeatCalculator instance;
    public BeatLinesRenderer beatLinesRenderer;
    public float bpm;
    public int currentBeat;
    public float currentTimeScroll;
    public float currentBeatTime;
    public int spawnOffset;
    public int despawnOffset;
    public int editorScale;
    public LoadSong loadSong;
    public Transform Grid;
    public Transform content;
    public bool playing;
    private float startTime;

    void Start()
    {
        instance = this;
        bpm = MapManager.instance.mapInfo._beatsPerMinute;
    }

    void Update()
    {
        if (Input.mouseScrollDelta.y > 0 && !playing)
        {
            currentBeat++;
            ScrollObjects(currentBeat, 1);
        }
        else if (Input.mouseScrollDelta.y < 0 && currentBeat > 0 && !playing)
        {
            currentBeat--;
            ScrollObjects(currentBeat, -1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!playing)
            {
                HitSoundManager.instance.ClearSounds(currentBeat);
                LoadObjects(currentBeat);
                currentBeatTime = currentBeat;
                startTime = Time.unscaledTime - GetRealTimeFromBeat(currentBeat);
                playing = true;
                loadSong.Offset(GetRealTimeFromBeat(currentBeat));
            }
            else
            {
                playing = false;
                currentBeat = Mathf.RoundToInt(currentBeatTime);
                LoadObjects(currentBeat);
                loadSong.StopSong();
            }
        }

        if (playing)
        {
            // Sync currentBeatTime with the actual elapsed time based on the music start time
            float elapsedTime = Time.unscaledTime - startTime;
            currentBeatTime = BeatFromRealTime(elapsedTime);

            if (currentBeatTime >= currentBeat + 1)
            {
                currentBeat = Mathf.FloorToInt(currentBeatTime);
                ScrollObjects(currentBeat, 1);
            }

            Grid.localPosition = new Vector3(0, 0, PositionFromBeat(currentBeatTime) * editorScale);
        }
    }

    void LoadObjects(int beat)
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }

        Grid.localPosition = new Vector3(0, 0, PositionFromBeat(beat) * editorScale);
        beatLinesRenderer.DrawLines(currentBeat);
        beatLinesRenderer.updateNumbers(currentBeat);

        for (int i = 0; i < spawnOffset + despawnOffset + 1; i++)
        {
            if (beat - despawnOffset + i >= 0)
            {
                SpawnObjectsAtBeat(beat - despawnOffset + i);
            }
        }
    }

    void ScrollObjects(int beat, int scrollOffset)
    {
        Grid.localPosition = new Vector3(0, 0, PositionFromBeat(beat + scrollOffset) * editorScale);
        if (Grid.localPosition.z < 0) Grid.localPosition = new Vector3(0, 0, 0);

        beatLinesRenderer.DrawLines(beat);
        beatLinesRenderer.updateNumbersInScroll(beat, scrollOffset);
        
        if (scrollOffset == 1)
        {
            foreach (Transform child in content.transform)
            {
                if (child.transform.position.z < PositionFromBeat(beat - despawnOffset) * editorScale)
                    Destroy(child.gameObject);
            }
            int spawnBeat = beat + spawnOffset;
            if (spawnBeat >= 0) SpawnObjectsAtBeat(spawnBeat);
        }
        else if (scrollOffset == -1)
        {
            foreach (Transform child in content.transform)
            {
                if (child.transform.position.z > PositionFromBeat(beat + spawnOffset) * editorScale)
                    Destroy(child.gameObject);
            }
            int spawnBeat = beat - despawnOffset;
            if (spawnBeat >= 0) SpawnObjectsAtBeat(spawnBeat);
        }
    }

    void SpawnObjectsAtBeat(int beat)
    {
        // Load Note Objects
        List<colorNotes> notes = ObjectManager.instance.beats[beat].colorNotes;
        foreach (var noteData in notes)
        {
            GameObject note = Instantiate(ObjectManager.instance.objects[noteData.c]);
            note.transform.SetParent(content);
            note.transform.localPosition = new Vector3(noteData.x, noteData.y, PositionFromBeat(noteData.b) * editorScale);
            if (noteData.d != 8)
            {
                note.transform.rotation = Quaternion.Euler(0, 0, Rotation(noteData.d));
                note.transform.GetChild(1).gameObject.SetActive(false);
            }
            else note.transform.GetChild(0).gameObject.SetActive(false);
        }

        // Load Bomb Objects
        List<bombNotes> bombs = ObjectManager.instance.beats[beat].bombNotes;
        foreach (var bombData in bombs)
        {
            GameObject bomb = Instantiate(ObjectManager.instance.objects[2]);
            bomb.transform.SetParent(content);
            bomb.transform.localPosition = new Vector3(bombData.x, bombData.y, PositionFromBeat(bombData.b) * editorScale);
        }

        // Load Bpm Objects
        List<bpmEvents> bpmChanges = ObjectManager.instance.beats[beat].bpmEvents;
        foreach (var bpmChangeData in bpmChanges)
        {
            GameObject bpmChange = Instantiate(ObjectManager.instance.objects[3]);
            bpmChange.transform.SetParent(content);
            bpmChange.transform.localPosition = new Vector3(8, 0, PositionFromBeat(bpmChangeData.b) * editorScale);
            bpmChange.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = bpmChangeData.m.ToString();
        }
    }

    public int Rotation(int level)
    {
        return level switch
        {
            0 => 180,
            1 => 0,
            2 => 270,
            3 => 90,
            4 => 225,
            5 => 135,
            6 => 315,
            7 => 45,
            _ => 0
        };
    }

    public float PositionFromBeat(float beat)
    {
        List<bpmEvents> bpms = ObjectManager.instance.bpmEvents;
        float position = 0;
        float previousBpm = bpm;
        float previousBeat = 0f;

        foreach (var bpmEvent in bpms)
        {
            if (beat <= bpmEvent.b)
            {
                break;
            }

            float duration = bpmEvent.b - previousBeat;
            position += duration * (60f / previousBpm);

            previousBpm = bpmEvent.m;
            previousBeat = bpmEvent.b;
        }

        float remainingDuration = beat - previousBeat;
        position += remainingDuration * (60f / previousBpm);

        return position;
    }

    public float BeatFromPosition(float position)
    {
        List<bpmEvents> bpms = ObjectManager.instance.bpmEvents;
        float _beat = 0;
        float accumulatedPosition = 0;
        for (int i = 0; i < bpms.Count; i++)
        {
            if (i + 1 < bpms.Count)
            {
                float nextBeatTime = bpms[i + 1].b;
                float duration = nextBeatTime - bpms[i].b;
                float sectionPosition = duration * (60f / bpms[i].m);

                if (accumulatedPosition + sectionPosition >= position)
                {
                    float remainingPosition = position - accumulatedPosition;
                    _beat = bpms[i].b + (remainingPosition / (60f / bpms[i].m));
                    return _beat;
                }
                else
                {
                    accumulatedPosition += sectionPosition;
                }
            }
            else
            {
                float remainingPosition = position - accumulatedPosition;
                _beat = bpms[i].b + (remainingPosition / (60f / bpms[i].m));
                Debug.Log(_beat + " - " + position);
                return _beat;
            }
        }
        Debug.Log(_beat + " - " + position);
        return _beat;
    }

    public float GetBpmAtBeat(float beat)
    {
        List<bpmEvents> bpms = ObjectManager.instance.bpmEvents;
        float value = bpm;

        for (int i = 0; i < bpms.Count; i++)
        {
            if (bpms[i].b > beat)
            {
                break;
            }
            value = bpms[i].m;
        }
        return value;
    }

    public float GetRealTimeFromBeat(float beat)
    {
        List<bpmEvents> bpms = ObjectManager.instance.bpmEvents;
        float realTime = 0;
        float previousBpm = bpm;
        float previousBeat = 0;

        foreach (var bpmEvent in bpms)
        {
            if (beat <= bpmEvent.b)
            {
                realTime += (beat - previousBeat) * (60f / previousBpm);
                return realTime;
            }
            realTime += (bpmEvent.b - previousBeat) * (60f / previousBpm);
            previousBpm = bpmEvent.m;
            previousBeat = bpmEvent.b;
        }
        realTime += (beat - previousBeat) * (60f / previousBpm);
        return realTime;
    }


    public float BeatFromRealTime(float realTime)
    {
        List<bpmEvents> bpms = ObjectManager.instance.bpmEvents;
        float beat = 0;
        float accumulatedTime = 0;

        for (int i = 0; i < bpms.Count; i++)
        {
            if (i + 1 < bpms.Count)
            {
                float nextBeatTime = bpms[i + 1].b;
                float bpmDuration = (nextBeatTime - bpms[i].b) * (60f / bpms[i].m);

                if (accumulatedTime + bpmDuration >= realTime)
                {
                    float remainingTime = realTime - accumulatedTime;
                    beat = bpms[i].b + (remainingTime / (60f / bpms[i].m));
                    return beat;
                }
                else
                {
                    accumulatedTime += bpmDuration;
                }
            }
            else
            {
                float remainingTime = realTime - accumulatedTime;
                beat = bpms[i].b + (remainingTime / (60f / bpms[i].m));
                return beat;
            }
        }

        return beat;
    }
}
