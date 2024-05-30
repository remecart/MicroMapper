using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ObjectManager : MonoBehaviour
{
    public string Path;
    public List<GameObject> objects;
    public Transform parent;
    public enum _difficulty {
        Easy,
        Normal,
        Hard,
        Expert,
        ExpertPlus
    } 

    public enum _beatmapCharacteristicName {
        Standard,
        Lawless
    } 

    public _difficulty diff;
    public _beatmapCharacteristicName beatchar;

    public List<beats> beats;
    public List<bpmEvents> bpmEvents;
    public BeatCalculator beatCalculator;

    // Start is called before the first frame update
    void Start() {
        LoadMap();
    }

    // Update is called once per frame
    void LoadMap() {
        string rawData = File.ReadAllText(Path + "\\" + diff.ToString() + beatchar.ToString() + ".dat");
        beats beat = JsonUtility.FromJson<beats>(rawData);

        for (int i = 0; i < beat.colorNotes.Count; i++) {
            int b = Mathf.FloorToInt(beat.colorNotes[i].b);
            beats[b].colorNotes.Add(beat.colorNotes[i]);
        }

        for (int i = 0; i < beat.bombNotes.Count; i++) {
            int b = Mathf.FloorToInt(beat.bombNotes[i].b);
            beats[b].bombNotes.Add(beat.bombNotes[i]);
        }

        bpmEvents bpm = new bpmEvents();
        bpm.b = 0;
        bpm.m = beatCalculator.bpm;
        bpmEvents.Add(bpm);

        for (int i = 0; i < beat.bpmEvents.Count; i++) {
            int b = Mathf.FloorToInt(beat.bpmEvents[i].b);
            beats[b].bpmEvents.Add(beat.bpmEvents[i]);
            bpmEvents.Add(beat.bpmEvents[i]);
        }

        // Old Code my beloved o7
        //
        //  foreach (var note in beat.colorNotes) {
        //      GameObject currentNote = Instantiate(notes[note.c]);
        //      currentNote.transform.SetParent(parent);
        //      currentNote.transform.localPosition = new Vector3(note.x, note.y, note.b * 3f);
        //      if (note.d != 8) {
        //          currentNote.transform.rotation = Quaternion.Euler(0, 0, Rotation(note.d));
        //          currentNote.transform.GetChild(1).gameObject.SetActive(false);
        //      }
        //      else currentNote.transform.GetChild(0).gameObject.SetActive(false);
        //      currentNote.transform.GetChild(3).gameObject.SetActive(false);
        //  }
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
}

[System.Serializable]
public class beats {
    public List<colorNotes> colorNotes;
    public List<bombNotes> bombNotes;
    public List<bpmEvents> bpmEvents;
} 

[System.Serializable]
public class bpmEvents {
    public float b;
    public float m;
}

[System.Serializable]
public class colorNotes {
    public float b;
    public int x;
    public int y;
    public int a;
    public int c;
    public int d;
}


[System.Serializable]
public class bombNotes {
    public float b;
    public int x;
    public int y;
}


[System.Serializable]
public class beatsV2 {
    public List<_notes> _notes;
    public List<_events> _events;
} 

[System.Serializable]
public class _events {
    public float _time;
    public float _type;
    public int _value;
    public float _floatValue;
}

[System.Serializable]
public class _notes {
    public float _time;
    public int _lineIndex;
    public int _lineLayer;
    public int _type;
    public int _direction;
}

