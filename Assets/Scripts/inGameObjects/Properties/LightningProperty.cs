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

    public override void onMissing()
    {
        StopAllCoroutines();
        base.onMissing();
    }
    public override IEnumerator AnimateMissing()
    {
        yield return StartCoroutine(FadeAmbientColor(blackColors));
        yield return base.AnimateMissing();
    }

    public override void onMissingFinish()
    {
        SceneColors = blackColors;        
        base.onMissingFinish();
    }

    public override void onAppearing() {
        StopAllCoroutines();
        base.onAppearing();
    }
    

    public override IEnumerator AnimateAppearing()
    {
        yield return StartCoroutine(FadeAmbientColor(startColors));
        yield return base.AnimateAppearing();
    }

    private IEnumerator FadeAmbientColor(AmbientColors end, float duration = 2f) {
        AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
        AmbientColors start = sceneColors;
        float index = 0;
        while (index < duration)
        {
            index += Time.deltaTime;
            float val = curve.Evaluate(index / duration);
            SceneColors = new AmbientColors() {
                sunColor = Color.LerpUnclamped(start.sunColor, end.sunColor, val),
                skyColor = Color.LerpUnclamped(start.skyColor, end.skyColor, val),
                equatorColor = Color.LerpUnclamped(start.equatorColor, end.equatorColor, val),
                groundColor = Color.LerpUnclamped(start.groundColor, end.groundColor, val)
            };
            
            yield return new WaitForEndOfFrame();
        }
    }


    public override void onAppearingFinish()
    {
        SceneColors = startColors;
        base.onAppearingFinish();
    }

    private void Reset() {
        Word = "lightning";
        AlternativeWords = new string[] { "lights" , "light" };
    }

}

public struct AmbientColors {
    public Color sunColor;
    public Color skyColor;
    public Color equatorColor;
    public Color groundColor;
}