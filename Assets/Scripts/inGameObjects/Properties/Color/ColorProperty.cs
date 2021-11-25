using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class ColorProperty : Property
{

    private SphericalMaskPPController maskPPController;
    [SerializeField]
    private Material colorMaterial;
    [SerializeField]
    private Material greyMaterial;

    private float sphereSize = 20f;

    [SerializeField]
    private Room room;
    private ColorAdjustments colorAdjustments;

    public ColorProperty() {
        Debug.Log("initialize");
        animationDuration = 10f;
    }
    public override void OnMissing()
    {
        base.OnMissing();
        StopAllCoroutines();
        if (Animated) {
            StartCoroutine(AnimateSaturation(false));
        } else 
            saturation = -100f;
    }

    public override void OnAppearing()
    {
        base.OnAppearing();
        StopAllCoroutines();
        if (Animated) {
            StartCoroutine(AnimateSaturation(true));
        } else
            saturation = 0;
    }

    public IEnumerator IncreaseMask(bool toColor) {
        maskPPController = room.Player.Camera.GetComponent<SphericalMaskPPController>();
        maskPPController.Camera = room.Player.Camera;
        maskPPController.spherePosition = currentChange.mirror.transform.position;
        maskPPController.enabled = true;
        maskPPController.on = true;
        if (toColor) {
            saturation = 0;
        }
        maskPPController.material = toColor ? colorMaterial : greyMaterial;
        float index = 0;
        while ( index < animationDuration) {
            index += Time.unscaledDeltaTime;
            maskPPController.radius = Mathf.Lerp(0, sphereSize, index / animationDuration);
            yield return new WaitForEndOfFrame();
        }
        maskPPController.enabled = false;
        maskPPController.on = false;
        saturation = toColor ? 0 : -100f;
    }
    public IEnumerator AnimateSaturation(bool toColor) {

        float start = 0;
        float end = -100f;
        if (toColor) {
            start = -100f;
            end = 0f;
        }
        saturation = start;
        float index = 0;
        while ( index < animationDuration) {
            index += Time.unscaledDeltaTime;
            saturation = Mathf.Lerp(start, end, index / animationDuration);
            yield return new WaitForEndOfFrame();
        }
        saturation = end;
    }


    public float saturation {
        set {
            if (colorAdjustments == null)
                room.Player.Volume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
            colorAdjustments.saturation.value = value;
        }
    }
    private void Reset() {
        Word = "color";
        AlternativeWords = new string[]{ "colour", "colors", "colours"};
    }

}
