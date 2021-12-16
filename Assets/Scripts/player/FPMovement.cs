using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// First person movement controlling a rigidbody and a camera.
/// Also handles the footstep sounds and other particles based on the movement.
///</summary>
public class FPMovement : MonoBehaviour
{
    private Vector2 lerpedVelocity;

    private Player player;

    private float cameraSensetivityFactor = 1f;

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
    public static float globalGravity = -9.81f * .8f;

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

    public Collider TopCollider {
        get { return topCollider;}
    }

    [SerializeField]
    private ParticleSystem waterSplash;

    [SerializeField]
    private ParticleSystem windParticles;
    public static string FOOTSTEP_SFXFILE = SFXFiles.player_footstep_normal;

    private float walkStepDistance = 4f;

    private Vector3 oldPos;

    public bool EnableHeadTilt {get; set; } = true;
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

    private int rotateSpeed = 2;

    private bool isRunning = false;
    private bool inAir = false;
    public bool InAir {
        get { return inAir;}
        set { 
            inAir = value; 
            player.CharacterAnimationPlayer.SetBool("inAir", inAir || inCeiling);
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
        isRunning = true;
    }
    public void EndRun() {
        isRunning = false;
    }

    ///<summary>
    ///Makes the player jump
    ///</summary>
    public void Jump() {
        if (inAir && rb.useGravity == false) {
            //negative jump at gravity is missing
            rb.velocity = new Vector3(rb.velocity.x, -jumpForce, rb.velocity.z);
            inCeiling = false;
            return;
        }

        if (inAir) return;
        
        InAir = true;
        StartCoroutine(MakeWindNoices());
        AudioHandler.Instance?.PlaySound(SFXFiles.player_jump, .1f);
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);

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
        if (other.contacts[0].thisCollider == topCollider) inCeiling = true;
        else inCeiling = false;
        
        if (inAir && other.gameObject.tag != Tags.Picked) {
            InAir = false;
            if (!player.IsMissing) AudioHandler.Instance?.PlaySound(SFXFiles.player_landing);
            oldPos = transform.position;
        }
    }

    private void FixedUpdate() {
        if (rb.useGravity){
            Vector3 gravity = globalGravity * gravityScale * Vector3.up;
            rb.AddForce(gravity, ForceMode.Acceleration);
        }
        UpdateAnimator();
    }


    private void OnEnable() {
        InputManager.OnStartRunning += StartRun;
        InputManager.OnEndRunning += EndRun;
        InputManager.OnMove += SetMovement;
        InputManager.OnRotate += setMouseDelta;
        InputManager.OnJump += Jump;
        PauseScreen.OnPause += DisableMovment;
        PauseScreen.OnResume += EnableMovment;
        WindowsErrorMessage.OnErrorShow += DisableMovment;
        WindowsErrorMessage.OnErrorHide += EnableMovment;
        RoomDebugger.OnShow += DisableMovment;
        RoomDebugger.OnHide += EnableMovment;
        SettingPanel.OnSave += ApplyMovementSettings;
        PerspectiveProperty.onPerspectiveMissing += HalfCameraSensitiviy;
        PerspectiveProperty.onPerspectiveAppearing += UnhalfCameraSensitiviy;
    }

    private void OnDisable() {
        InputManager.OnStartRunning -= StartRun;
        InputManager.OnEndRunning -= EndRun;
        InputManager.OnMove -= SetMovement;
        InputManager.OnRotate -= setMouseDelta;
        InputManager.OnJump -= Jump;
        PauseScreen.OnPause -= DisableMovment;
        PauseScreen.OnResume -= EnableMovment;
        WindowsErrorMessage.OnErrorShow -= DisableMovment;
        WindowsErrorMessage.OnErrorHide -= EnableMovment;
        RoomDebugger.OnShow -= DisableMovment;
        RoomDebugger.OnHide -= EnableMovment;
        SettingPanel.OnSave -= ApplyMovementSettings;
        PerspectiveProperty.onPerspectiveMissing -= HalfCameraSensitiviy;
        PerspectiveProperty.onPerspectiveAppearing -= UnhalfCameraSensitiviy;

        FPMovement.FOOTSTEP_SFXFILE = SFXFiles.player_footstep_normal;
    }

    private void HalfCameraSensitiviy() {
        cameraSensetivityFactor = .5f;
    }
    private void UnhalfCameraSensitiviy() {
        cameraSensetivityFactor = 1f;
    }

    private void Awake() {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
        ApplyMovementSettings(Settings.GetSettings());
        cameraStartYPos = cameraPivot.localPosition.y;
    }

    private void ApplyMovementSettings(Settings settings) {
        controlSettings = settings.controlSettings;
    }

    private void Start()
    {
        EnableCursor(false);

        oldPos = transform.position;
    }
    void Update()
    {
        if (EnableRotation) UpdateRotation();
        if (EnableWalk) UpdateMovement();
        if (rb.useGravity == false) {
            InAir = !IsOnFloor();
        } else {
            inCeiling = InAir = !IsOnFloor();
        }
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

    private Vector3 oldPosAnimation = Vector3.zero;
    ///<summary>
    /// Updates the walk animator based on what distance the rigigbody really made.
    ///</summary>
    private void UpdateAnimator() {

        Vector3 delta = transform.InverseTransformDirection(transform.position - oldPosAnimation);
        oldPosAnimation = transform.position;

        delta /= isRunning ? .23f : .15f; // i know, magic numbers...
        Vector2 animationDelta = new Vector2(delta.x, delta.z);
        lerpedVelocity = Vector2.Lerp(lerpedVelocity, animationDelta * (isRunning ? 1 : .5f), Time.deltaTime * 10f);
        player.CharacterAnimationPlayer.SetWalkValues(lerpedVelocity);
    }

    ///<summary>
    ///Checks on when to make footstep sounds and tilt the camera
    ///</summary>
    private void UpdateWalking() {
        if (inAir) return;
        Vector3 delta = new Vector3(transform.position.x - oldPos.x, 0, transform.position.z - oldPos.z);

        if (!player.IsMissing && EnableHeadTilt)  UpdateCameraTilt(delta);

        if (delta.magnitude > walkStepDistance){
            oldPos = transform.position;
            if (!player.IsMissing) {
                // AudioHandler.Instance?.PlaySound(footstepFile, footstepFile == SFXFiles.player_footstep ? .05f : 1f);
                Debug.Log("foot step file = " + FOOTSTEP_SFXFILE);
                float volume = FOOTSTEP_SFXFILE == SFXFiles.player_footstep_normal ? .05f : 1f;
                float pitch = player.IsShrinked ? 1.5f :(player.IsEnlarged ? .5f : 1f);
                if (FOOTSTEP_SFXFILE == SFXFiles.player_footstep_water) {
                    pitch = .5f;
                    volume = .2f;
                }
                AudioHandler.Instance?.Play3DSound(FOOTSTEP_SFXFILE, transform, volume, pitch, false, true, 50);
                if (FOOTSTEP_SFXFILE != SFXFiles.player_footstep_normal) {
                    Debug.Log("emit splash");
                    waterSplash.Emit(1);
                }
            }
        }
    }


    ///<summary>
    /// Changes the camera offset and rotation to mimic the walking bounce.
    ///</summary>
    private void UpdateCameraTilt(Vector3 delta) {
        cameraYIndex = (delta.magnitude / walkStepDistance) * (Mathf.PI * 2);
        float currentCameraYpos = Mathf.Sin(cameraYIndex) * cameraYOffset;
        cameraPivot.localPosition = new Vector3(
            0, 
            cameraStartYPos + currentCameraYpos, 
            0 
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
    private bool IsOnFloor() {
        //if (inAir) return false;
        RaycastHit[] hit;

        float radius = transform.localScale.x * .5f;
        float offset = .1f;

        //TODO: add a collider mask so that it can only collide with the floor.
        hit = Physics.SphereCastAll(transform.position - new Vector3(0, -offset -radius * .1f), radius * .1f, Vector3.down, 1f);
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
            // rb.MovePosition(transform.position + new Vector3(0,-(distance - offset * 1.1f) * .5f,0));
            return true;
        } else {
            distance = 10f;
            return false;
        }
    }

    private void OnCollisionExit(Collision other) {
        if (IsOnFloor() == false) {
            InAir = true;
            StartCoroutine(MakeWindNoices());
        }
    }

    ///<summary>
    ///Updates the camera and player rotation based on the delta of input.
    ///</summary>
    private void UpdateRotation()
    {
        float inversionX = (controlSettings.Camera_x_invert ? -1 : 1);
        //horizontal rotation
        transform.Rotate(new Vector3(0, mouseDelta.x * rotateSpeed * controlSettings.Camera_sensetivity * inversionX * cameraSensetivityFactor , 0));

        float inversionY = (controlSettings.Camera_y_invert ? -1 : 1);

        //vertical rotation
        cameraPivot.Rotate(new Vector3(-mouseDelta.y * rotateSpeed * controlSettings.Camera_sensetivity * inversionY * cameraSensetivityFactor, 0, 0));


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
        Gizmos.DrawSphere(transform.position + new Vector3(0,-distance + transform.localScale.x * .5f ,0), transform.localScale.x * .5f);
    }
}
