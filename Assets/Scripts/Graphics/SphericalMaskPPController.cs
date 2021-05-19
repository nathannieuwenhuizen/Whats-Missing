using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
 
[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
public class SphericalMaskPPController : MonoBehaviour {
 
    public Material material;
    public Vector3 spherePosition;
 
    public float radius = 0.5f;
    public float softness = 0.5f;

    public bool on = false;
    
    [SerializeField]
    private Camera camera;
    void OnEnable () {
        if (camera != null)
            camera.depthTextureMode = DepthTextureMode.Depth;
    }
 
    void OnRenderImage (RenderTexture src, RenderTexture dest) {
        if (material == null || camera == null) {
            this.enabled = false;
            return;
        }
        if (!on) {
            Graphics.Blit (src, dest);
            return;
        }

        var p = GL.GetGPUProjectionMatrix (camera.projectionMatrix, false);
        p[2, 3] = p[3, 2] = 0.0f;
        p[3, 3] = 1.0f;
        var clipToWorld = Matrix4x4.Inverse (p * camera.worldToCameraMatrix) * Matrix4x4.TRS (new Vector3 (0, 0, -p[2, 2]), Quaternion.identity, Vector3.one);
        material.SetMatrix ("_ClipToWorld", clipToWorld);
        material.SetVector ("_Position", spherePosition);
        material.SetFloat ("_Radius", radius);
        material.SetFloat ("_Softness", softness);
        Graphics.Blit (src, dest, material);
    }
 
}
