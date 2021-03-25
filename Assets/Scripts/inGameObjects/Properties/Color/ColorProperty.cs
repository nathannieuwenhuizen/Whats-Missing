using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class ColorProperty : Property
{

    [SerializeField]
    private Room room;

    public override void onMissing()
    {
        base.onMissing();
        saturation = -100f;
    }
    public override void onAppearing()
    {
        base.onAppearing();
        saturation = 0;

    }

    public float saturation {
        set {
            room.Player.Camera.gameObject.gameObject.GetComponent<PostProcessVolume>().profile.GetSetting<ColorGrading>().saturation.value = value;
        }
    }
}
