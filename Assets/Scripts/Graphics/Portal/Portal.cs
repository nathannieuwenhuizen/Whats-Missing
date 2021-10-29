using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    // referenses

    public Portal connectedPortal;
    
    public Camera mainCamera;
    public Camera reflectionCamera;
    public Transform reflectionPlane;
    private RenderTexture outputTexture;

    // parameters
    public bool copyCameraParamerers;
    public float verticalOffset;
    private bool isReady;

    private bool inView = false;
    public bool InView {
        get { return inView;}
        set { 
            inView = value; 
            reflectionCamera.gameObject.SetActive(value);
        }
    }
    private bool isActive = true;
    public bool IsActive {
        get { return isActive;}
        set { 
            isActive = value; 
            if (value == false) {
                InView = false;
            }
        }
    }

    // cache
    private Transform mainCamTransform;
    private Transform reflectionCamTransform;

    private void Start() {
        outputTexture = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.ARGB32);
        outputTexture.depth = 32;
        // outputTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.BGRA32);
        outputTexture.name = "my texture2";
        outputTexture.Create();
        reflectionCamera.targetTexture = outputTexture;
        reflectionPlane.GetComponent<MeshRenderer>().material.SetTexture("_ReflectionTex", outputTexture);

        IsActive = isActive;
    }


    private void Update()
    {
        if (!isActive) return;
        
        InView = IncameraRange();
    
        if (!InView) return;

        if (isReady && mainCamera != null)
            RenderReflection();
        else {
            mainCamera = Camera.main;
            OnValidate();
        }
    }

    private void RenderReflection()
    {
        // take main camera directions and position world space
        Vector3 cameraDirectionWorldSpace = mainCamTransform.forward;
        Vector3 cameraUpWorldSpace = mainCamTransform.up;
        Vector3 cameraPositionWorldSpace = mainCamTransform.position;

        cameraPositionWorldSpace.y += verticalOffset;

        // transform direction and position by reflection plane
        Vector3 cameraDirectionPlaneSpace = reflectionPlane.InverseTransformDirection(cameraDirectionWorldSpace);
        Vector3 cameraUpPlaneSpace = reflectionPlane.InverseTransformDirection(cameraUpWorldSpace);
        Vector3 cameraPositionPlaneSpace = reflectionPlane.InverseTransformPoint(cameraPositionWorldSpace);

        Debug.Log("Main cameraDeltaPos: " + cameraPositionPlaneSpace);
        // invert direction and position by reflection plane
        cameraDirectionPlaneSpace.y *= -1;
        cameraDirectionPlaneSpace.x *= -1;
        cameraUpPlaneSpace.y *= -1;
        cameraUpPlaneSpace.x *= -1;
        cameraPositionPlaneSpace.y *= -1;
        cameraPositionPlaneSpace.x *= -1;

        // transform direction and position from reflection plane local space to world space
        cameraDirectionWorldSpace = connectedPortal.reflectionPlane.TransformDirection(cameraDirectionPlaneSpace);
        cameraUpWorldSpace = connectedPortal.reflectionPlane.TransformDirection(cameraUpPlaneSpace);
        cameraPositionWorldSpace = connectedPortal.reflectionPlane.TransformPoint(cameraPositionPlaneSpace);

        // apply direction and position to reflection camera
        reflectionCamTransform.position = cameraPositionWorldSpace;
        reflectionCamTransform.LookAt(cameraPositionWorldSpace + cameraDirectionWorldSpace, cameraUpWorldSpace);
        SetNearClipPlane();
        // reflectionCamera.nearClipPlane = Vector3.Distance(reflectionCamTransform.position, reflectionPlane.position);            
    }

    private void SetNearClipPlane() {
        Transform clipPlane = connectedPortal.transform;
        int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, clipPlane.position - reflectionCamTransform.position));

        Vector3 cameraSpacePos = reflectionCamera.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        int revert = clipPlane.position.y < mainCamera.transform.position.y ? 1 : -1;
        Vector3 cameraSpaceNormal = reflectionCamera.worldToCameraMatrix.MultiplyVector(clipPlane.up * revert) * dot;
        float camSpaceDst = -Vector3.Dot(cameraSpacePos, cameraSpaceNormal);
        Vector4 clipPlaneCameraSpace = new Vector4(cameraSpaceNormal.x, cameraSpaceNormal.y, cameraSpaceNormal.z, camSpaceDst);


        reflectionCamera.projectionMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
    }


    public bool IncameraRange() {

        if (mainCamera == null) {
            mainCamera = Camera.main;
            OnValidate();
        }

        Renderer renderer = reflectionPlane.GetComponent<MeshRenderer>();
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if(GeometryUtility.TestPlanesAABB(planes, renderer.bounds)){
            return true;
        } else {
            return false;   
        }
    }

    private void OnValidate()
    {
        if (mainCamera != null)
        {
            mainCamTransform = mainCamera.transform;
            isReady = true;
        }
        else
            isReady = false;

        if (reflectionCamera != null)
        {
            reflectionCamTransform = reflectionCamera.transform;
            isReady = true;
        }
        else
            isReady = false;

        if(isReady && copyCameraParamerers)
        {
            copyCameraParamerers = !copyCameraParamerers;
            reflectionCamera.CopyFrom(mainCamera);

            reflectionCamera.targetTexture = outputTexture;
        }
    }
}
