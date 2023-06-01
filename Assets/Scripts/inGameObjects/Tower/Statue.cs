using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : PickableRoomObject
{
    protected override void Awake()
    {
        base.Awake();
        largeScale = 1.4f;
        shrinkScale = .3f;
    }
}
