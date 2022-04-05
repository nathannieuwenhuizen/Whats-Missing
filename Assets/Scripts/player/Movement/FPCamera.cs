using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCamera
{
    private ControlSettings controlSettings;
    private Transform transform;

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

    [SerializeField]
    private Transform cameraPivot;
    public Transform CameraPivot {
        get { return cameraPivot;}
    }

    private int rotateSpeed = 2;
    private int verticalAngle = 80;

    private Vector2 mouseDelta = new Vector2();

    public FPCamera(Transform _transform, Transform _cameraPivot) {
        transform = _transform;
        cameraPivot =_cameraPivot;
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

    private void OnEnable() {
        PerspectiveProperty.onPerspectiveMissing += HalfCameraSensitiviy;
        PerspectiveProperty.onPerspectiveAppearing += UnhalfCameraSensitiviy;
        InputManager.OnRotate -= setMouseDelta;
    }

    private void OnDisable() {
        PerspectiveProperty.onPerspectiveMissing -= HalfCameraSensitiviy;
        PerspectiveProperty.onPerspectiveAppearing -= UnhalfCameraSensitiviy;
        InputManager.OnRotate -= setMouseDelta;
    }

    /** Enables the cursor */
    private void EnableCursor(bool enabled = false)
    {
        Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enabled;
    }

    private void HalfCameraSensitiviy() {
        cameraSensetivityFactor = .5f;
    }
    private void UnhalfCameraSensitiviy() {
        cameraSensetivityFactor = 1f;
    }

    ///<summary>
    /// Changes the camera offset and rotation to mimic the walking bounce.
    ///</summary>
    private void UpdateCameraTilt(Vector3 delta, float walkStepDistance) {
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
            cameraPivot.localRotation = Quaternion.Euler( new Vector3(Mathf.Max(cameraPivot.localRotation.eulerAngles.x, 360 - verticalAngle) , 0, 0));
        } else
        {
            cameraPivot.localRotation = Quaternion.Euler(new Vector3(Mathf.Min(cameraPivot.localRotation.eulerAngles.x, verticalAngle), 0, 0));
        }
    }
}
