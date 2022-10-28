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

    public FPCamera FPCamera {
        get { 
            return player.FPCamera;
        }
    }

    ///<summary>
    /// Range of the foot on the ground beofre it consider itself in the air
    ///</summary>
    public static float FOOT_RANGE = 1f;
    public bool on_grass = false;
    private Vector2 lerpedVelocity;
    public Vector2 LerpedVelocity {
        get { return lerpedVelocity;}
    }

    private Player player;

    public Player Player {
        get { return player;}
    }
    [SerializeField]
    private IKPass IKPass;

    private bool kinematicMovement = true;
    public bool KinematicMovement {
        get { return kinematicMovement;}
        set { kinematicMovement = value; }
    }

    //gravity
    public float gravityScale = 1.0f; 
    public static float GLOBAL_GRAVITY = -9.81f * .8f;

    private ControlSettings controlSettings;
    public ControlSettings ControlSettings {
        get { return controlSettings;}
    }

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
    private ParticleSystem waveEmitter;
    private WaterSplash waterSplash;

    [SerializeField]
    private ParticleSystem windParticles;


    public static string FOOTSTEP_SFXFILE = SFXFiles.player_footstep_normal;

    private float walkStepDistance = 4f;

    private Vector3 oldPos;
    private Vector3 oldFixedPos;
    
    private float walkCycleIndex = 0;

    public bool EnableHeadTilt {get; set; } = true;
    public bool EnableWalk {get; set; } = true;
    public bool EnableRotation {get; set; } = true;


    //movement
    [Header("Movement speeds")]
    [SerializeField]
    private float walkSpeed = 5;
    public float WalkMultiplier { get; set;} = 1f;
    
    [SerializeField]
    private float runSpeed = 10;
    [SerializeField]
    private float jumpForce = 200f;

    private bool isRunning = false;
    private bool inAir = false;
    public bool InAir {
        get { return inAir;}
        set { 
            if (!value) {
                if (windSound != null) windSound.Stop(true);
                windParticles.Stop();
            }
            if (inAir == value) return;
            inAir = value; 
            player.CharacterAnimationPlayer.SetBool("inAir", inAir);
            if (value) {
                if (windCoroutine != null) StopCoroutine(windCoroutine);
                windCoroutine = StartCoroutine(MakeWindNoices());
            }
        }
    }

    private bool inCeiling = false;

    private Coroutine windCoroutine;

    private Vector2 walkDelta = new Vector2();
    private Vector2 mouseDelta = new Vector2();
    public void SetMovement(Vector2 delta)
    {
        walkDelta = delta;
    }

    private float distanceToFloor = 0;

     private void Awake() {
        waterSplash = new WaterSplash(waveEmitter);
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
        ApplyMovementSettings(Settings.GetSettings());
    }

    private void ApplyMovementSettings(Settings settings) {
        controlSettings = settings.controlSettings;
    }

    private void Start()
    {
        FPCamera.Start();
        SetOldPosToTransform();
    }

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
        if (inAir ) {
            //negative jump at gravity is missing
            if (rb.useGravity == false) rb.velocity = new Vector3(rb.velocity.x, -jumpForce, rb.velocity.z);
            return;
        }
        
        InAir = true;
        AudioHandler.Instance?.PlaySound(SFXFiles.player_jump, .1f);
        if (GLOBAL_GRAVITY < 0) {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        } else rb.velocity = new Vector3(rb.velocity.x, -jumpForce, rb.velocity.z);


    }

    ///<summary>
    ///Checks on whether to make the windparticles and wind noices when falling from higher altitudes.
    ///</summary>
    SFXInstance windSound;
    private IEnumerator MakeWindNoices() {
        bool windEffectEnabled = false;
        if (windSound == null) windSound = AudioHandler.Instance?.PlaySound(SFXFiles.wind_fall, .5f, 1, true);
        windSound.Play();
        yield return new WaitForSeconds(1f);
        while (InAir && EnableWalk)
        {
            if (rb.velocity.y < -10f && windEffectEnabled == false){
                windEffectEnabled = true;
                windParticles.Play();
                windSound.Play();
            }

            if (windEffectEnabled) {
                windParticles.emissionRate = 20 + Mathf.Abs(rb.velocity.y);
            }
            yield return new WaitForEndOfFrame();
        }
        windParticles.Stop();
        if (windSound != null) windSound.Stop(true);
    }

    protected void DisableMovment() {
        EnableWalk = false;
        EnableRotation = false;
    }
    protected void EnableMovment() {
        EnableWalk = true;
        EnableRotation = true;
    }

    private void FixedUpdate() {
        if (rb.useGravity){
            Vector3 gravity = GLOBAL_GRAVITY * gravityScale * Vector3.up;
            rb.AddForce(gravity, ForceMode.Acceleration);
        }
        UpdateAnimator();
        oldFixedPos = transform.position;
    }


    private void OnEnable() {
        FPCamera.OnEnable();
        InputManager.OnStartRunning += StartRun;
        InputManager.OnEndRunning += EndRun;
        InputManager.OnMove += SetMovement;
        InputManager.OnJump += Jump;
        PauseScreen.OnPause += DisableMovment;
        PauseScreen.OnResume += EnableMovment;
        WindowsErrorMessage.OnErrorShow += DisableMovment;
        WindowsErrorMessage.OnErrorHide += EnableMovment;
        // RoomDebugger.OnShow += DisableMovment;
        // RoomDebugger.OnHide += EnableMovment;
        SettingPanel.OnSave += ApplyMovementSettings;
    }

    private void OnDisable() {
        FPCamera.OnDisable();
        InputManager.OnStartRunning -= StartRun;
        InputManager.OnEndRunning -= EndRun;
        InputManager.OnMove -= SetMovement;
        InputManager.OnJump -= Jump;
        PauseScreen.OnPause -= DisableMovment;
        PauseScreen.OnResume -= EnableMovment;
        WindowsErrorMessage.OnErrorShow -= DisableMovment;
        WindowsErrorMessage.OnErrorHide -= EnableMovment;
        // RoomDebugger.OnShow -= DisableMovment;
        // RoomDebugger.OnHide -= EnableMovment;
        SettingPanel.OnSave -= ApplyMovementSettings;
        FPMovement.FOOTSTEP_SFXFILE = SFXFiles.player_footstep_normal;
        if (windCoroutine != null) StopCoroutine(windCoroutine);
    }


   
    void Update()
    {
        if (EnableWalk) UpdateMovement();

        InAir = !IsOnFloor();
        FPCamera.UpdateRotation();
        oldPos = transform.position;
    }
    private void LateUpdate() {
        
    }

    ///<summary>
    ///Updates the velocity of the rigidbody.
    ///</summary>
    private void UpdateMovement()
    {
        float speed =  ((isRunning ? runSpeed : walkSpeed) * WalkMultiplier);// * (WaterArea.IN_WATER ? .5f : 1f));
        Vector3 dir = transform.TransformDirection(
            new Vector3(
                walkDelta.x * speed, 
                rb.velocity.y, 
                walkDelta.y * speed
            ));
        if (!kinematicMovement) {
            if (walkDelta.x != 0 || walkDelta.y != 0) rb.velocity = Vector3.Lerp(rb.velocity, dir, Time.deltaTime * 10f);
        } else {
            rb.velocity = new Vector3(dir.x, rb.velocity.y, dir.z);
        }
        UpdateWalking();
    }

    ///<summary>
    /// Updates the walk animator based on what distance the rigigbody really made.
    ///</summary>
    private void UpdateAnimator() {

        Vector3 delta = transform.InverseTransformDirection(transform.position - oldFixedPos);

        delta /= isRunning ? .23f : .15f; // i know, magic numbers...
        Vector2 animationDelta = new Vector2(delta.x, delta.z);
        lerpedVelocity = Vector2.Lerp(lerpedVelocity, animationDelta * (isRunning ? 1 : .5f), Time.deltaTime * 10f);
        player.CharacterAnimationPlayer.SetWalkValues(lerpedVelocity);

        IKPass.SetHeadDirection(cameraPivot);
    }

    ///<summary>
    /// Set the old pos to the current position.
    /// Used when gregory is transported without twearking the camera movement in endlesshallway
    ///</summary>
    public void SetOldPosToTransform() {
        oldPos = oldFixedPos = transform.position;
    }

    ///<summary>
    ///Checks on when to make footstep sounds and tilt the camera
    ///</summary>
    private void UpdateWalking() {
        Vector3 moveDelta = new Vector3(transform.position.x - oldPos.x, 0, transform.position.z - oldPos.z);
        walkCycleIndex += moveDelta.magnitude;

        if (!player.IsMissing && EnableHeadTilt)  FPCamera.UpdateCameraTilt(walkCycleIndex, walkStepDistance);
        
        if (inAir) return;


        if (walkCycleIndex > walkStepDistance){
            walkCycleIndex = 0;
            if (!player.IsMissing) {
                SFXInstance footSound = AudioHandler.Instance?.Play3DSound(FOOTSTEP_SFXFILE, transform, 1, 1, false, true, 50);
                footSound.FMODInstance.setParameterByName(FMODParams.GROUNDPARAM, GetGroundMaterial());
                waterSplash.OnFootStep();
            }
        }
    }

    public float GetGroundMaterial() {
        if (WaterArea.IN_WATER) {
            return 2;
        } else if (on_grass) return 1;
        return 0;
    }

    ///<summary>
    ///Checks if the player is above ground. If the distance is 0 or lower than 0, it sticks to the ground. If not it returns false.
    ///</summary>
    private bool IsOnFloor() {
        RaycastHit[] hit;

        float radius = transform.localScale.x * .5f;
        float offset = .1f;

        //TODO: add a collider mask so that it can only collide with the floor.
        hit = Physics.SphereCastAll(transform.position + transform.up * (radius + offset), radius, transform.up * -1, FOOT_RANGE);
        RaycastHit closest = default(RaycastHit);
        float _distance = Mathf.Infinity;
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].distance < _distance && hit[i].distance != 0) {
                _distance = hit[i].distance;
                closest = hit[i];
            }
        }
        if (_distance != Mathf.Infinity) {
            distanceToFloor = _distance;
            return true;
        } else {
            distanceToFloor = 0;
            return false;
        }
    }


    #region  collision
    private void OnCollisionEnter(Collision other) {  
              
        if (inAir && other.gameObject.tag != Tags.Picked) {
            InAir = false;
            if (!player.IsMissing) AudioHandler.Instance?.PlaySound(SFXFiles.player_landing);
            oldPos = transform.position;
        }
        if (other.gameObject.tag == Tags.Environment_GRASS) on_grass = true;
    }

    private void OnCollisionExit(Collision other) {
        if (IsOnFloor() == false) {
            InAir = true;
            StartCoroutine(MakeWindNoices());

        }
        if (other.gameObject.tag == Tags.Environment_GRASS) on_grass = false;
    }

    #endregion

    private void OnDrawGizmos() {
        Gizmos.color = IsOnFloor() ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0,-distanceToFloor,0));
        Gizmos.DrawLine(transform.position, transform.position + (transform.up * -1f));
        Gizmos.DrawWireSphere(transform.position + new Vector3(0,-distanceToFloor + transform.localScale.x * .5f ,0), transform.localScale.x * .5f);
    }
}
