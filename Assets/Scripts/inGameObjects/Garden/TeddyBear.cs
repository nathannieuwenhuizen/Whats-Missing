using System;
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
    private Transform endScenePosition;

    [SerializeField]
    private Animator midIslandAnimator;

    public TeddyBear() {
        largeScale = 13f;
    }
    private void Reset() {
        Word = "teddybear";
        AlternativeWords = new string[] {"bear", "teddy"};
    }
    private void Awake() {
        rigidBody.isKinematic = true;
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
        Focused = false;
        Interactable = false;
        OutlineEnabled = false;
        

        room.Player.Movement.EnableWalk = false;
        room.Player.Movement.RB.useGravity = false;
        room.Player.Movement.EnableRotation = false;
        room.Player.Movement.enabled = false;
        StartCoroutine(room.Player.transform.AnimatingPos(endScenePosition.position, AnimationCurve.EaseInOut(0,0,1,1), 1f));
        StartCoroutine(room.Player.transform.AnimatingRotation(endScenePosition.rotation, AnimationCurve.EaseInOut(0,0,1,1), 1f));
        StartCoroutine(TeddybearCutscene());

        StartCoroutine(PlayCutsceneAudio(1f , () => {
            Debug.Log("audio gets played!");
            AudioHandler.Instance.PlaySound(SFXFiles.knee_on_dirt, .2f, .9f);
        }));
        StartCoroutine(PlayCutsceneAudio(3f , () => {
            AudioHandler.Instance.PlaySound(SFXFiles.pulling_pocket, .5f);
        }));
        StartCoroutine(PlayCutsceneAudio(5.4f , () => {
            AudioHandler.Instance.PlaySound(SFXFiles.music_box, .3f);
        }));
        StartCoroutine(PlayCutsceneAudio(7.8f , () => {
            AudioHandler.Instance.PlaySound(SFXFiles.music_box_on_ground, 1f);
        }));
        StartCoroutine(PlayCutsceneAudio(8f , () => {
            AudioHandler.Instance.PlayUISound(SFXFiles.gregory_cry, .7f);
        }));
        StartCoroutine(PlayCutsceneAudio(11f , () => {
            AudioHandler.Instance.FadeListener(0, 5f);
        }));
    }
    public IEnumerator PlayCutsceneAudio(float delay, Action callback) {
        yield return new WaitForSeconds(delay);
        callback();
    }

    public IEnumerator TeddybearCutscene() {
        room.Player.SetLevel2EndAnimation();
        yield return new WaitForSeconds(11f);
        OnTeddyBearPickUp?.Invoke();
        yield return new WaitForSeconds(5f);
        OnCutsceneEnd?.Invoke();
    }

    public override void OnEnlargeRevertFinish()
    {
        Debug.Log("teddy is now small");
        Interactable = true;
        OutlineEnabled = true;
        // rigidBody.isKinematic = false;
        base.OnEnlargeRevertFinish();
    }

}
