using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveProperty : Property
{

    private Vector3 startLocalPos;
    private float cameraYOffset = .5f;

    [SerializeField]
    private Room room;

    private Camera m_camera;

    public static OnPropertyToggle onPerspectiveMissing;
    public static OnPropertyToggle onPerspectiveAppearing;



    private Matrix4x4 ortho, perspective;
    private float fov = 60f,
                 near = .1f,
                 far = 1000f,
                 orthographicSize = 5f;
    private float orthoNear = -2f;
    private float aspect;
    private bool orthoOn = false;

    public void CameraSetup() {
        m_camera = room.Player.Camera;
        startLocalPos = m_camera.transform.localPosition;
        fov = m_camera.fieldOfView;

        aspect = (float)Screen.width / (float)Screen.height;
        ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, orthoNear, far);
        perspective = Matrix4x4.Perspective(fov, aspect, near, far);
    }

    public PerspectiveProperty() {
        animationDuration = 1f;
    }


    #region  missing
    public override void OnMissing()
    {
        m_camera.orthographic = true;
        StopAllCoroutines();
        onPerspectiveMissing?.Invoke();
        room.Player.Movement.EnableHeadTilt = false;
        StartCoroutine(AnimateMissing());
    }
    public override IEnumerator AnimateMissing()
    {
        m_camera.projectionMatrix = perspective;
        StartCoroutine(m_camera.transform.AnimatingLocalPos(startLocalPos + new Vector3(0,cameraYOffset,0), AnimationCurve.EaseInOut(0,0,1,1), animationDuration));
        yield return StartCoroutine(BlendToMatrix(ortho, orthoNear, animationDuration, 8,true));
        yield return base.AnimateMissing();
    }

    public override void OnMissingFinish()
    {
        m_camera.transform.localPosition = startLocalPos + new Vector3(0,cameraYOffset,0);
        m_camera.projectionMatrix = ortho;
        m_camera.nearClipPlane = orthoNear;

        base.OnMissingFinish();
    }
    #endregion


    #region appearing
    
    public override void OnAppearing() {
        StopAllCoroutines();
        StartCoroutine(AnimateAppearing());
    }
    public override IEnumerator AnimateAppearing()
    {
        StartCoroutine(m_camera.transform.AnimatingLocalPos(startLocalPos, AnimationCurve.EaseInOut(0,0,1,1), animationDuration));
        yield return StartCoroutine(BlendToMatrix(perspective, near, animationDuration, 8,false));
        base.AnimateAppearing();
        OnAppearingFinish();

    }
 
    public override void OnAppearingFinish()
    {
        base.OnAppearingFinish();
        room.Player.Movement.EnableHeadTilt = true;
        onPerspectiveAppearing?.Invoke();
        m_camera.orthographic = false;
        m_camera.projectionMatrix = perspective;
        m_camera.nearClipPlane = near;
        m_camera.transform.localPosition = startLocalPos;
    }
    #endregion


    public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time) {
        Matrix4x4 ret = new Matrix4x4();
        for (int i = 0; i < 16; i++)
            ret[i] = Mathf.Lerp(from[i], to[i], time);
        return ret;
    }
 
    private IEnumerator LerpFromTo(Matrix4x4 src, Matrix4x4 dest, float srcNear, float destNear, float duration, float ease, bool reverse) {
        float startTime = Time.time;
        while (Time.time - startTime < duration) {
            float step;
            if (reverse)step = 1-Mathf.Pow(1-(Time.time - startTime) / duration, ease);
            else step = Mathf.Pow((Time.time - startTime) / duration, ease);
            m_camera.projectionMatrix = MatrixLerp(src, dest, step);
            m_camera.nearClipPlane = Mathf.Lerp(srcNear, destNear, step);
            yield return 1;
        }
        m_camera.projectionMatrix = dest;
        m_camera.nearClipPlane = destNear;
        // m_camera.orthographic = reverse;
    }
 
    public IEnumerator BlendToMatrix(Matrix4x4 targetMatrix, float targetNear, float duration, float ease, bool reverse) {
        yield return StartCoroutine(LerpFromTo(m_camera.projectionMatrix, targetMatrix, m_camera.nearClipPlane, targetNear, duration, ease, reverse));
    }

    private void Reset() {
        Word = "perspective";
        AlternativeWords = new string[] {"angle", "point of view", "view"};
    }
    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        CameraSetup();
    }

}
