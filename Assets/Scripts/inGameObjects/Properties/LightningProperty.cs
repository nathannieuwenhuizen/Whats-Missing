using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

///<summary>
/// The enviroment lighting property of the whole room.
///</summary>
public class LightningProperty : Property
{

    private AmbientColors blackColors = new AmbientColors() {
        groundColor = Color.black, 
        equatorColor = Color.black, 
        skyColor = Color.black,
        sunColor = Color.black};

    private AmbientColors startColors;

    private AmbientColors sceneColors;

    public AmbientColors SceneColors {
        get => sceneColors;
        set {
            sceneColors = value;
            UpdateRenderSettings();
        }
    }

    public LightningProperty() {
        animationDuration = 2f;
    }
    
    private void UpdateRenderSettings() {
        RenderSettings.ambientGroundColor = SceneColors.groundColor;
        RenderSettings.ambientEquatorColor = SceneColors.equatorColor;
        RenderSettings.ambientSkyColor = SceneColors.skyColor;
        RenderSettings.sun.color = SceneColors.sunColor;

    }
    private void Awake() {
        startColors = new AmbientColors() {
            groundColor = RenderSettings.ambientGroundColor,
            equatorColor = RenderSettings.ambientEquatorColor,
            skyColor = RenderSettings.ambientSkyColor,
            sunColor = RenderSettings.sun.color
        };
    }

    public override void OnMissing()
    {
        StopAllCoroutines();
        base.OnMissing();
    }
    public override IEnumerator AnimateMissing()
    {
        yield return StartCoroutine(FadeAmbientColor(blackColors));
        yield return base.AnimateMissing();
    }

    public override void OnMissingFinish()
    {
        SceneColors = blackColors;        
        base.OnMissingFinish();
    }

    public override void OnAppearing() {
        StopAllCoroutines();
        base.OnAppearing();
    }
    

    public override IEnumerator AnimateAppearing()
    {
        yield return StartCoroutine(FadeAmbientColor(startColors));
        yield return base.AnimateAppearing();
    }

    private IEnumerator FadeAmbientColor(AmbientColors end) {
        AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
        AmbientColors start = sceneColors;
        float index = 0;
        while (index < animationDuration)
        {
            index += Time.deltaTime;
            float val = curve.Evaluate(index / animationDuration);
            SceneColors = AmbientColors.LerpUnclamped(start, end, val);            
            yield return new WaitForEndOfFrame();
        }
    }


    public override void OnAppearingFinish()
    {
        SceneColors = startColors;
        base.OnAppearingFinish();
    }

    private void Reset() {
        Word = "lightning";
        AlternativeWords = new string[] { "lights" , "light" };
    }

}

[System.Serializable]
public struct AmbientColors {
    public Color sunColor;
    public Color skyColor;
    public Color equatorColor;
    public Color groundColor;

    public static AmbientColors LerpUnclamped(AmbientColors start, AmbientColors end, float val) {
        return new AmbientColors() {
            sunColor = Color.LerpUnclamped(start.sunColor, end.sunColor, val),
            skyColor = Color.LerpUnclamped(start.skyColor, end.skyColor, val),
            equatorColor = Color.LerpUnclamped(start.equatorColor, end.equatorColor, val),
            groundColor = Color.LerpUnclamped(start.groundColor, end.groundColor, val)
        };
    }
}