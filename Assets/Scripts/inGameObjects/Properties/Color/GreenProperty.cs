using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GreenProperty : Property
{
    [SerializeField]
    private Room room;

    public override void OnMissing()
    {
        base.OnMissing();
        greenValue = 0;
    }
    public override void OnAppearing()
    {
        base.OnAppearing();
        greenValue = 100f;

    }

    public float greenValue {
        set {
            // room.Player.Camera.gameObject.gameObject.GetComponent<PostProcessVolume>().profile.GetSetting<ColorGrading>().mixerRedOutGreenIn.value = value;
            room.Player.Camera.gameObject.gameObject.GetComponent<PostProcessVolume>().profile.GetSetting<ColorGrading>().mixerGreenOutGreenIn.value = value;
        }
    }
    private void Reset() {
        Word = "green";
    }

}
