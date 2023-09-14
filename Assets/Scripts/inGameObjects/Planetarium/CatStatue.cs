using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatStatue : PickableRoomObject
{
    protected override void Awake()
    {
        base.Awake();
        grabSound = SFXFiles.cat_statue;
    }

    private void Reset() {
        word = "Cat";
        missingChangeEffect = MissingChangeEffect.dissolve;
        flamable = true;
    }
}
