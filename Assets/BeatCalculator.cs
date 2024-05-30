using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeatCalculator : MonoBehaviour
{
    public float currentBeat;
    public float currentBeatTime;
    public GameObject content;
    public BeatLinesRenderer beatLinesRenderer;
    public int gridDuration;
    public int pastGridDuration;

    public ObjectManager objectManager;
    public bool playing;
    public float bpm;

    public float editorScale;
    public LoadSong loadSong;
    public GameObject Grid;

    // Start is called before the first frame update
    void Start()
    {
        LoadNearbyObjects();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && playing == false) {
            PlayMap();
            currentBeatTime = currentBeat;
            playing = true;
            loadSong.Offset(GetRealTimeFrom(currentBeat));
        } else if (Input.GetKeyDown(KeyCode.Space) && playing == true) {
            playing = false;
            LoadNearbyObjects();
            loadSong.StopSong();
        }

        if (Input.mouseScrollDelta.y > 0 && playing == false) {
            currentBeat++;
            LoadNearbyObjects();
        } else if (Input.mouseScrollDelta.y < 0 && currentBeat > 0f && playing == false) {
            currentBeat--;
            LoadNearbyObjects();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playing) {
            currentBeatTime += Time.deltaTime * (GetBpmAtBeat(currentBeatTime) / 60f);
            
            if (currentBeatTime > currentBeat) {
                currentBeat++;
                PlayMap();
            }

            Grid.transform.position = new Vector3(Grid.transform.position.x, Grid.transform.position.y, Grid.transform.position.z + Time.deltaTime * editorScale);
        }
    }

    void LoadNearbyObjects() {
        foreach (Transform child in content.transform) {
            Destroy(child.gameObject);
        }
        
        int beat = Mathf.FloorToInt(currentBeat);
        beatLinesRenderer.DrawLines(currentBeat);
        beatLinesRenderer.updateNumbers(currentBeat);
        Grid.transform.position = new Vector3(Grid.transform.position.x, Grid.transform.position.y, Zpos(beat) * editorScale);

        for (int i = 0; i < gridDuration + pastGridDuration; i++) {
            if (beat - pastGridDuration + i >= 0) {
                List<colorNotes> notes = objectManager.beats[beat - pastGridDuration + i].colorNotes;
                for (int a = 0; a < notes.Count; a++) {
                    colorNotes note = notes[a];
                    GameObject currentNote = Instantiate(objectManager.objects[note.c]);
                    currentNote.transform.SetParent(content.transform);
                    currentNote.transform.localPosition = new Vector3(note.x, note.y, Zpos(note.b) * editorScale);
                    if (note.d != 8) {
                        currentNote.transform.rotation = Quaternion.Euler(0, 0, Rotation(note.d));
                        currentNote.transform.GetChild(1).gameObject.SetActive(false);
                    }
                    else currentNote.transform.GetChild(0).gameObject.SetActive(false);
                }

                List<bombNotes> bombs = objectManager.beats[beat - pastGridDuration + i].bombNotes;
                for (int a = 0; a < bombs.Count; a++) {
                    bombNotes bomb = bombs[a];
                    GameObject currentBomb = Instantiate(objectManager.objects[2]);
                    currentBomb.transform.SetParent(content.transform);
                    currentBomb.transform.localPosition = new Vector3(bomb.x, bomb.y, Zpos(bomb.b) * editorScale);
                }

                List<bpmEvents> bpmEvents = objectManager.beats[beat - pastGridDuration + i].bpmEvents;
                for (int a = 0; a < bpmEvents.Count; a++) {
                    bpmEvents bpm = bpmEvents[a];
                    GameObject bpmEvent = Instantiate(objectManager.objects[3]);
                    bpmEvent.transform.SetParent(content.transform);
                    bpmEvent.transform.localPosition = new Vector3(8f, 0, Zpos(bpm.b) * editorScale);
                    bpmEvent.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = bpm.m.ToString();
                }
            }
        }
    }

    void PlayMap() {
        foreach (Transform child in content.transform) {
            if (child.transform.position.z < -pastGridDuration) Destroy(child.gameObject);
        }
        
        int beat = Mathf.FloorToInt(currentBeat);
        int spawnBeat = beat + gridDuration;
        beatLinesRenderer.DrawLines(currentBeat);
        beatLinesRenderer.updateNumbersInPlay(currentBeat);
        
        List<colorNotes> notes = objectManager.beats[spawnBeat].colorNotes;
        for (int a = 0; a < notes.Count; a++) {
            colorNotes note = notes[a];
            GameObject currentNote = Instantiate(objectManager.objects[note.c]);
            currentNote.transform.SetParent(content.transform);
            currentNote.transform.localPosition = new Vector3(note.x, note.y, Zpos(note.b) * editorScale);
            if (note.d != 8) {
                currentNote.transform.rotation = Quaternion.Euler(0, 0, Rotation(note.d));
                currentNote.transform.GetChild(1).gameObject.SetActive(false);
            }
            else currentNote.transform.GetChild(0).gameObject.SetActive(false);
        }

        List<bombNotes> bombs = objectManager.beats[spawnBeat].bombNotes;
        for (int a = 0; a < bombs.Count; a++) {
            bombNotes bomb = bombs[a];
            GameObject currentBomb = Instantiate(objectManager.objects[2]);
            currentBomb.transform.SetParent(content.transform);
            currentBomb.transform.localPosition = new Vector3(bomb.x, bomb.y, Zpos(bomb.b) * editorScale);
        }

        List<bpmEvents> bpmEvents = objectManager.beats[spawnBeat].bpmEvents;
        for (int a = 0; a < bpmEvents.Count; a++) {
            bpmEvents bpmChange = bpmEvents[a];
            GameObject bpmEvent = Instantiate(objectManager.objects[3]);
            bpmEvent.transform.SetParent(content.transform);
            bpmEvent.transform.localPosition = new Vector3(8f, 0, Zpos(bpmChange.b) * editorScale);
            bpmEvent.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = bpmChange.m.ToString();
        }
    }

    public int Rotation(int level) {
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

    public float Zpos(float beat) {

        List<bpmEvents> bpms = objectManager.bpmEvents;
        float value = 0;
        int number = 0;

        float position = 0;

        for (int i = 0; i < bpms.Count; i++) {
            if (i + 1 < bpms.Count) {
                if (bpms[i + 1].b - value <= beat) {
                    float duration = bpms[i + 1].b - bpms[i].b;
                    value += duration;
                    position += + duration * (60f / bpms[i].m);
                    number += 1;
                }
            }
        }
        
        position += (beat - bpms[number].b) * (60f / bpms[number].m);
        return position;
    }

    public float BeatFromPosition(float position) {
        List<bpmEvents> bpms = objectManager.bpmEvents;
        float _beat = 0;
        float accumulatedPosition = 0;

        for (int i = 0; i < bpms.Count; i++) {
            if (i + 1 < bpms.Count) {
                float nextBeatTime = bpms[i + 1].b;
                float duration = nextBeatTime - bpms[i].b;
                float sectionPosition = duration * (60f / bpms[i].m);

                if (accumulatedPosition + sectionPosition >= position) {
                    float remainingPosition = position - accumulatedPosition;
                    _beat = bpms[i].b + (remainingPosition / (60f / bpms[i].m));
                    return _beat;
                } else {
                    accumulatedPosition += sectionPosition;
                }
            } else {
                // If we are at the last bpm event, we calculate based on the remaining position
                float remainingPosition = position - accumulatedPosition;
                _beat = bpms[i].b + (remainingPosition / (60f / bpms[i].m));
                Debug.Log(_beat + " - " + position);
                return _beat;
           }
        }
        Debug.Log(_beat + " - " + position);
        return _beat; // In case position exceeds the total length covered by bpmEvents
    }

    public float GetBpmAtBeat(float beat) {

        List<bpmEvents> bpms = objectManager.bpmEvents;
        float value = bpm;

        for (int i = 0; i < bpms.Count; i++) {
            if (bpms[i].b <= beat) {
                value = bpms[i].m;
            }
        }
        return value;
    }

    public float GetRealTimeFrom(float beat) {

        List<bpmEvents> bpms = objectManager.bpmEvents;
        float value = 0;
        int number = 0;

        float realTime = 0;

        for (int i = 0; i < bpms.Count; i++) {
            if (i + 1 < bpms.Count) {
                if (bpms[i + 1].b - value <= beat) {
                    float duration = bpms[i + 1].b - bpms[i].b;
                    value += duration;
                    realTime += + duration * (60f / bpms[i].m);
                    number += 1;
                }
            }
        }

        realTime += (beat - bpms[number].b) * (60f / bpms[number].m);
        return realTime;
    }
}
