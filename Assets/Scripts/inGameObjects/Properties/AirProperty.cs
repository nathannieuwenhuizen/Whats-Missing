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
        AudioHandler.Instance.MusicSource.Pause();
        base.OnMissing();
        OnAirMissing?.Invoke();
    }
    public override IEnumerator AnimateMissing()
    {
        yield return base.AnimateMissing();
    }

    public override void onMissingFinish()
    {
        base.onMissingFinish();
    }

    public override void OnAppearing() {
        AudioHandler.Instance.MusicSource.Play();
        OnAirAppearing?.Invoke();
        base.OnAppearing();
    }
    



    public override void onAppearingFinish()
    {
        base.onAppearingFinish();
    }

    private void Reset() {
        Word = "air";
        AlternativeWords = new string[] { "oxygen" , "wind" };
    }

}