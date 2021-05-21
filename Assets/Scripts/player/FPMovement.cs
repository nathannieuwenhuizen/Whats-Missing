using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SFXObject))]
public class FPMovement : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private Transform cameraPivot;
    [SerializeField]
    private Collider topCollider;

    private SFXObject sfx;
    private float walkSoundDistance = 1.5f;
    [SerializeField]
    private AudioClip[] footStepSounds;
    [SerializeField]
    private AudioClip jumpSound;
    [SerializeField]
    private AudioClip landingSound;

    private Vector3 oldPos;

    public bool EnableWalk {get; set; } = true;
    public bool EnableRotation {get; set; } = true;
    private int walkSpeed = 5;
    private int rotateSpeed = 2;

    [SerializeField]
    private float jumpForce = 200f;
    private bool inAir = false;
    private bool inCeiling = false;
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
        sfx.Play(jumpSound, .1f);
        rb.AddForce(new Vector3(0,jumpForce * (inCeiling ? -1 : 1),0));

        inCeiling = false;
    }

    private void OnCollisionEnter(Collision other) {
        if (inAir) {
            inAir = false;
            sfx.Play(landingSound);
            oldPos = transform.position;
        }
        if (other.contacts[0].thisCollider == topCollider) {
            inCeiling = true;
        } else {
            inCeiling = false;
        }
    }

    private void MakeWalkingSounds() {
        if (inAir) return;
        Vector3 delta = new Vector3(transform.position.x - oldPos.x, 0, transform.position.z - oldPos.z);

        if (delta.magnitude > walkSoundDistance){
            oldPos = transform.position;
            sfx.Play(footStepSounds[Mathf.FloorToInt(Random.Range(0, footStepSounds.Length))], .05f);
        }
    }

    private void OnEnable() {
        InputManager.OnMove += SetMovement;
        InputManager.OnRotate += setMouseDelta;
        InputManager.OnJump += Jump;
    }

    private void OnDisable() {
        InputManager.OnMove -= SetMovement;
        InputManager.OnRotate -= setMouseDelta;
        InputManager.OnJump -= Jump;
    }


    private void Start()
    {
        sfx = GetComponent<SFXObject>();
        rb = GetComponent<Rigidbody>();
        EnableCursor(false);

        oldPos = transform.position;
    }
    void Update()
    {
        if (EnableWalk) UpdateMovement();
        if (EnableRotation) UpdateRotation();
    }

    private void UpdateMovement()
    {
        Vector3 dir = transform.TransformDirection(new Vector3(walkDelta.x * walkSpeed, rb.velocity.y, walkDelta.y * walkSpeed));
        if (Time.timeScale == 1) {
            rb.isKinematic = false;

            rb.velocity = dir;
        } else {
            rb.isKinematic = true;
            dir.y = 0;
            transform.position += dir / 100;
            // rb.MovePosition(transform.position + dir);
        }
        MakeWalkingSounds();
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
