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
    public override void onMissing()
    {
        StopAllCoroutines();
        base.onMissing();
    }
    public override IEnumerator AnimateMissing()
    {
        yield return base.AnimateMissing();
    }

    public override void onMissingFinish()
    {
        OnAirMissing?.Invoke();
        base.onMissingFinish();
    }

    public override void onAppearing() {
        OnAirAppearing?.Invoke();
        base.onAppearing();
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