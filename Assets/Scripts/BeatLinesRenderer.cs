using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeatLinesRenderer : MonoBehaviour
{
    public GameObject beatNumber;
    public BeatCalculator beatCalculator;
    public int _linesPastGrid;
    public int _linesFrontGrid;
    public LineRenderer lineRenderer;
    public GameObject beats;

    void Start() {
    }

    public void DrawLines(float beat) {
        lineRenderer.positionCount = 0;

        int linesPastGrid = Mathf.RoundToInt(beatCalculator.GetBpmAtBeat(beat) / 100f) + _linesPastGrid;
        int linesFrontGrid = Mathf.RoundToInt(beatCalculator.GetBpmAtBeat(beat) / 100f) + _linesFrontGrid;

        for (int i = 0; i < linesPastGrid + linesFrontGrid; i++) {
            lineRenderer.positionCount = lineRenderer.positionCount + 4;
            int b = i * 4;
            lineRenderer.SetPosition(b, new Vector3(2, 0, beatCalculator.Zpos(i + beat - linesPastGrid) * beatCalculator.editorScale));
            lineRenderer.SetPosition(b + 1, new Vector3(-2, 0, beatCalculator.Zpos(i + beat - linesPastGrid) * beatCalculator.editorScale));
            lineRenderer.SetPosition(b + 2, new Vector3(-2, 0, beatCalculator.Zpos(i + beat + 1 - linesPastGrid) * beatCalculator.editorScale));
            lineRenderer.SetPosition(b + 3, new Vector3(2, 0, beatCalculator.Zpos(i + beat + 1 - linesPastGrid) * beatCalculator.editorScale));
        }

        lineRenderer.positionCount = lineRenderer.positionCount + 7;
        lineRenderer.SetPosition(lineRenderer.positionCount - 7, new Vector3(2, 0, beatCalculator.Zpos(beat - linesPastGrid) * beatCalculator.editorScale));

        lineRenderer.SetPosition(lineRenderer.positionCount - 6, new Vector3(1, 0, beatCalculator.Zpos(beat - linesPastGrid) * beatCalculator.editorScale));
        lineRenderer.SetPosition(lineRenderer.positionCount - 5, new Vector3(1, 0, beatCalculator.Zpos(linesPastGrid + linesFrontGrid + beat - linesPastGrid) * beatCalculator.editorScale));
        lineRenderer.SetPosition(lineRenderer.positionCount - 4, new Vector3(0, 0, beatCalculator.Zpos(linesPastGrid + linesFrontGrid + beat - linesPastGrid) * beatCalculator.editorScale));
        lineRenderer.SetPosition(lineRenderer.positionCount - 3, new Vector3(0, 0, beatCalculator.Zpos(beat - linesPastGrid) * beatCalculator.editorScale));
        lineRenderer.SetPosition(lineRenderer.positionCount - 2, new Vector3(-1, 0, beatCalculator.Zpos(beat - linesPastGrid) * beatCalculator.editorScale));
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(-1, 0, beatCalculator.Zpos(linesPastGrid + linesFrontGrid + beat - linesPastGrid) * beatCalculator.editorScale));
    }

    public void updateNumbersInPlay(float beat) {
        Destroy(beats.transform.GetChild(0).gameObject);
        int linesPastGrid = Mathf.RoundToInt(beatCalculator.GetBpmAtBeat(beat) / 100f) + _linesPastGrid;
        int linesFrontGrid = Mathf.RoundToInt(beatCalculator.GetBpmAtBeat(beat) / 100f) + _linesFrontGrid;

        float temp = beat + linesFrontGrid;

        GameObject go = Instantiate(beatNumber);
        go.transform.SetParent(beats.transform);
        go.transform.position = new Vector3(2.5f, 0, beatCalculator.Zpos(temp) * beatCalculator.editorScale);
        go.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = (beat + linesFrontGrid).ToString();
    }

    public void updateNumbers(float beat) {
        int linesPastGrid = Mathf.RoundToInt(beatCalculator.GetBpmAtBeat(beat) / 100f) + _linesPastGrid;
        int linesFrontGrid = Mathf.RoundToInt(beatCalculator.GetBpmAtBeat(beat) / 100f) + _linesFrontGrid;

        for (int a = 0; a < beats.transform.childCount; a++) {
            Destroy(beats.transform.GetChild(a).gameObject);
        }

        for (int i = 0; i < linesPastGrid + linesFrontGrid; i++) {
            GameObject go = Instantiate(beatNumber);
            go.transform.SetParent(beats.transform);
            go.transform.position = new Vector3(2.5f, 0, beatCalculator.Zpos(i + beat - linesPastGrid) * beatCalculator.editorScale);
            go.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = (beat + i - linesPastGrid).ToString();
        }
    }
}
