using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class ColorProperty : Property
{

    private SphericalMaskPPController maskPPController;
    [SerializeField]
    private Material colorMaterial;
    [SerializeField]
    private Material greyMaterial;

    private float animationDuration = 3f;
    private float sphereSize = 20f;

    [SerializeField]
    private Room room;

    public override void onMissing()
    {
        Debug.Log("on missing!" + Animated);
        base.onMissing();
        if (Animated) {
            StopAllCoroutines();
            StartCoroutine(IncreaseMask(false));
        } else 
            saturation = -100f;
    }
    public override void onAppearing()
    {
        base.onAppearing();
        if (Animated) {
            StopAllCoroutines();
            StartCoroutine(IncreaseMask(true));
        } else
            saturation = 0;
    }

    public IEnumerator IncreaseMask(bool toColor) {
        maskPPController = room.Player.Camera.GetComponent<SphericalMaskPPController>();
        maskPPController.Camera = room.Player.Camera;
        maskPPController.spherePosition = currentChange.television.transform.position;
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


    public float saturation {
        set {
            room.Player.Camera.gameObject.gameObject.GetComponent<PostProcessVolume>().profile.GetSetting<ColorGrading>().saturation.value = value;
        }
    }
}
