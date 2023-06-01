using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SkinnedMeshToMesh : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMesh;
    public VisualEffect VFXGraph;
    public float refreshRate;
    [HideInInspector]
    public Coroutine UpdateVFXCoroutine;

    private bool vfxActive = false;

    [SerializeField]
    private bool updateScale = true;

    // Start is called before the first frame update
    void Awake()
    {
        StartVFX();
    }

   IEnumerator UpdateVFXGraph()
    {
        yield return new WaitForEndOfFrame();
        while (gameObject.activeSelf)
        {
            //make mesh
            Mesh m = new Mesh();
            skinnedMesh.BakeMesh(m);
            Vector3[] vertices = m.vertices;
            Mesh m2 = new Mesh();
            m2.vertices = vertices;

            //update mesh in vfx graph
            VFXGraph.SetMesh("Mesh", m2);

            //update scale with that of the skinnedmesh
            if (updateScale) VFXGraph.transform.localScale = Vector3.one * (1f / transform.localScale.x);
            yield return new WaitForSeconds (refreshRate);
        }
    }

    ///<summary>
    /// Starts the vfx update loop
    ///</summary>
    public void StartVFX() {
        if (vfxActive) return;
        vfxActive = true;
        UpdateVFXCoroutine = StartCoroutine(UpdateVFXGraph());
        VFXGraph.Play();
    }

    ///<summary>
    /// Stops the VFX update loop
    ///</summary>
    public void StopVFX() {
        if (!vfxActive) return;
        vfxActive = false;

        if (UpdateVFXCoroutine != null) StopCoroutine(UpdateVFXCoroutine);
        VFXGraph.Stop();
    }
}
