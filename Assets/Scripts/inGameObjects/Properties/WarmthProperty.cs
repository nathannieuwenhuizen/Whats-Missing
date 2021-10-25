using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

///<summary>
/// The warmtPropery of the room. If it is missing. Every object gets and snow material.
///</summary>
public class WarmthProperty : Property
{

    public static OnPropertyToggle OnWarmthMissing;
    public static OnPropertyToggle OnWarmthAppearing;
    [SerializeField]
    private Room room;

    private WhiteBalance whiteBalance;

    [SerializeField]
    private Material snowMaterial;

    [SerializeField]
    private ParticleSystem snowParticles;

    private float animationDuration = 3f;
    private float endOpacity = .8f;

    public override void OnMissing()
    {
        snowParticles.Play();
        OnWarmthMissing?.Invoke();
        foreach(MeshRenderer mr in room.GetAllObjectsInRoom<MeshRenderer>()) {
            Material[] materials = mr.materials;
            List<Material> temp = new List<Material>(materials);
            Material newMat = snowMaterial;
            temp.Add(newMat);
            materials = temp.ToArray();
            mr.materials = materials;
            mr.UpdateGIMaterials();
        }

        base.OnMissing();
    }
    public override IEnumerator AnimateMissing()
    {

        StartCoroutine(snowMaterial.AnimatingSnowMaterial(0, endOpacity, AnimationCurve.EaseInOut(0,0,1,1), animationDuration));
        StartCoroutine(AnimateWarmth(false));
        yield return new WaitForSeconds(animationDuration);
        yield return base.AnimateMissing();
    }

    public override void OnMissingFinish()
    {
        snowMaterial.SetFloat("Opacity", endOpacity);

        base.OnMissingFinish();
        temperature = -100;

    }

    public IEnumerator AnimateWarmth(bool toWarmth) {

        float start = 0;
        float end = -100f;
        if (toWarmth) {
            start = -100f;
            end = 0f;
        }
        temperature = start;
        float index = 0;
        while ( index < animationDuration) {
            index += Time.unscaledDeltaTime;
            temperature = Mathf.Lerp(start, end, index / animationDuration);
            yield return new WaitForEndOfFrame();
        }
        temperature = end;
    }


    public float temperature {
        set {
            if (whiteBalance == null)
                room.Player.Volume.profile.TryGet<WhiteBalance>(out whiteBalance);
            whiteBalance.temperature.value = value;
            // room.Player.Camera.gameObject.gameObject.GetComponent<PostProcessVolume>().profile.GetSetting<ColorGrading>().saturation.value = value;
        }
    }

    public override void OnAppearing() {
        snowParticles.Stop();
        OnWarmthAppearing?.Invoke();

        base.OnAppearing();
    }
    
    public override IEnumerator AnimateAppearing()
    {
        StartCoroutine(snowMaterial.AnimatingSnowMaterial(endOpacity,0, AnimationCurve.EaseInOut(0,0,1,1), animationDuration));
        StartCoroutine(AnimateWarmth(true));
        yield return new WaitForSeconds(animationDuration);
        yield return base.AnimateAppearing();
    }


    public override void OnAppearingFinish()
    {
        base.OnAppearingFinish();
        foreach(MeshRenderer mr in room.GetAllObjectsInRoom<MeshRenderer>()) {
            Material[] materials = mr.materials;
            List<Material> temp = new List<Material>(materials);
            temp.RemoveAt(temp.Count - 1);
            materials = temp.ToArray();
            mr.materials = materials;
            mr.UpdateGIMaterials();

        }
        temperature = 0;

    }

    private void Reset() {
        Word = "temperature";
        AlternativeWords = new string[] { "warmth", "heat", "hot", "hotness", "warmness" };
    }

}
