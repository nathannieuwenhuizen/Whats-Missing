using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeddyBear : InteractabelObject
{


    public TeddyBear() {
        largeScale = 7f;
    }
    private void Reset() {
        Word = "teddybear";
        AlternativeWords = new string[] {"bear", "teddy"};
    }

    public override void OnEnlargingFinish()
    {
        Interactable = false;
        base.OnEnlargingFinish();
    }

    public override void OnEnlargeRevertFinish()
    {
        Interactable = true;
        base.OnEnlargeRevertFinish();
    }

}
