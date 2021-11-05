using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// The texture property of the whole room. When a texture is missing, it gets replaced by the no texture material.
///</summary>
public class TextureProperty : Property
{

    [SerializeField]
    private Room room;
    [SerializeField]
    private Material noTextureMaterial;

    public override void OnMissing()
    {
        foreach(Renderer mr in room.GetAllObjectsInRoom<Renderer>()) {
            Material[] materials = mr.sharedMaterials;
            List<Material> temp = new List<Material>(materials);
            Material newMat = noTextureMaterial;
            temp.Add(newMat);
            materials = temp.ToArray();
            mr.sharedMaterials = materials;
            mr.UpdateGIMaterials();
        }

        base.OnMissing();
    }
    public override IEnumerator AnimateMissing()
    {

        StartCoroutine(noTextureMaterial.AnimatingDissolveMaterial(1,0, AnimationCurve.EaseInOut(0,0,1,1), 3f, 0.02f));
        yield return new WaitForSeconds(3f);
        yield return base.AnimateMissing();
    }

    public override void OnMissingFinish()
    {
        noTextureMaterial.SetFloat("Dissolve", 0);
        base.OnMissingFinish();
    }
    

    public override IEnumerator AnimateAppearing()
    {
        StartCoroutine(noTextureMaterial.AnimatingDissolveMaterial(0,1, AnimationCurve.EaseInOut(0,0,1,1), 3f, 0.02f));
        yield return new WaitForSeconds(3f);
        yield return base.AnimateAppearing();
    }


    public override void OnAppearingFinish()
    {
        base.OnAppearingFinish();
        foreach(Renderer mr in room.GetAllObjectsInRoom<Renderer>()) {
            Material[] materials = mr.sharedMaterials;
            List<Material> temp = new List<Material>(materials);
            temp.RemoveAt(temp.Count - 1);
            materials = temp.ToArray();
            mr.sharedMaterials = materials;
            mr.UpdateGIMaterials();

        }
    }

    private void Reset() {
        Word = "texture";
        AlternativeWords = new string[] { "textures" };
    }
}
