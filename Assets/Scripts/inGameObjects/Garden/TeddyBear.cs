using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeddyBear : InteractabelObject
{

    public delegate void TeddyBearEvent();
    public static TeddyBearEvent OnTeddyBearPickUp;

    [SerializeField]
    private Rigidbody rigidBody;

    [SerializeField]
    private Animator midIslandAnimator;

    public TeddyBear() {
        largeScale = 13f;
    }
    private void Reset() {
        Word = "teddybear";
        AlternativeWords = new string[] {"bear", "teddy"};
    }

    public override void OnEnlarge()
    {
        midIslandAnimator.SetTrigger("show");
        rigidBody.isKinematic = true;
        Interactable = false;
        base.OnEnlarge();
    }

    public override void OnEnlargingFinish()
    {
        base.OnEnlargingFinish();
    }

    public override void Interact()
    {
        Debug.Log("teddy bear cutscene start");
        base.Interact();
        OnTeddyBearPickUp?.Invoke();
    }

    public override void OnEnlargeRevertFinish()
    {
        Debug.Log("teddy is now small");
        Interactable = true;
        rigidBody.isKinematic = false;
        base.OnEnlargeRevertFinish();
    }

}
