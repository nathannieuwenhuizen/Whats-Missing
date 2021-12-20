using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorProperty : Property
{

    public static OnPropertyToggle OnColorEnlarged;

    private BlitMaterialFeature blitFeature;
    private Material material;
    [SerializeField] private ForwardRendererData rendererData = null;
    private string featureName = "Saturation";


    // private SphericalMaskPPController maskPPController;
    // [SerializeField]
    // private Material colorMaterial;
    // [SerializeField]
    // private Material greyMaterial;

    // private float sphereSize = 20f;

    [SerializeField]
    private Room room;
    private ColorAdjustments colorAdjustments;

    public ColorProperty() {
        animationDuration = 3f;
    }
    public override void OnMissing()
    {
        base.OnMissing();
        StopAllCoroutines();
        if (Animated) {
            StartCoroutine(AnimateSaturation(-100f));
        } else 
            saturation = -100f;
    }

    public override void OnAppearing()
    {
        base.OnAppearing();
        StopAllCoroutines();
        if (Animated) {
            StartCoroutine(AnimateSaturation(0));
        } else
            saturation = 0;
    }

    private void Awake() {
        if(TryGetFeature(out var feature)) {
            blitFeature = feature as BlitMaterialFeature;
            blitFeature.SetActive(false);
            material = blitFeature.Material;
            BlitSaturation = 1f;
        }
    }

    private void SetMaterialActive(bool val) {
        if(TryGetFeature(out var feature)) {
            rendererData.SetDirty();
            feature.SetActive(val);
        }
    }

    public override void OnEnlarge()
    {
        OnColorEnlarged?.Invoke();
        SetMaterialActive(true);
        base.OnEnlarge();
    }

    public override IEnumerator AnimateEnlarging()
    {
        StartCoroutine(AnimateBlitSaturation(2f));
        yield return StartCoroutine(AnimateSaturation(100f));
        base.AnimateEnlarging();
    }

    public override void OnEnlargingFinish()
    {
        base.OnEnlargingFinish();
        saturation = 100f;
    }

    public override IEnumerator AnimateEnlargeRevert()
    {
        StartCoroutine(AnimateBlitSaturation(1f));
        yield return StartCoroutine(AnimateSaturation(0));
        base.AnimateEnlargeRevert();
    }

    public override void OnEnlargeRevertFinish()
    {
        base.OnEnlargeRevertFinish();
        saturation = 0;
        SetMaterialActive(false);

    }

    // public IEnumerator IncreaseMask(bool toColor) {
    //     maskPPController = room.Player.Camera.GetComponent<SphericalMaskPPController>();
    //     maskPPController.Camera = room.Player.Camera;
    //     maskPPController.spherePosition = currentChange.mirror.transform.position;
    //     maskPPController.enabled = true;
    //     maskPPController.on = true;
    //     if (toColor) {
    //         saturation = 0;
    //     }
    //     maskPPController.material = toColor ? colorMaterial : greyMaterial;
    //     float index = 0;
    //     while ( index < animationDuration) {
    //         index += Time.unscaledDeltaTime;
    //         maskPPController.radius = Mathf.Lerp(0, sphereSize, index / animationDuration);
    //         yield return new WaitForEndOfFrame();
    //     }
    //     maskPPController.enabled = false;
    //     maskPPController.on = false;
    //     saturation = toColor ? 0 : -100f;
    // }
    public IEnumerator AnimateSaturation(float end = -100f) {
        float start = saturation;
        float index = 0;
        while ( index < animationDuration) {
            index += Time.unscaledDeltaTime;
            saturation = Mathf.Lerp(start, end, index / animationDuration);
            yield return new WaitForEndOfFrame();
        }
        saturation = end;
    }
    public IEnumerator AnimateBlitSaturation(float end = 0) {
        float start = BlitSaturation;
        float index = 0;
        while ( index < animationDuration) {
            index += Time.unscaledDeltaTime;
            BlitSaturation = Mathf.Lerp(start, end, index / animationDuration);
            yield return new WaitForEndOfFrame();
        }
        BlitSaturation = end;
    }


    public float saturation {
        set {
            if (colorAdjustments == null)
                room.Player.Volume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
            colorAdjustments.saturation.value = value;
        } get {
            if (colorAdjustments == null)
                room.Player.Volume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
            return colorAdjustments.saturation.value;
        }
    }
    public float BlitSaturation {
        get => material.GetFloat("_Saturation");
        set => material.SetFloat("_Saturation", value);
    }

    private bool TryGetFeature(out ScriptableRendererFeature feature) {
        feature = rendererData.rendererFeatures.Where((f) => f.name == featureName).FirstOrDefault();
        return feature != null;
    }


    private void OnDisable() {
        SetMaterialActive(false);
    }

    private void Reset() {
        Word = "color";
        AlternativeWords = new string[]{ "colour", "colors", "colours"};
    }

}
