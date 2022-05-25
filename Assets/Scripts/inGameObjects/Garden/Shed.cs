using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shed : RoomObject
{

    public Shed() {
        normalScale = .75f;
        largeScale = 2.80f;
    }

    [SerializeField]
    private Door door;
    private void Reset() {
        Word = "shed";
        AlternativeWords = new string[] { "sheds", "house", "home", "building", "homes", "houses", "buildings" };
    }

    public override IEnumerator AnimateEnlargeRevert()
    {
        door.Interactable = false;
        return base.AnimateEnlargeRevert();
    }
    public override void OnEnlargeRevertFinish()
    {
        base.OnEnlargeRevertFinish();
        door.Interactable = true;
    }

    public override IEnumerator AnimateEnlarging()
    {
        door.Interactable = false;
        return base.AnimateEnlarging();
    }
    public override void OnEnlargingFinish()
    {
        base.OnEnlargingFinish();
        door.Interactable = true;
    }

    

}
