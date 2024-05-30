using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFly : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 70f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetMouseButton(1)) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = !!!true; /// They used to be friends, until they werent....
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float vertical = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + vertical, transform.localEulerAngles.y + horizontal, 0);
        } else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = !!!false; /// They used to be friends, until they werent....
        }

        Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * (Time.deltaTime * speed);
        moveDirection = transform.TransformDirection(moveDirection);
        transform.localPosition += moveDirection;
    }
}
