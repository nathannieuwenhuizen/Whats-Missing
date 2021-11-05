using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Portal : MonoBehaviour
{
    // referenses

    [SerializeField]
    private Collider[] connectedColliders;

    private bool insidePortal = false;

    private Player player;
    private static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 0.0f, 180.0f);

    private float positionOffset  = .2f;

    public Portal connectedPortal;
    
    private Camera mainCamera;
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
        if (outputTexture == null) {
            outputTexture = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.ARGB64);
            outputTexture.depth = 32;
            outputTexture.name = "my texture2";
            outputTexture.Create();
        }
        reflectionCamera.targetTexture = outputTexture;
        reflectionPlane.GetComponent<MeshRenderer>().material.SetTexture("_ReflectionTex", outputTexture);

        IsActive = isActive;
    }


    private void LateUpdate()
    {
        if (!isActive) return;
        
        InView = IncameraRange();

        if (insidePortal && player != null) {
            Vector3 objPos = transform.InverseTransformPoint(player.transform.position);
            if (objPos.y < positionOffset)
            {
                Debug.Log("warp!?");
                Teleport();
            } 
            player.Camera.nearClipPlane = objPos.y > 0 ? .01f : .7f;
        }
    
        if (!InView) return;

        if (isReady && mainCamera != null)
            RenderPortal();
        else {
            mainCamera = Camera.main;
            OnValidate();
        }
    }

    public void OnPortalEnter(Player _player) {
        player = _player;

        insidePortal = true;
        foreach (Collider coll in connectedColliders)
        {
            coll.enabled = false;
        }
    }

    public void OnPortalLeave() {
        player.Camera.nearClipPlane = .7f;

        insidePortal = false;
        foreach (Collider coll in connectedColliders)
        {
            coll.enabled = true;
        }
    }

    private void Teleport() {
        OnPortalLeave();
        connectedPortal.OnPortalEnter(player);

        var inTransform = transform;
        var outTransform = connectedPortal.transform;

        // Update position of object.
        Vector3 relativePos = inTransform.InverseTransformPoint(player.transform.position);
        relativePos.y -= positionOffset;
        relativePos = halfTurn * relativePos;
        player.transform.position = outTransform.TransformPoint(relativePos);

        // Update rotation of object.
        Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * player.transform.rotation;
        relativeRot = halfTurn * relativeRot;
        player.transform.rotation = outTransform.rotation * relativeRot;    
    }

    

    private void RenderPortal()
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
        int revert = transform.position.y < mainCamera.transform.position.y ? 1 : -1;
        Vector3 cameraSpaceNormal = reflectionCamera.worldToCameraMatrix.MultiplyVector(clipPlane.up * revert) * dot;
        float camSpaceDst = -Vector3.Dot(cameraSpacePos, cameraSpaceNormal);
        Vector4 clipPlaneCameraSpace = new Vector4(cameraSpaceNormal.x, cameraSpaceNormal.y, cameraSpaceNormal.z, camSpaceDst);

        reflectionCamera.orthographic = true;
        reflectionCamera.projectionMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
    }


    public bool IncameraRange() {

        if (mainCamera == null) {
            mainCamera = Camera.main;
            OnValidate();
        }

        Renderer renderer = reflectionPlane.GetComponent<MeshRenderer>();
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        float offset = transform.InverseTransformPoint(Camera.main.transform.position).y;
        Debug.Log("offset: " + offset);
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
