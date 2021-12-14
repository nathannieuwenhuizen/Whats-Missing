using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shed : RoomObject
{

    public Shed() {
        normalScale = 75;
        largeScale = 280;
    }
    private void Reset() {
        Word = "shed";
        AlternativeWords = new string[] { "sheds" };
    }
}
