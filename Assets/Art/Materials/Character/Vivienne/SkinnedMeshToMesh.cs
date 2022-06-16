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
    // Start is called before the first frame update
    void Start()
    {
        UpdateVFXCoroutine = StartCoroutine(UpdateVFXGraph());
    }

   IEnumerator UpdateVFXGraph()
    {
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
            VFXGraph.transform.localScale = Vector3.one * (1f / transform.localScale.x);
            yield return new WaitForSeconds (refreshRate);
        }
    }
    public void StopVFX() {
        if (UpdateVFXCoroutine != null) StopCoroutine(UpdateVFXCoroutine);
        VFXGraph.Stop();
    }
}
