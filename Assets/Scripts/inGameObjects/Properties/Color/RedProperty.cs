using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class RedProperty : Property
{
    [SerializeField]
    private Room room;

    public override void OnMissing()
    {
        base.OnMissing();
        redValue = 0;
    }
    public override void OnAppearing()
    {
        base.OnAppearing();
        redValue = 100f;

    }

    public float redValue {
        set {
            room.Player.Camera.gameObject.gameObject.GetComponent<PostProcessVolume>().profile.GetSetting<ColorGrading>().mixerRedOutRedIn.value = value;
        }
    }
    private void Reset() {
        Word = "red";
    }

}
