using System.Collections;
using System.Collections.Generic;
using Boss;
using UnityEngine;
///<summary>
/// Handles all the camera movement and rotations. 
///</summary>
public class FPCamera
{
    //fp movements
    private FPMovement FPMovement;
    private Transform cameraPivot { get => FPMovement.CameraPivot; }
    private Transform transform { get => FPMovement.transform; }
    private ControlSettings controlSettings { get => FPMovement.ControlSettings; }
    private bool EnableRotation { get => FPMovement.EnableRotation; }

    private float cameraSensetivityFactor = 2f;
    private float steeringSpeed = 4f;
    
    private Transform startParent;

    //camera movement offset values
    private float cameraYOffset = 0.03f;
    private float cameraStartYPos;
    private float cameraYIndex = 0;
    private float cameraZIndex = 0;
    private int verticalAngle = 80;

    private Transform steeringTarget;
    private float steeringOffset = 0f;

    private bool cameraZRotationTilt = false;

    public bool CameraZRotationTilt {
        get { return cameraZRotationTilt;}
        set { 
            cameraZRotationTilt = value; 
            if (!value) ResetCameraTilt();
        }
    }
    private float cameraZRotationMagnitude = 10f;

    //steering behaviour, for cutscenes
    private SteeringBehaviour steeringBehavior;
    private Transform desiredAim;
    public Transform DesiredAim {
        get { return desiredAim;}
    }
    private Transform currentAim;
    private bool useSteeringBehaviour = false;
    public bool UseSteeringBehaviour {
        get { return useSteeringBehaviour;}
        set { 
            if (currentAim == null) {
                currentAim = Object.Instantiate(new GameObject("camera aim"), cameraPivot.position + cameraPivot.forward, Quaternion.identity, FPMovement.transform.parent).transform;
                desiredAim = Object.Instantiate(new GameObject("camera desired aim"), cameraPivot.position + cameraPivot.forward, Quaternion.identity, FPMovement.transform.parent).transform;
            }
            currentAim.position = cameraPivot.position + cameraPivot.forward;
            useSteeringBehaviour = value; 
        }
    }
    public SteeringBehaviour SteeringBehaviour {
        get { 
            if (steeringBehavior == null) {
                steeringBehavior = new SteeringBehaviour(currentAim,desiredAim);
            }
            return steeringBehavior;
            }
        set { steeringBehavior = value; }
    }

    private Vector2 mouseDelta = new Vector2();

    public FPCamera(FPMovement _FPMovement) {
        FPMovement = _FPMovement;
    }

    public void setMouseDelta(Vector2 delta)
    {
        mouseDelta = delta;
    }

    public void Awake() {
        cameraStartYPos = cameraPivot.localPosition.y;
    }

    public void Start() {
        EnableCursor(false);
    }

    public void OnEnable() {
        PerspectiveProperty.onPerspectiveMissing += HalfCameraSensitiviy;
        PerspectiveProperty.onPerspectiveAppearing += UnhalfCameraSensitiviy;
        InputManager.OnRotate += setMouseDelta;
        BossCutsceneState.OnBossCutsceneStart += OnBossCutSceneStart;
        BossCutsceneState.OnBossCutsceneEnd += OnBossCutSceneEnd;
        ForcefieldDemo.Forcefield.OnForceFieldImpact += ApplyLittleShake;
        BasaltWall.OnDestroy += ApplyLittleShake;
    }

    public void OnDisable() {
        PerspectiveProperty.onPerspectiveMissing -= HalfCameraSensitiviy;
        PerspectiveProperty.onPerspectiveAppearing -= UnhalfCameraSensitiviy;
        InputManager.OnRotate -= setMouseDelta;
        BossCutsceneState.OnBossCutsceneStart += OnBossCutSceneStart;
        BossCutsceneState.OnBossCutsceneEnd += OnBossCutSceneEnd;
        ForcefieldDemo.Forcefield.OnForceFieldImpact -= ApplyLittleShake;
        BasaltWall.OnDestroy -= ApplyLittleShake;

    }


    ///<summary>
    /// Enables the cursor making it visible or invisible
    ///</summary>
    public void EnableCursor(bool enabled = false)
    {
        Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enabled;
    }


    ///<summary>
    /// Halfs the camera sensetivity
    ///</summary>
    private void HalfCameraSensitiviy() {
        cameraSensetivityFactor = 1f;
    }
    ///<summary>
    /// Unhalfs the cmaera sensetivity
    ///</summary>
    private void UnhalfCameraSensitiviy() {
        cameraSensetivityFactor = 2f;
    }

    ///<summary>
    /// Changes the camera offset and rotation to mimic the walking bounce.
    ///</summary>
    public void UpdateCameraTilt(Vector3 delta, float walkStepDistance) {
        cameraYIndex = (delta.magnitude / walkStepDistance) * (Mathf.PI * 2);
        float currentCameraYpos = Mathf.Sin(cameraYIndex) * cameraYOffset;
        float z = 0 + Mathf.Clamp(FPMovement.LerpedVelocity.magnitude - .5f, 0f, 1f) * .3f;
        cameraPivot.localPosition = new Vector3(
            0, 
            cameraStartYPos + currentCameraYpos, 
            z
        );

        cameraZIndex += Time.deltaTime;
        float zRotation = 0;
        if (cameraZRotationTilt) {
            zRotation = Mathf.Sin(cameraZIndex) * cameraZRotationMagnitude;
        } 
        if (!cameraIsShaking)
            cameraPivot.localRotation = Quaternion.Euler( new Vector3(
                cameraPivot.localRotation.eulerAngles.x,
                cameraPivot.localRotation.eulerAngles.y,
                zRotation
            ));
    }
    private void ResetCameraTilt() {
        cameraPivot.localRotation = Quaternion.Euler( new Vector3(
            cameraPivot.localRotation.eulerAngles.x,
            cameraPivot.localRotation.eulerAngles.y,
            0
        ));
    }



    private Vector3 currentMouseRotDeltaX;
    private Vector3 currentMouseRotDeltaY;
    private float lerpSpeed = 20f;
    private bool lerped = false;
    ///<summary>
    ///Updates the camera and player rotation based on the delta of input.
    ///</summary>
    public void UpdateRotation()
    {
        if (useSteeringBehaviour) {
            UpdateSteeringBehaviour();
            return;
        }
        if (!EnableRotation) return; 

        //horizontal rotation
        float inversionX = (controlSettings.Camera_x_invert ? -1 : 1);
        currentMouseRotDeltaX =Vector3.Lerp(currentMouseRotDeltaX, 
        new Vector3(0, mouseDelta.x * controlSettings.Camera_sensetivity * inversionX * cameraSensetivityFactor , 0), Time.deltaTime * lerpSpeed);
        if (lerped) transform.Rotate(currentMouseRotDeltaX);
        else transform.Rotate(new Vector3(0, mouseDelta.x * controlSettings.Camera_sensetivity * inversionX * cameraSensetivityFactor , 0));

        //vertical rotation
        float inversionY = (controlSettings.Camera_y_invert ? -1 : 1);
        currentMouseRotDeltaY =Vector3.Lerp(currentMouseRotDeltaY, 
        new Vector3(-mouseDelta.y * controlSettings.Camera_sensetivity * inversionY * cameraSensetivityFactor, 0, 0), Time.deltaTime * lerpSpeed);
        if (lerped) cameraPivot.Rotate(currentMouseRotDeltaY);
        else cameraPivot.Rotate(new Vector3(-mouseDelta.y * controlSettings.Camera_sensetivity * inversionY * cameraSensetivityFactor, 0, 0));


        //setting max angle cap
        if (cameraPivot.localRotation.eulerAngles.x > 180)
            cameraPivot.localRotation = Quaternion.Euler( new Vector3(Mathf.Max(cameraPivot.localRotation.eulerAngles.x, 360 - verticalAngle) , 0, cameraPivot.localRotation.eulerAngles.z));
        else
            cameraPivot.localRotation = Quaternion.Euler(new Vector3(Mathf.Min(cameraPivot.localRotation.eulerAngles.x, verticalAngle), 0, cameraPivot.localRotation.eulerAngles.z));
    }

    private bool cameraIsShaking = false;
    public void ApplyLittleShake() {
        cameraIsShaking = true;
        FPMovement.StartCoroutine(cameraPivot.ShakeZRotation(1f, 10, .5f, () => {
            cameraIsShaking = false;
        }));
    }


    private void OnBossCutSceneStart(Boss.Boss boss, float zoom) {
        UseSteeringBehaviour = true;
        currentAim.position = desiredAim.position = cameraPivot.position + cameraPivot.transform.forward;
        SteeringBehaviour.Velocity = Vector3.zero;
        steeringTarget = boss.Eye.transform;
        steeringOffset = 5f;
        
    }

    private void ShowAimCutscene(Transform _target, float duration) {
        UseSteeringBehaviour = true;
        currentAim.position = desiredAim.position = cameraPivot.position + cameraPivot.transform.forward * 5f;
        SteeringBehaviour.Velocity = Vector3.zero;
        steeringTarget = _target.transform;
        // StartCoroutine(TempAimCutscene(duration));
    }
    private IEnumerator TempAimCutscene(float duration) {
        yield return new WaitForSeconds(duration);
        UseSteeringBehaviour = false;
    }

    private void OnBossCutSceneEnd(Boss.Boss boss, float zoom) {
        UseSteeringBehaviour = false;
    }



    private Vector3 old = new Vector3(0,0,0);
    public void UpdateSteeringBehaviour() {
        // desiredAim.position = steeringTarget.position - steeringTarget.up * steeringOffset;
        // SteeringBehaviour.UpdatePosition(steeringSpeed);
        // transform.LookAt(currentAim, Vector3.up);
        // transform.localRotation = Quaternion.Euler( new Vector3(0, transform.localRotation.eulerAngles.y, 0));

        // cameraPivot.LookAt(currentAim, Vector3.up);

        desiredAim.position = steeringTarget.position - steeringTarget.up * steeringOffset;

        Quaternion rotation = Quaternion.LookRotation(desiredAim.position - cameraPivot.position, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * steeringSpeed);

        transform.localRotation = Quaternion.Euler( new Vector3(0, transform.localRotation.eulerAngles.y, 0));

        rotation = Quaternion.LookRotation(desiredAim.position - cameraPivot.position, cameraPivot.up);
        // Debug.Log("rotation = " + rotation.eulerAngles.y);
        cameraPivot.eulerAngles = rotation.eulerAngles;
        // cameraPivot.rotation = Quaternion.Slerp(cameraPivot.rotation, rotation, Time.deltaTime * steeringSpeed);

        // Debug.Log("dist = " + Vector3.Distance(old, rotation.eulerAngles));
        Debug.Log(cameraPivot.localEulerAngles);
        old = rotation.eulerAngles;
    }
}
