namespace Custom.Rendering
{
    using System.Collections;
    using UnityEngine;

    // [ExecuteAlways]
    public class PlanarReflection : MonoBehaviour
    {
        // referenses
        public Camera mainCamera;
        public Camera reflectionCamera;
        public Transform reflectionPlane;


        //output Texture
        private RenderTexture output_texture_high;
        private RenderTexture output_texture_mid;
        private RenderTexture output_texture_low;

        // parameters
        public bool copyCameraParamerers;
        public float verticalOffset;
        private bool isReady;


        public float updateInterval = 1f /60f;
        public float UpdateInterval {
            get { return updateInterval;}
            set { updateInterval = value; }
        }
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
            output_texture_high = CreateRenderTexture("highTexture", 1f);
            output_texture_mid = CreateRenderTexture("midTexture", .5f);
            output_texture_low = CreateRenderTexture("midTexture", .3f);

            SetRenderTexture(output_texture_mid);
            IsActive = isActive;

        }

        private RenderTexture CreateRenderTexture(string name, float factor) {
            RenderTexture newTexture = new RenderTexture(Mathf.RoundToInt((float)Screen.width * factor), Mathf.RoundToInt((float)Screen.height * factor), 8, RenderTextureFormat.ARGB32);
            newTexture.name = name;
            newTexture.Create();
            return newTexture;
        }

        private void SetRenderTexture(RenderTexture texture) {
            if (reflectionCamera.targetTexture == texture) return;

            reflectionCamera.targetTexture = texture;            
            reflectionPlane.GetComponent<MeshRenderer>().material.SetTexture("_ReflectionTex", texture);
            // reflectionCamera.Render();
        }

        private void UpdateLODTexture() {

            float distance = Vector3.Distance(mainCamera.gameObject.transform.position, reflectionPlane.transform.position);
            if (distance < 15f) {
                SetRenderTexture(output_texture_high);
            } else if (distance < 50f){
                SetRenderTexture(output_texture_mid);
            } else {
                SetRenderTexture(output_texture_low);
            }
        }


        private void LateUpdate()
        {
            UpdateRendering();
        }

        private void example() {

        }
        private void UpdateRendering() {

            if (!isActive) return;
            InView = IncameraRange();
            if (!InView) return;
            if (isReady && mainCamera != null){
                UpdateLODTexture();
                RenderReflection(); 
            }
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
            int revert = clipPlane.position.y <= mainCamera.transform.position.y ? 1 : -1;
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

                reflectionCamera.targetTexture = output_texture_high;
            }
        }
    }
}