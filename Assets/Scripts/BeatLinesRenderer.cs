using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeatLinesRenderer : MonoBehaviour
{
    public GameObject beatNumber;
    public BeatCalculator beatCalculator;
    public LineRenderer lineRenderer;
    public GameObject beats;
    private int spawnOffset;
    private int despawnOffset;
    private int scrollCache;

    void Start() {
        spawnOffset = BeatCalculator.instance.spawnOffset;
        despawnOffset = BeatCalculator.instance.despawnOffset;

        updateNumbers(0);
        DrawLines(0);
    }

    public void DrawLines(float beat) {
        lineRenderer.positionCount = 0;
        for (int i = 0; i < despawnOffset + spawnOffset; i++) {
            lineRenderer.positionCount = lineRenderer.positionCount + 4;
            int b = i * 4;
            lineRenderer.SetPosition(b, new Vector3(2, 0, beatCalculator.PositionFromBeat(i + beat - despawnOffset) * beatCalculator.editorScale));
            lineRenderer.SetPosition(b + 1, new Vector3(-2, 0, beatCalculator.PositionFromBeat(i + beat - despawnOffset) * beatCalculator.editorScale));
            lineRenderer.SetPosition(b + 2, new Vector3(-2, 0, beatCalculator.PositionFromBeat(i + beat + 1 - despawnOffset) * beatCalculator.editorScale));
            lineRenderer.SetPosition(b + 3, new Vector3(2, 0, beatCalculator.PositionFromBeat(i + beat + 1 - despawnOffset) * beatCalculator.editorScale));
        }

        lineRenderer.positionCount = lineRenderer.positionCount + 7;
        lineRenderer.SetPosition(lineRenderer.positionCount - 7, new Vector3(2, 0, beatCalculator.PositionFromBeat(beat - despawnOffset) * beatCalculator.editorScale));
        lineRenderer.SetPosition(lineRenderer.positionCount - 6, new Vector3(1, 0, beatCalculator.PositionFromBeat(beat - despawnOffset) * beatCalculator.editorScale));
        lineRenderer.SetPosition(lineRenderer.positionCount - 5, new Vector3(1, 0, beatCalculator.PositionFromBeat(despawnOffset + spawnOffset + beat - despawnOffset) * beatCalculator.editorScale));
        lineRenderer.SetPosition(lineRenderer.positionCount - 4, new Vector3(0, 0, beatCalculator.PositionFromBeat(despawnOffset + spawnOffset + beat - despawnOffset) * beatCalculator.editorScale));
        lineRenderer.SetPosition(lineRenderer.positionCount - 3, new Vector3(0, 0, beatCalculator.PositionFromBeat(beat - despawnOffset) * beatCalculator.editorScale));
        lineRenderer.SetPosition(lineRenderer.positionCount - 2, new Vector3(-1, 0, beatCalculator.PositionFromBeat(beat - despawnOffset) * beatCalculator.editorScale));
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(-1, 0, beatCalculator.PositionFromBeat(despawnOffset + spawnOffset + beat - despawnOffset) * beatCalculator.editorScale));
    }

    public void updateNumbersInScroll(int beat, int scrollOffset) {
        if (scrollCache != scrollOffset) updateNumbers(beat);
        else if (scrollOffset == 1)
        {
            foreach (Transform text in beats.transform)
            {
                if (text.transform.position.z < BeatCalculator.instance.PositionFromBeat(beat - despawnOffset - 1) * BeatCalculator.instance.editorScale)
                    Destroy(text.gameObject);
            }
            int spawnBeat = beat + spawnOffset;
            if (spawnBeat >= 0) createText(spawnBeat);
        }
        else if (scrollOffset == -1)
        {
            foreach (Transform text in beats.transform)
            {
                if (text.transform.position.z > BeatCalculator.instance.PositionFromBeat(beat + spawnOffset + 1) * BeatCalculator.instance.editorScale)
                    Destroy(text.gameObject);
            }
            int spawnBeat = beat - despawnOffset;
            if (spawnBeat >= 0) createText(spawnBeat);
        }
    }

    public void updateNumbers(int beat) {

        foreach (Transform text in beats.transform) {
            Destroy(text.gameObject);
        }

        for (int i = 0; i < despawnOffset + spawnOffset; i++) {
            if (beat + i - despawnOffset >= 0) {
                createText(i + beat - despawnOffset);
            }
        }
    }

    public void createText(int beat) {
        GameObject go = Instantiate(beatNumber);
        go.transform.SetParent(beats.transform);
        go.transform.position = new Vector3(4f, 0, beatCalculator.PositionFromBeat(beat) * beatCalculator.editorScale);
        go.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = (beat).ToString();
    }
}
