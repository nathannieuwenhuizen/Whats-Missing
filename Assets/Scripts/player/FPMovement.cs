using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// First person movement controlling a rigidbody and a camera.
/// Alos handles the footstep sounds and other particles based on the movement.
///</summary>
public class FPMovement : MonoBehaviour
{

    //camera movement offset values
    private float cameraYOffset = 0.03f;
    private float cameraStartYPos;
    private float cameraYIndex = 0;
    private float cameraZIndex = 0;
    private bool cameraZRotationTilt = false;
    public bool CameraZRotationTilt {
        get { return cameraZRotationTilt;}
        set { cameraZRotationTilt = value; }
    }
    private float cameraZRotationMagnitude = 10f;


    //gravity
    public float gravityScale = 1.0f; 
    public static float globalGravity = -9.81f;

    private ControlSettings controlSettings;

    private Rigidbody rb;
    public Rigidbody RB {
        get { return rb;}
    }

    [SerializeField]
    private Transform cameraPivot;
    public Transform CameraPivot {
        get { return cameraPivot;}
    }
    [SerializeField]
    private Collider topCollider;
    [SerializeField]
    private ParticleSystem windParticles;
    private SFXFiles footstepFile = SFXFiles.player_footstep;

    private float walkStepDistance = 4f;

    private Vector3 oldPos;

    public bool EnableWalk {get; set; } = true;
    public bool EnableRotation {get; set; } = true;


    //movement
    [Header("Movement speeds")]
    [SerializeField]
    private float walkSpeed = 5;
    [SerializeField]
    private float runSpeed = 8;
    [SerializeField]
    private float jumpForce = 200f;

    [SerializeField]
    private Animator characterAnimator;

    public Animator CharacterAnimator {
        get { return characterAnimator;}
    }
    private int rotateSpeed = 2;

    private bool isRunning = false;
    private bool inAir = false;
    public bool InAir {
        get { return inAir;}
        set { 
            inAir = value; 
            characterAnimator.SetBool("inAir", inAir);
        }
    }

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

    private float distance = 0;

    /** Enables the cursor */
    private void EnableCursor(bool enabled = false)
    {
        
        Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enabled;
    }

    public void StartRun() {
        Debug.Log("start run");
        isRunning = true;
    }
    public void EndRun() {
        isRunning = false;
    }

    ///<summary>
    ///Makes the player jump
    ///</summary>
    public void Jump() {
        if (inAir && rb.useGravity == true) return;
        InAir = true;
        StartCoroutine(MakeWindNoices());
        AudioHandler.Instance?.PlaySound(SFXFiles.player_jump, .1f);
        rb.velocity = new Vector3(rb.velocity.x, jumpForce * (inCeiling ? -1 : 1), rb.velocity.z);

        inCeiling = false;
    }

    ///<summary>
    ///Checks on whether to make the windparticles and wind noices when falling from higher altitudes.
    ///</summary>
    private IEnumerator MakeWindNoices() {
        bool windEffectEnabled = false;
        while (InAir)
        {
            if (rb.velocity.y < -20f && windEffectEnabled == false){
                windEffectEnabled = true;
                AudioHandler.Instance.PlaySound(SFXFiles.wind_fall, .5f, 1, true);
                windParticles.Play();
            }

            if (windEffectEnabled) {
                windParticles.emissionRate = 20 + Mathf.Abs(rb.velocity.y);
            }
            yield return new WaitForEndOfFrame();
        }
        windParticles.Stop();
        AudioHandler.Instance.StopSound(SFXFiles.wind_fall);
    }

    protected void DisableMovment() {
        EnableWalk = false;
        EnableRotation = false;
    }
    protected void EnableMovment() {
        EnableWalk = true;
        EnableRotation = true;
    }

    private void OnCollisionEnter(Collision other) {
        footstepFile = other.gameObject.tag == "Stairs" ? SFXFiles.stairs_footstep : SFXFiles.player_footstep;

        if (inAir && other.gameObject.tag != Tags.Picked) {
            InAir = false;
            AudioHandler.Instance?.PlaySound(SFXFiles.player_landing);
            oldPos = transform.position;
        }
        if (other.contacts[0].thisCollider == topCollider) {
            inCeiling = true;
        } else {
            inCeiling = false;
        }
    }

    private void FixedUpdate() {
        if (rb.useGravity){
            Vector3 gravity = globalGravity * gravityScale * Vector3.up;
            rb.AddForce(gravity, ForceMode.Acceleration);
        }
    }


    private void OnEnable() {
        InputManager.OnStartRunning += StartRun;
        InputManager.OnEndRunning += EndRun;
        InputManager.OnMove += SetMovement;
        InputManager.OnRotate += setMouseDelta;
        InputManager.OnJump += Jump;
        PauseScreen.OnPause += DisableMovment;
        PauseScreen.OnResume += EnableMovment;
    }

    private void OnDisable() {
        InputManager.OnStartRunning -= StartRun;
        InputManager.OnEndRunning -= EndRun;
        InputManager.OnMove -= SetMovement;
        InputManager.OnRotate -= setMouseDelta;
        InputManager.OnJump -= Jump;
        PauseScreen.OnPause -= DisableMovment;
        PauseScreen.OnResume -= EnableMovment;
    }

    private void Awake() {
        controlSettings = Settings.GetSettings().controlSettings;
        cameraStartYPos = cameraPivot.localPosition.y;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        EnableCursor(false);

        oldPos = transform.position;
    }
    void Update()
    {
        if (EnableRotation) UpdateRotation();
        if (EnableWalk) UpdateMovement();
        //CheckFloorCollision();
        UpdateAnimator();
    }

    ///<summary>
    ///Updates the velocity of the rigidbody.
    ///</summary>
    private void UpdateMovement()
    {
        Vector3 dir = transform.TransformDirection(
            new Vector3(
                walkDelta.x * (isRunning ? runSpeed : walkSpeed), 
                rb.velocity.y, 
                walkDelta.y * (isRunning ? runSpeed : walkSpeed)
            ));
        rb.velocity = dir;
        UpdateWalking();
    }
    private void UpdateAnimator() {
        characterAnimator.SetFloat("deltaX", walkDelta.x * (isRunning ? 1 : .5f));
        characterAnimator.SetFloat("deltaY", walkDelta.y * (isRunning ? 1 : .5f));
    }

    ///<summary>
    ///Checks on when to make footstep sounds and tilt the camera
    ///</summary>
    private void UpdateWalking() {
        if (inAir) return;
        Vector3 delta = new Vector3(transform.position.x - oldPos.x, 0, transform.position.z - oldPos.z);

        UpdateCameraTilt(delta);

        if (delta.magnitude > walkStepDistance){
            oldPos = transform.position;
            AudioHandler.Instance?.PlaySound(footstepFile, footstepFile == SFXFiles.player_footstep ? .05f : 1f);
            AudioHandler.Instance?.Player3DSound(footstepFile, transform, footstepFile == SFXFiles.player_footstep ? .05f : 1f, 1, false, true, 50);
        }
    }


    ///<summary>
    /// Changes the camera offset and rotation to mimic the walking bounce.
    ///</summary>
    private void UpdateCameraTilt(Vector3 delta) {
        cameraYIndex = (delta.magnitude / walkStepDistance) * (Mathf.PI * 2);
        float currentCameraYpos = Mathf.Sin(cameraYIndex) * cameraYOffset;
        cameraPivot.localPosition = new Vector3(
            cameraPivot.localPosition.x, 
            cameraStartYPos + currentCameraYpos, 
            cameraPivot.localPosition.z 
        );

        cameraZIndex += Time.deltaTime;
        float zRotation = 0;
        if (cameraZRotationTilt) {
            zRotation = Mathf.Sin(cameraZIndex) * cameraZRotationMagnitude;
        } 
        cameraPivot.localRotation = Quaternion.Euler( new Vector3(
            cameraPivot.localRotation.eulerAngles.x,
            cameraPivot.localRotation.eulerAngles.y,
            zRotation
        ));
    }

    ///<summary>
    ///Checks if the player is above ground. If the distance is 0 or lower than 0, it sticks to the ground. If not it returns false.
    ///</summary>
    private bool CheckFloorCollision() {
        if (inAir || rb.useGravity == false) return false;
        RaycastHit[] hit;

        float radius = transform.localScale.x / 2f;
        float offset = .1f;

        //TODO: add a collider mask so that it can only collide with the floor.
        hit = Physics.SphereCastAll(transform.position - new Vector3(0, -offset -radius), radius, Vector3.down, 1f);
        RaycastHit closest = default(RaycastHit);
        float _distance = 10f;
        for (int i = 0; i < hit.Length; i++)
        {
            // Debug.Log(hit[i].transform.name + " | "+ hit[i].distance);
            if (hit[i].distance < _distance && hit[i].distance != 0) {
                _distance = hit[i].distance;
                closest = hit[i];
            }
        }
        if (_distance != 10f) {
            distance = _distance;
            rb.MovePosition(transform.position + new Vector3(0,-(distance - offset),0));
            InAir = false;
            return true;
        } else {
            distance = 10f;
            return false;
        }
    }

    private void OnCollisionExit(Collision other) {
        if (CheckFloorCollision() == false) {
            InAir = true;
            StartCoroutine(MakeWindNoices());
        }        
    }
    private void OnTriggerEnter(Collider other) {
        AreaTextMeshFader text = other.gameObject.GetComponent<AreaTextMeshFader>();
        if (text != null){
            text.FadeIn();
        }
    }

    private void OnTriggerExit(Collider other) {
        AreaTextMeshFader text = other.gameObject.GetComponent<AreaTextMeshFader>();
        if (text != null){
            text.FadeOut();
        }
    }
    ///<summary>
    ///Updates the camera and player rotation based on the delta of input.
    ///</summary>
    private void UpdateRotation()
    {
        float inversionX = (controlSettings.Camera_x_invert ? -1 : 1);
        //horizontal rotation
        transform.Rotate(new Vector3(0, mouseDelta.x * rotateSpeed * controlSettings.Camera_sensetivity * inversionX , 0));

        float inversionY = (controlSettings.Camera_y_invert ? -1 : 1);

        //vertical rotation
        cameraPivot.Rotate(new Vector3(-mouseDelta.y * rotateSpeed * controlSettings.Camera_sensetivity * inversionY, 0, 0));


        //setting max angle cap
        if (cameraPivot.localRotation.eulerAngles.x > 180)
        {
            cameraPivot.localRotation = Quaternion.Euler( new Vector3(Mathf.Max(cameraPivot.rotation.eulerAngles.x, 360 - verticalAngle) , 0, 0));
        } else
        {
            cameraPivot.localRotation = Quaternion.Euler(new Vector3(Mathf.Min(cameraPivot.rotation.eulerAngles.x, verticalAngle), 0, 0));
        }
    }


    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0,-distance,0));
        Gizmos.DrawSphere(transform.position + new Vector3(0,-distance + transform.localScale.x *.1f ,0), transform.localScale.x * .1f);
    }
}
