using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : RoomObject
{

    protected override void Awake()
    {
        base.Awake();
        largeScale = 1f;
        shrinkScale = 1f;
    }
    private void Reset() {
        Word = "bridge";
        AlternativeWords = new string[] { "bridges" };
    }
}
