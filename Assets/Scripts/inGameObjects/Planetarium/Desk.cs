using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desk : RoomObject
{
    public Desk()
    {
        largeScale = 200;
    }

    private void Reset() {
        Word = "desk";
        AlternativeWords = new string[] { "desks", "bureau", "table" };
    }

}
