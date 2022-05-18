using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// TheBasic script for rendering the camera viewport to a plane, the portal and lanar reflection scrit overrides it.
///</summary>
public class RenderTexturePlane : MonoBehaviour
{

    public delegate void OnTextureUpdate(RenderTexturePlane plane);
    public static event OnTextureUpdate OnTextureUpdating;
    // referenses
    public Camera mainCamera;
    public Camera reflectionCamera;
    public Transform reflectionPlane;

    //output Textures
    private RenderTexture output_texture_high;
    private RenderTexture output_texture_mid;
    private RenderTexture output_texture_low;


    public RenderTexture CurrentTexture {
        get => reflectionCamera.targetTexture;
    }

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

    protected bool isActive = true;
    public bool IsActive {
        get { return isActive;}
        set { 
            isActive = value; 
            if (value == false) {
                InView = false;
            }
        }
    }

    private float timeInterval = 0;
    public float RenderInterval () {
        if (QualitySettings.GetQualityLevel() >= 2) return 0;
        
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
        if (distance < 20f) {
            return 0; //render every frame
        } else if (distance < 40f) 
            return 2f / 60f; //30 fps
        return 4f / 60f; //15 fps
    }


    // cache
    protected Transform mainCamTransform;
    protected Transform reflectionCamTransform;

    protected virtual void Awake() {
        output_texture_high = CreateRenderTexture("highTexture", 1f);
        output_texture_mid = CreateRenderTexture("midTexture", 1f);
        output_texture_low = CreateRenderTexture("midTexture", .5f);

        SetRenderTexture(output_texture_mid);
        IsActive = isActive;

    }

    private RenderTexture CreateRenderTexture(string name, float factor) {
        RenderTexture newTexture = new RenderTexture(Mathf.RoundToInt((float)Screen.width * factor), Mathf.RoundToInt((float)Screen.height * factor), 8, RenderTextureFormat.ARGB64);
        newTexture.name = name;
        newTexture.Create();
        return newTexture;
    }

    public void SetRenderTexture(RenderTexture texture) {
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


    protected virtual void LateUpdate()
    {
        UpdateRendering();
    }

    private void UpdateRendering() {

        if (!isActive) return;
        InView = IncameraRange();
        if (!InView) return;
        if (isReady && mainCamera != null){
            timeInterval += Time.deltaTime;
            if (timeInterval > RenderInterval())
            {
                reflectionCamera.enabled = true;
                timeInterval = 0;
                UpdateLODTexture();
                UpdateCamera();
                OnTextureUpdating?.Invoke(this);

            } else {
                reflectionCamera.enabled = false;
            }
        }
        else {
            mainCamera = Camera.main;
            OnValidate();
        }
    }

    protected virtual void UpdateCamera()
    {

    }


    protected virtual void SetNearClipPlane() {

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
