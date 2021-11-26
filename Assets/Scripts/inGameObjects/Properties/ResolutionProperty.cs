using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

///<summary>
/// Property that determines the resolution of the screenn. It does it by changing the blit feature material.
///</summary>
public class ResolutionProperty : Property
{
    private float shrinkResolution = 100;
    private BlitMaterialFeature blitFeature;
    private Material material;
    [SerializeField] private ForwardRendererData rendererData = null;
    private string featureName = "Resolution";

    private bool TryGetFeature(out ScriptableRendererFeature feature) {
        feature = rendererData.rendererFeatures.Where((f) => f.name == featureName).FirstOrDefault();
        return feature != null;
    }

    private void Awake() {
        if(TryGetFeature(out var feature)) {
            blitFeature = feature as BlitMaterialFeature;
            blitFeature.SetActive(false);
            material = blitFeature.Material;
        }
    }

    private void SetMaterialActive(bool val) {
        if(TryGetFeature(out var feature)) {
            rendererData.SetDirty();
            feature.SetActive(val);
        }
    }

    public float Resolution {
        get => material.GetFloat("_Resolution");
        set => material.SetFloat("_Resolution", value);
    }
    
    private void OnDisable() {
        SetMaterialActive(false);
    }

    public override void OnShrinking()
    {
        SetMaterialActive(true);
        base.OnShrinking();
    }
    public override IEnumerator AnimateShrinking()
    {
        yield return AnimateMaterialResolutionProperty(Screen.width, shrinkResolution);
        yield return base.AnimateShrinking();
    }

    public override void OnShrinkingFinish()
    {
        base.OnShrinkingFinish();
        Resolution = shrinkResolution;
    }

    public override void OnShrinkRevert()
    {
        Debug.Log("on shrink revert");
        base.OnShrinkRevert();
    }
    public override IEnumerator AnimateShrinkRevert()
    {
        Debug.Log("shrink revert animate");
        yield return AnimateMaterialResolutionProperty(shrinkResolution, Screen.width);
        yield return base.AnimateShrinkRevert();
    }
    public override void OnShrinkingRevertFinish()
    {
        Debug.Log("shrink revert finish");

        base.OnShrinkingRevertFinish();
        SetMaterialActive(false);
    }

    private IEnumerator AnimateMaterialResolutionProperty(float begin, float end) {
        float index = 0;
        while(index < animationDuration) {
            yield return new WaitForEndOfFrame();
            index += Time.deltaTime;
            Resolution = Mathf.Lerp(begin, end, index / animationDuration);
        }
        Resolution = end;
    }

    private void Reset() {
        Word = "resolution";
        AlternativeWords = new string[]{ "resoltions", "pixel", "pixels", "quality"};
    }

}
