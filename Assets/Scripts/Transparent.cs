using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparent : MonoBehaviour
{
    public Material material;
    public Transform trans;

    void Start() {
        material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z < trans.position.z) {
            ToggleNoteTransparency(1f);
        } else if (transform.position.z >= trans.position.z) {
            ToggleNoteTransparency(0f);
        }
    }

    public void ToggleNoteTransparency(float value)
    {
        material.SetFloat("_Transparent", value);
    }
}
