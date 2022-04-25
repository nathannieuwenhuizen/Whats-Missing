using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Rendering;

///<summary>
/// The reflection property is the reflection the mirrors make at the level. 
/// When missing the mirrors get a default black material.
///</summary>
public class ReflectionPropery : Property
{
    private List<PlanarReflection> planarReflections;
    [SerializeField]
    private Room room;
    [SerializeField]
    private Material mirrorMaterial;

    public override void OnMissing()
    {
        planarReflections = new List<PlanarReflection>(room.GetAllObjectsInRoom<PlanarReflection>());
        foreach(PlanarReflection reflection in planarReflections) {
            MeshRenderer mr = reflection.GetComponent<MeshRenderer>();

            Material[] materials = mr.sharedMaterials;
            List<Material> temp = new List<Material>(materials);
            temp.Add(mirrorMaterial);
            materials = temp.ToArray();
            mr.sharedMaterials = materials;
            mr.UpdateGIMaterials();
        }
        Debug.Log("update reflection mat missing");
        base.OnMissing();
    }

    public override IEnumerator AnimateMissing()
    {
        StartCoroutine(mirrorMaterial.AnimatingDissolveMaterial(1,0, AnimationCurve.EaseInOut(0,0,1,1), animationDuration, 0.02f));
        yield return new WaitForSeconds(animationDuration);
        yield return base.AnimateMissing();
    }
    public override void OnMissingFinish()
    {
        foreach(PlanarReflection reflection in planarReflections) {
            reflection.enabled = false;
        }
        mirrorMaterial.SetFloat("Dissolve", 0);
        base.OnMissingFinish();
    }

    public override void OnAppearing()
    {
        foreach(PlanarReflection reflection in planarReflections) {
            reflection.enabled = true;
        }
        base.OnAppearing();
    }

    public override IEnumerator AnimateAppearing()
    {
        StartCoroutine(mirrorMaterial.AnimatingDissolveMaterial(0,1, AnimationCurve.EaseInOut(0,0,1,1), animationDuration, 0.02f));
        yield return new WaitForSeconds(animationDuration);
        yield return base.AnimateAppearing();
    }


    public override void OnAppearingFinish()
    {
        base.OnAppearingFinish();
        foreach(PlanarReflection reflection in planarReflections) {
            MeshRenderer mr = reflection.GetComponent<MeshRenderer>();

            Material[] materials = mr.sharedMaterials;
            List<Material> temp = new List<Material>(materials);
            temp.RemoveAt(temp.Count - 1);
            materials = temp.ToArray();
            mr.sharedMaterials = materials;
            mr.UpdateGIMaterials();

        }
    }

    private void Reset() {
        Word = "reflection";
        AlternativeWords = new string[]{ "reflections", "mirror", "mirrors"};
    }
}
