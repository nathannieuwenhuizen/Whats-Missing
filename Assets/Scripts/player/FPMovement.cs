using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// First person movement controlling a rigidbody and a camera.
/// Alos handles the footstep sounds and other particles based on the movement.
///</summary>
public class FPMovement : MonoBehaviour
{

    private ControlSettings controlSettings;
    private Rigidbody rb;

    [SerializeField]
    private Transform cameraPivot;
    [SerializeField]
    private Collider topCollider;
    [SerializeField]
    private ParticleSystem windParticles;
    private SFXFiles footstepFile = SFXFiles.player_footstep;

    private float walkSoundDistance = 1.5f;

    private Vector3 oldPos;

    public bool EnableWalk {get; set; } = true;
    public bool EnableRotation {get; set; } = true;
    [SerializeField]
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

    private float distance = 0;

    /** Enables the cursor */
    private void EnableCursor(bool enabled = false)
    {
        
        Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enabled;
    }

    ///<summary>
    ///Makes the player jump
    ///</summary>

    public void Jump() {
        if (inAir) return;
        inAir = true;
        StartCoroutine(MakeWindNoices());
        AudioHandler.Instance?.PlaySound(SFXFiles.player_jump, .1f);
        rb.AddForce(new Vector3(0,jumpForce * (inCeiling ? -1 : 1),0));

        inCeiling = false;
    }

    ///<summary>
    ///Checks on whether to make the windparticles and wind noices when falling from higher altitudes.
    ///</summary>
    private IEnumerator MakeWindNoices() {
        bool windEffectEnabled = false;
        while (inAir)
        {
            if (rb.velocity.y < -10f && windEffectEnabled == false){
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

        if (inAir) {
            inAir = false;
            AudioHandler.Instance?.PlaySound(SFXFiles.player_landing);
            oldPos = transform.position;
        }
        if (other.contacts[0].thisCollider == topCollider) {
            inCeiling = true;
        } else {
            inCeiling = false;
        }
    }

    ///<summary>
    ///Checks on when to make footstep sounds
    ///</summary>
    private void MakeWalkingSounds() {
        if (inAir) return;
        Vector3 delta = new Vector3(transform.position.x - oldPos.x, 0, transform.position.z - oldPos.z);

        if (delta.magnitude > walkSoundDistance){
            oldPos = transform.position;
            AudioHandler.Instance?.PlaySound(footstepFile, footstepFile == SFXFiles.player_footstep ? .05f : 1f);
        }
    }

    private void OnEnable() {
        InputManager.OnMove += SetMovement;
        InputManager.OnRotate += setMouseDelta;
        InputManager.OnJump += Jump;
        PauseScreen.OnPause += DisableMovment;
        PauseScreen.OnResume += EnableMovment;
    }

    private void OnDisable() {
        InputManager.OnMove -= SetMovement;
        InputManager.OnRotate -= setMouseDelta;
        InputManager.OnJump -= Jump;
        PauseScreen.OnPause -= DisableMovment;
        PauseScreen.OnResume -= EnableMovment;
    }

    private void Awake() {
        controlSettings = Settings.GetSettings().controlSettings;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        EnableCursor(false);

        oldPos = transform.position;
    }
    void Update()
    {
        if (EnableWalk) UpdateMovement();
        if (EnableRotation) UpdateRotation();
        CheckFloorCollision();
    }

    ///<summary>
    ///Updates the velocity of the rigidbody.
    ///</summary>
    private void UpdateMovement()
    {
        Vector3 dir = transform.TransformDirection(new Vector3(walkDelta.x * walkSpeed, rb.velocity.y, walkDelta.y * walkSpeed));
        if (Time.timeScale == 1) {
            rb.isKinematic = false;
            rb.velocity = dir;
        }
        MakeWalkingSounds();
    }

    ///<summary>
    ///Checks if the player is above ground. If the distance is 0 or lower than 0, it sticks to the ground. If not it returns false.
    ///</summary>
    private bool CheckFloorCollision() {
        if (inAir) return false;
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
            return true;;
        } else {
            distance = 10f;
            return false;
        }
    }

    private void OnCollisionExit(Collision other) {
        if (CheckFloorCollision() == false) {
            inAir = true;
            StartCoroutine(MakeWindNoices());
        }
        //check if the player really is in the air, by checking below the player.
        
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
        Gizmos.DrawSphere(transform.position + new Vector3(0,-distance + transform.localScale.x / 2f ,0), transform.localScale.x / 2f);
    }
}
