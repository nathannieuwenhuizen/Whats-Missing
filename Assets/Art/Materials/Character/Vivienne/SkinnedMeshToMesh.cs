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
            Mesh m = new Mesh();
            skinnedMesh.BakeMesh(m);
            Vector3[] vertices = m.vertices;
            Mesh m2 = new Mesh();
            m2.vertices = vertices;

            VFXGraph.SetMesh("Mesh", m2);

            yield return new WaitForSeconds (refreshRate);
        }
    }
    public void StopVFX() {
        if (UpdateVFXCoroutine != null) StopCoroutine(UpdateVFXCoroutine);
        VFXGraph.Stop();
    }
}
