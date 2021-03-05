using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPMovement : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private Transform cameraPivot;

    private int walkSpeed = 5;
    private int rotateSpeed = 5;

    private int verticalAngle = 80;

    private Vector2 walkDelta = new Vector2();
    public void SetMovement(Vector2 delta)
    {
        walkDelta = delta;
    }

    /** Enables the cursor */
    private void EnableCursor(bool enabled = false)
    {
        Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enabled;
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        EnableCursor(false);
    }
    void Update()
    {
        UpdateMovement();
        UpdateRotation();
    }
    private Vector2 getMouseDelta()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    private void UpdateMovement()
    {
        rb.velocity = transform.TransformDirection(new Vector3(walkDelta.x * walkSpeed, rb.velocity.y, walkDelta.y * walkSpeed));
    }
    private void UpdateRotation()
    {
        //horizontal rotation
        transform.Rotate(new Vector3(0, getMouseDelta().x * rotateSpeed, 0));

        //vertical rotation
        cameraPivot.Rotate(new Vector3(-getMouseDelta().y * rotateSpeed, 0, 0));


        //setting max angle cap
        if (cameraPivot.localRotation.eulerAngles.x > 180)
        {
            cameraPivot.localRotation = Quaternion.Euler( new Vector3(Mathf.Max(cameraPivot.rotation.eulerAngles.x, 360 - verticalAngle) , 0, 0));
        } else
        {
            cameraPivot.localRotation = Quaternion.Euler(new Vector3(Mathf.Min(cameraPivot.rotation.eulerAngles.x, verticalAngle), 0, 0));
        }

    }
}
