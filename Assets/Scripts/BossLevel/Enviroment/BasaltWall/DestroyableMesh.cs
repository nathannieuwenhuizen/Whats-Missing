 using UnityEngine;
 using System.Collections;
 using System.Collections.Generic;
 
 
 public class DestroyableMesh : MonoBehaviour {
 
     public IEnumerator SplitMesh (bool destroy, Vector3 forceOrigin)    {
 
        if(GetComponent<MeshFilter>() == null || GetComponent<SkinnedMeshRenderer>() == null) {
            yield return null;
        }

        if(GetComponent<Collider>()) {
            GetComponent<Collider>().enabled = false;
        }

        Mesh M = new Mesh();
        if(GetComponent<MeshFilter>()) {
            M = GetComponent<MeshFilter>().mesh;
        }
        else if(GetComponent<SkinnedMeshRenderer>()) {
            M = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        }

        Material[] materials = new Material[0];
        if(GetComponent<MeshRenderer>()) {
            materials = GetComponent<MeshRenderer>().materials;
        }
        else if(GetComponent<SkinnedMeshRenderer>()) {
            materials = GetComponent<SkinnedMeshRenderer>().materials;
        }

        Vector3[] verts = M.vertices;
        Vector3[] normals = M.normals;
        Vector2[] uvs = M.uv;
        float height = GetComponent<MeshRenderer>().bounds.max.y - GetComponent<MeshRenderer>().bounds.min.y; 
        Debug.Log("height = " + height);
         for (int submesh = 0; submesh < M.subMeshCount; submesh++) {
 
            //  int[] indices = M.GetTriangles(submesh);
 
             for (int i = 0; i < 2; i += 1)    {
                //  Vector3[] newVerts = new Vector3[3];
                //  Vector3[] newNormals = new Vector3[3];
                //  Vector2[] newUvs = new Vector2[3];
                //  for (int n = 0; n < 3; n++)    {
                //      int index = indices[i + n];
                //      newVerts[n] = verts[index];
                //      newUvs[n] = uvs[index];
                //      newNormals[n] = normals[index];
                //  }
 
                //  Mesh mesh = new Mesh();
                //  mesh.vertices = newVerts;
                //  mesh.normals = newNormals;
                //  mesh.uv = newUvs;
                 
                //  mesh.triangles = new int[] { 0, 1, 2, 2, 1, 0 };
 
                GameObject GO = new GameObject("SubMesh " + (i));
            //  GO.layer = LayerMask.NameToLayer(Tags.Forcefield);
                GO.transform.position = transform.position + Vector3.up * ((i / 3) * height);

                // Vector3 scale = new Vector3(1,1f * ((float)i / 3f),1);
                // scale.x *= transform.localScale.x;
                // scale.y *= transform.localScale.y;
                // scale.z *= transform.localScale.z;
                Vector3 scale = transform.localScale * Random.Range(.2f, .8f);
                scale.z /= 3f;

                GO.transform.localScale = scale;
                
                GO.transform.rotation = transform.rotation;
                GO.AddComponent<MeshRenderer>().material = materials[submesh];
                GO.AddComponent<MeshFilter>().mesh = M;
                GO.AddComponent<BoxCollider>();
                float force = 3000;
                GO.AddComponent<Rigidbody>().AddExplosionForce(Random.Range(force, force), forceOrigin, 40);
                Destroy(GO, 5 + Random.Range(0.0f, 5.0f));
             }
         }
 
        GetComponent<Renderer>().enabled = false;
        
        yield return new WaitForSeconds(1.0f);
        if(destroy == true) {
            Destroy(gameObject);
        }
 
     }
 
 
 }