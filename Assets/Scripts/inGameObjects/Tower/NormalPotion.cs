using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalPotion : PickableRoomObjectThatplaysSound
{
    public override string AudioFile()
    {
        return SFXFiles.normal_potion;
    }

    private void Reset() {
        Word = "Potion";
        AlternativeWords = new string[] { "potions", "glass" };
    }
}
