using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Rendering;

///<summary>
/// The texture property of the whole room. When a texture is missing, it gets replaced by the no texture material.
///</summary>
public class TextureProperty : Property
{

    [SerializeField]
    private Room room;
    [SerializeField]
    private Material noTextureMaterial;

    public static event OnPropertyToggle OnTextureMissing;

    private List<MaterialHolders> materialHolders;

    public override void OnMissing()
    {
        materialHolders = new List<MaterialHolders>();

        foreach(Renderer mr in room.GetAllObjectsInRoom<Renderer>()) {
            //filter the mirror textures and the lightray
            if (mr.GetComponent<PlanarReflection>() != null || 
            mr.GetComponent<RectTransform>() != null ||
            mr.gameObject.name == "lightray"
            ) continue;
            Material[] materials = mr.sharedMaterials;
            List<Material> temp = new List<Material>(materials);
            temp.Add(noTextureMaterial);
            materials = temp.ToArray();
            mr.sharedMaterials = materials;
            mr.UpdateGIMaterials();
            materialHolders.Add (new MaterialHolders() {renderer = mr, materials = mr.sharedMaterials});
        }

        base.OnMissing();
    }
    public override IEnumerator AnimateMissing()
    {

        StartCoroutine(noTextureMaterial.AnimatingDissolveMaterial(1,0, AnimationCurve.EaseInOut(0,0,1,1), animationDuration, 0.02f));
        yield return new WaitForSeconds(animationDuration);
        yield return base.AnimateMissing();
    }

    public override void OnMissingFinish()
    {
        foreach(MaterialHolders mr in materialHolders) {
            Material newMat = noTextureMaterial;
            mr.renderer.sharedMaterials = new Material[]{newMat};
            mr.renderer.UpdateGIMaterials();
        }

        OnTextureMissing?.Invoke();
        noTextureMaterial.SetFloat("Dissolve", 0);
        base.OnMissingFinish();
    }
    public override void OnAppearing()
    {
        foreach(MaterialHolders holder in materialHolders) {
            holder.renderer.sharedMaterials = holder.materials;
            holder.renderer.UpdateGIMaterials();
        }
        base.OnAppearing();
    }

    public override IEnumerator AnimateAppearing()
    {
        StartCoroutine(noTextureMaterial.AnimatingDissolveMaterial(0,1, AnimationCurve.EaseInOut(0,0,1,1), animationDuration, 0.02f));
        yield return new WaitForSeconds(animationDuration);
        yield return base.AnimateAppearing();
    }


    public override void OnAppearingFinish()
    {
        base.OnAppearingFinish();
        foreach(MaterialHolders mr in materialHolders) {
            Material[] materials = mr.renderer.sharedMaterials;
            List<Material> temp = new List<Material>(materials);
            temp.RemoveAt(temp.Count - 1);
            materials = temp.ToArray();
            mr.renderer.sharedMaterials = materials;
            mr.renderer.UpdateGIMaterials();

        }
    }

    private void Reset() {
        Word = "texture";
        AlternativeWords = new string[] { "textures", "shaders", "shader" };
    }
}
public struct MaterialHolders {
    public Renderer renderer;
    public Material[] materials;
}