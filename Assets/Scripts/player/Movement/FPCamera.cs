using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///<summary>
/// Handles all the camera mvoement and rotations. 
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

    //camera movement offset values
    private float cameraYOffset = 0.03f;
    private float cameraStartYPos;
    private float cameraYIndex = 0;
    private float cameraZIndex = 0;
    private int verticalAngle = 80;
    private Transform steeringTarget;
    private bool cameraZRotationTilt = false;

    public bool CameraZRotationTilt {
        get { return cameraZRotationTilt;}
        set { cameraZRotationTilt = value; }
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
                currentAim = Object.Instantiate(new GameObject("camera aim"), cameraPivot.position + cameraPivot.forward, Quaternion.identity, transform).transform;
                desiredAim = Object.Instantiate(new GameObject("camera desired aim"), cameraPivot.position + cameraPivot.forward, Quaternion.identity, transform).transform;
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
    }

    public void OnDisable() {
        PerspectiveProperty.onPerspectiveMissing -= HalfCameraSensitiviy;
        PerspectiveProperty.onPerspectiveAppearing -= UnhalfCameraSensitiviy;
        InputManager.OnRotate -= setMouseDelta;
        BossCutsceneState.OnBossCutsceneStart += OnBossCutSceneStart;
        BossCutsceneState.OnBossCutsceneEnd += OnBossCutSceneEnd;
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
        transform.Rotate(new Vector3(0, mouseDelta.x * controlSettings.Camera_sensetivity * inversionX * cameraSensetivityFactor , 0));

        //vertical rotation
        float inversionY = (controlSettings.Camera_y_invert ? -1 : 1);
        cameraPivot.Rotate(new Vector3(-mouseDelta.y * controlSettings.Camera_sensetivity * inversionY * cameraSensetivityFactor, 0, 0));


        //setting max angle cap
        if (cameraPivot.localRotation.eulerAngles.x > 180)
            cameraPivot.localRotation = Quaternion.Euler( new Vector3(Mathf.Max(cameraPivot.localRotation.eulerAngles.x, 360 - verticalAngle) , 0, 0));
        else
            cameraPivot.localRotation = Quaternion.Euler(new Vector3(Mathf.Min(cameraPivot.localRotation.eulerAngles.x, verticalAngle), 0, 0));
    }


    private void OnBossCutSceneStart(Boss boss) {
        UseSteeringBehaviour = true;
        currentAim.position = desiredAim.position = cameraPivot.position + cameraPivot.transform.forward;
        SteeringBehaviour.Velocity = Vector3.zero;
        steeringTarget = boss.transform;
    }

    private void OnBossCutSceneEnd(Boss boss) {
        UseSteeringBehaviour = false;
    }

    public void UpdateSteeringBehaviour() {
        desiredAim.position = steeringTarget.position;
        SteeringBehaviour.UpdatePosition();
        cameraPivot.LookAt(currentAim, transform.up);
    }
}
