namespace Custom.Rendering
{
    using System.Collections;
    using UnityEngine;

    // [ExecuteAlways]
    public class PlanarReflection : RenderTexturePlane
    {
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
            cameraUpPlaneSpace.y *= -1;
            cameraPositionPlaneSpace.y *= -1;

            // transform direction and position from reflection plane local space to world space
            cameraDirectionWorldSpace = reflectionPlane.TransformDirection(cameraDirectionPlaneSpace);
            cameraUpWorldSpace = reflectionPlane.TransformDirection(cameraUpPlaneSpace);
            cameraPositionWorldSpace = reflectionPlane.TransformPoint(cameraPositionPlaneSpace);

            // apply direction and position to reflection camera
            reflectionCamTransform.position = cameraPositionWorldSpace;
            reflectionCamTransform.LookAt(cameraPositionWorldSpace + cameraDirectionWorldSpace, cameraUpWorldSpace);
            SetNearClipPlane();
            // reflectionCamera.nearClipPlane = Vector3.Distance(reflectionCamTransform.position, reflectionPlane.position);            
        }

        public Vector3 delta;

        protected override void SetNearClipPlane() {
            Transform clipPlane = transform;
            int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, clipPlane.position - reflectionCamTransform.position));

            Vector3 cameraSpacePos = reflectionCamera.worldToCameraMatrix.MultiplyPoint(clipPlane.position);

            delta = clipPlane.InverseTransformDirection(clipPlane.transform.position - mainCamera.transform.position);

            int invert = delta.z > 0 ? -1 : 1;
            // int invert = clipPlane.position.y <= mainCamera.transform.position.y ? 1 : -1;



            Vector3 cameraSpaceNormal = reflectionCamera.worldToCameraMatrix.MultiplyVector(clipPlane.up * invert) * dot;
            float camSpaceDst = -Vector3.Dot(cameraSpacePos, cameraSpaceNormal);
            Vector4 clipPlaneCameraSpace = new Vector4(cameraSpaceNormal.x, cameraSpaceNormal.y, cameraSpaceNormal.z, camSpaceDst);

            reflectionCamera.orthographic = false;
            reflectionCamera.projectionMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }

        public void OnDrawGizmosSelected() {
            Gizmos.DrawLine(transform.position, transform.position + delta);
            Gizmos.DrawSphere(transform.position, .1f);
        }
    }
}