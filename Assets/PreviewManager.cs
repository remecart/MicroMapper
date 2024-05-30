using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PreviewManager : MonoBehaviour
{
    private List<(Vector3,Vector3)> cameraTransforms = new List<(Vector3,Vector3)>();
    private Camera MainCamera;
    [SerializeField]
    private PlayerFly flyScript;
    
    void Start()
    {
        MainCamera = Camera.main;
        cameraTransforms.Add((MainCamera.transform.position,MainCamera.transform.rotation.eulerAngles));
        
        cameraTransforms.Add((new Vector3(8,5,-7.0f),new Vector3(15,-30)));
        cameraTransforms.Add((new Vector3(0,2,-6.0f),new Vector3(0,0)));
    }
    
    public void AddCameraTransform(Vector3 position, Vector3 rotation)
    {
        cameraTransforms.Add((position,rotation));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (cnt == 0)
            {
                var transform1 = MainCamera.transform;
                cameraTransforms[0] = (transform1.position,transform1.rotation.eulerAngles);
            }
            cnt++;
            UpdateCameraPosition();
        }
    }
    
    public void UpdateCameraPosition()
    {
        if (cnt >= cameraTransforms.Count)
        {
            cnt = 0;
        }
        
        flyScript.enabled = cnt == 0;
        
        MainCamera.transform.position = cameraTransforms[cnt].Item1;
        MainCamera.transform.rotation = Quaternion.Euler(cameraTransforms[cnt].Item2);
    }

    private int cnt = 0;

}
