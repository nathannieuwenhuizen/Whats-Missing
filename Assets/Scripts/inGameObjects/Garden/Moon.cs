using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : Property
{

    private Material materialSkybox;


    private void Awake() {
        materialSkybox  = RenderSettings.skybox;
    }

    public Moon() {
        normalScale = 7;
        largeScale = 9.75f;
        AnimationDuration = .5f;
    }

    public float MoonSize {
        get => materialSkybox.GetFloat("_MoonSize");
        set => materialSkybox.SetFloat("_MoonSize", value);
    }    
    public float MoonRotation {
        get => materialSkybox.GetFloat("_MoonRotation");
        set => materialSkybox.SetFloat("_MoonRotation", value);
    }

    public override void OnEnlarge()
    {
        MoonRotation = 45;
        StartCoroutine(AnimateEnlarging());
    }

    public override IEnumerator AnimateEnlarging()
    {
        yield return StartCoroutine(AnimateSize(largeScale, AnimationCurve.EaseInOut(0,0,1,1)));
        base.AnimateEnlarging();
    }
    public override void OnEnlargingFinish()
    {
        Debug.Log("enlarging moon" + MoonSize);
        MoonSize = largeScale;
        Debug.Log("enlarging moon" + MoonSize);
        base.OnEnlargingFinish();
    }

    public override void OnEnlargeRevert()
    {
        AnimationDuration = 3f;
        base.OnEnlargeRevert();
    }

    public override IEnumerator AnimateEnlargeRevert()
    {
        yield return StartCoroutine(AnimateSize(normalScale, AnimationCurve.EaseInOut(0,0,1,1)));
        base.AnimateEnlargeRevert();
    }
    public override void OnEnlargeRevertFinish()
    {
        MoonSize = normalScale;
        base.OnEnlargeRevertFinish();
    }

    private void OnDisable() {
        MoonSize = normalScale;
    }


    private IEnumerator AnimateSize(float endSize,  AnimationCurve curve) {
        float timePassed = 0f;
        float beginSize = MoonSize;
        while (timePassed < animationDuration) {
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
            MoonSize = Mathf.LerpUnclamped(beginSize, endSize , curve.Evaluate(timePassed / animationDuration));
        }
        MoonSize = endSize;
    }

    private void Reset() {
        Word = "moon";
        AlternativeWords = new string[] { "moons", "celestial"};
    }

}
