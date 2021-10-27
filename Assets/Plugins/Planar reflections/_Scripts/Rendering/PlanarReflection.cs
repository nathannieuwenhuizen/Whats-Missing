namespace Custom.Rendering
{
    using UnityEngine;

    [ExecuteAlways]
    public class PlanarReflection : MonoBehaviour
    {
        // referenses
        public Camera mainCamera;
        public Camera reflectionCamera;
        public Transform reflectionPlane;
        public RenderTexture outputTexture;

        // parameters
        public bool copyCameraParamerers;
        public float verticalOffset;
        private bool isReady;

        // cache
        private Transform mainCamTransform;
        private Transform reflectionCamTransform;



        private void Update()
        {
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

        private void SetNearClipPlane() {
            Transform clipPlane = transform;
            int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, clipPlane.position - reflectionCamTransform.position));

            Vector3 cameraSpacePos = reflectionCamera.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
            int revert = clipPlane.position.y < mainCamera.transform.position.y ? 1 : -1;
            Vector3 cameraSpaceNormal = reflectionCamera.worldToCameraMatrix.MultiplyVector(clipPlane.up * revert) * dot;
            float camSpaceDst = -Vector3.Dot(cameraSpacePos, cameraSpaceNormal);
            Vector4 clipPlaneCameraSpace = new Vector4(cameraSpaceNormal.x, cameraSpaceNormal.y, cameraSpaceNormal.z, camSpaceDst);


            reflectionCamera.projectionMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
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
}