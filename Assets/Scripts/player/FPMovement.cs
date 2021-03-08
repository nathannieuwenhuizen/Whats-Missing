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

    [SerializeField]
    private float jumpForce = 200f;
    private bool inAir = false;
    private int verticalAngle = 80;

    private Vector2 walkDelta = new Vector2();
    private Vector2 mouseDelta = new Vector2();
    public void SetMovement(Vector2 delta)
    {
        walkDelta = delta;
    }
    public void setMouseDelta(Vector2 delta)
    {
        mouseDelta = delta;
    }


    /** Enables the cursor */
    private void EnableCursor(bool enabled = false)
    {
        Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enabled;
    }

    public void Jump() {
        if (inAir) return;
        inAir = true;
        rb.AddForce(new Vector3(0,jumpForce,0));
    }
    private void OnCollisionEnter(Collision other) {
        inAir = false;
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

    private void UpdateMovement()
    {
        rb.velocity = transform.TransformDirection(new Vector3(walkDelta.x * walkSpeed, rb.velocity.y, walkDelta.y * walkSpeed));
    }
    private void UpdateRotation()
    {
        //horizontal rotation
        transform.Rotate(new Vector3(0, mouseDelta.x * rotateSpeed, 0));

        //vertical rotation
        cameraPivot.Rotate(new Vector3(-mouseDelta.y * rotateSpeed, 0, 0));


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
