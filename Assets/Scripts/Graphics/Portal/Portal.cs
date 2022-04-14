using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Portal : RenderTexturePlane, IPortal
{
    [SerializeField]
    private Collider[] connectedColliders;


    private Player player;
    private static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 0.0f, 180.0f);

    private float positionOffset  = 0f;

    public Portal connectedPortal;
    
    private RenderTexture outputTexture;

    public IPortal ConnectedPortal { get => connectedPortal; }
    public bool InsidePortal { get; set; } = false;

    protected override void LateUpdate()
    {
        if (connectedPortal != null) base.LateUpdate();

        if (!isActive) return;
        if (!InView) return;
        
        if (InsidePortal && player != null) {
            Vector3 objPos = transform.InverseTransformPoint(player.transform.position);
            if (objPos.y < positionOffset)
            {
                Teleport(player);
            } 
        }
    }

    public void OnPortalEnter(Player _player) {
        player = _player;

        InsidePortal = true;
        foreach (Collider coll in connectedColliders)
        {
            coll.enabled = false;
        }
    }

    public void OnPortalLeave() {

        InsidePortal = false;
        foreach (Collider coll in connectedColliders)
        {
            coll.enabled = true;
        }
    }

    public void Teleport(Player _player) {
        player = _player;
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

    

    protected override void UpdateCamera()
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
    }

    protected override void SetNearClipPlane() {
        
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
}
