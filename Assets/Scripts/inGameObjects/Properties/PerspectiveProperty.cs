using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveProperty : Property
{
    [SerializeField]
    private Room room;

    private Camera m_camera;


    private Matrix4x4 ortho,
                        perspective;
    public float fov = 60f,
                        near = .1f,
                        far = 1000f,
                        orthographicSize = 5f;
    private float aspect;
    private bool orthoOn = false;

    private void Reset() {
        Word = "perspective";
        AlternativeWords = new string[] {"angle", "point of view", "view"};
    }
    public override void OnMissing()
    {
        room.Player.Movement.EnableRotation = false;
        m_camera = room.Player.Camera;
        // fieldOfView = cam.fieldOfView;
        // farClipPlane = cam.farClipPlane;
        base.OnMissing();
    }


    public void CameraSetup() {
        m_camera = room.Player.Camera;

        aspect = (float)Screen.width / (float)Screen.height;
        ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far);
        perspective = Matrix4x4.Perspective(fov, aspect, near, far);
        // m_camera.projectionMatrix = perspective;
        // orthoOn = false;
    }

    public override IEnumerator AnimateMissing()
    {
        // yield return StartCoroutine(AnimateOrthographic(0,1));
        yield return base.AnimateMissing();
    }

    public override void onMissingFinish()
    {
        base.onMissingFinish();
    }


    void Update() {
        if (room.Player != null) {
            CameraSetup();
        }
    

        if (Input.GetKeyDown(KeyCode.P)) {
            if (room.Player != null) {
                Debug.Log("switch!!!");
                CameraSetup();
                orthoOn = !orthoOn;
                if (orthoOn)
                {
                    m_camera.orthographic = true;
                    m_camera.projectionMatrix = perspective;

                    BlendToMatrix(ortho, 1f, 8,true);
                }
                else
                    BlendToMatrix(perspective, 1f, 8,false);
            }
        }
    }
    
    public override void OnAppearing() {
        // m_camera.orthographic = false;
        base.OnAppearing();
    }

    public override IEnumerator AnimateAppearing()
    {
        // yield return StartCoroutine(AnimateOrthographic(1,0));
        yield return base.AnimateAppearing();
    }
 

    public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time) {
        Matrix4x4 ret = new Matrix4x4();
        for (int i = 0; i < 16; i++)
            ret[i] = Mathf.Lerp(from[i], to[i], time);
        return ret;
    }
 
    private IEnumerator LerpFromTo(Matrix4x4 src, Matrix4x4 dest, float duration, float ease, bool reverse) {
        float startTime = Time.time;
        while (Time.time - startTime < duration) {
            float step;
            if (reverse)step = 1-Mathf.Pow(1-(Time.time - startTime) / duration, ease);
            else step = Mathf.Pow((Time.time - startTime) / duration, ease);
            m_camera.projectionMatrix = MatrixLerp(src, dest, step);
            yield return 1;
        }
        m_camera.projectionMatrix = dest;
        // m_camera.orthographic = reverse;
    }
 
    public Coroutine BlendToMatrix(Matrix4x4 targetMatrix, float duration, float ease, bool reverse) {
        StopAllCoroutines();
        return StartCoroutine(LerpFromTo(m_camera.projectionMatrix, targetMatrix, duration, ease, reverse));
    }

    public override void onAppearingFinish()
    {
        room.Player.Movement.EnableRotation = true;
        base.onAppearingFinish();
    }
}
