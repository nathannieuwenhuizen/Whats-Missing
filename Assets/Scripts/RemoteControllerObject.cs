using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteControllerObject : InteractabelObject
{

    [SerializeField]
    private IntroSceneHandeler handeler;

    public override void Interact()
    {
        base.Interact();
        handeler.PickUpRemote();
        gameObject.SetActive(false);
    }
}
