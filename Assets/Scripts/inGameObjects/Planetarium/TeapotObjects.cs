using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeapotObjects : PickableRoomObjectThatplaysSound
{
    public override string AudioFile()
    {
        return SFXFiles.teacup_falling;
    }
}
