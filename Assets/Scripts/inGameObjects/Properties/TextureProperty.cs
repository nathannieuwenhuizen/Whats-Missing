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


    public override void onMissing()
    {
        foreach(MeshRenderer mr in room.GetAllObjectsInRoom<MeshRenderer>()) {
            Material[] materials = mr.materials;
            List<Material> temp = new List<Material>(materials);
            Material newMat = noTextureMaterial;
            temp.Add(newMat);
            materials = temp.ToArray();
            mr.materials = materials;
            mr.UpdateGIMaterials();
        }

        base.onMissing();
    }
    public override IEnumerator AnimateMissing()
    {

        StartCoroutine(noTextureMaterial.AnimatingDissolveMaterial(1,0, AnimationCurve.EaseInOut(0,0,1,1), 3f, 0.02f));
        yield return new WaitForSeconds(3f);
        yield return base.AnimateMissing();
    }

    public override void onMissingFinish()
    {
        noTextureMaterial.SetFloat("Dissolve", 0);

        base.onMissingFinish();

    }
    

    public override IEnumerator AnimateAppearing()
    {
        StartCoroutine(noTextureMaterial.AnimatingDissolveMaterial(0,1, AnimationCurve.EaseInOut(0,0,1,1), 3f, 0.02f));
        yield return new WaitForSeconds(3f);
        yield return base.AnimateAppearing();
    }


    public override void onAppearingFinish()
    {
        base.onAppearingFinish();
        foreach(MeshRenderer mr in room.GetAllObjectsInRoom<MeshRenderer>()) {
            Material[] materials = mr.materials;
            List<Material> temp = new List<Material>(materials);
            temp.RemoveAt(temp.Count - 1);
            materials = temp.ToArray();
            mr.materials = materials;
            mr.UpdateGIMaterials();

        }
    }

    private void Reset() {
        Word = "texture";
        AlternativeWords = new string[] { "textures" };
    }

}
