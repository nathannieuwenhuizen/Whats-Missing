using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Sofa : ParentRoomObject
{
    public Sofa()
    {
        largeScale = 200;
    }

    private void Reset() {
        Word = "sofa";
        AlternativeWords = new string[] { "sofas", "chair", "chairs" };
        flippingAxis = FlippingAxis.up; 
    }
}
