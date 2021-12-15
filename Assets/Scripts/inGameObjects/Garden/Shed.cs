using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shed : RoomObject
{

    public Shed() {
        normalScale = .75f;
        largeScale = 2.80f;
    }
    private void Reset() {
        Word = "shed";
        AlternativeWords = new string[] { "sheds" };
    }
}
