using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

///<summary>
/// The enviroment lighting property of the whole room.
///</summary>
public class AirProperty : Property
{

    public static OnPropertyToggle OnAirMissing;
    public static OnPropertyToggle OnAirAppearing;

    public override void OnMissing()
    {
        AudioHandler.Instance.PauseMusic();
        base.OnMissing();
        OnAirMissing?.Invoke();
    }
    public override IEnumerator AnimateMissing()
    {
        yield return base.AnimateMissing();
    }

    public override void OnMissingFinish()
    {
        base.OnMissingFinish();
    }

    public override void OnAppearing() {
        AudioHandler.Instance.ResumeMusic();
        OnAirAppearing?.Invoke();
        base.OnAppearing();
    }
    



    public override void OnAppearingFinish()
    {
        base.OnAppearingFinish();
    }

    private void Reset() {
        Word = "air";
        AlternativeWords = new string[] { "oxygen" , "wind" };
    }

}