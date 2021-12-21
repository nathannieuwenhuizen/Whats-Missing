using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeddyBear : InteractabelObject
{

    public delegate void TeddyBearEvent();
    public static TeddyBearEvent OnTeddyBearPickUp;
    public static TeddyBearEvent OnCutsceneEnd;
    public static TeddyBearEvent OnTeddyBearEnlarged;

    private bool inCutScene = false;


    [SerializeField]
    private Room room;

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
        OutlineEnabled = false;
        OnTeddyBearEnlarged?.Invoke();
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

        if (inCutScene) return;
        inCutScene = true;

        OnTeddyBearPickUp?.Invoke();
        room.Player.Movement.EnableWalk = false;
        room.Player.Movement.EnableRotation = false;
        StartCoroutine(TeddybearCutscene());
    }

    public IEnumerator TeddybearCutscene() {
        yield return new WaitForSeconds(1f);
        OnCutsceneEnd?.Invoke();
    }

    public override void OnEnlargeRevertFinish()
    {
        Debug.Log("teddy is now small");
        Interactable = true;
        OutlineEnabled = true;
        rigidBody.isKinematic = false;
        base.OnEnlargeRevertFinish();
    }

}
