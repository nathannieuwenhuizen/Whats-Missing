using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painting : InteractabelObject
{
    public delegate void PaintingEvent();
    public static PaintingEvent OnInteract;

    [SerializeField]
    private PaintingButton paintingButton;

    [SerializeField]
    private MeshRenderer meshObject;
    [SerializeField]
    private Collider coll;
    [SerializeField]
    private MeshRenderer button;
    [SerializeField]
    private Portal[] portals;
    [SerializeField]
    private GameObject hiddenRoom;
    private Animator animator;
    private bool open = false;
    private bool animating = false;

    private bool wasMissing = false;

    private Rigidbody rigidBody;

    protected override void Awake() {
        base.Awake();
        OutlineEnabled = false;
        Interactable = false;
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = true;
        if (!wasMissing && !open) SetPortalsActive(false);
    }
    public override void OnRoomLeave()
    {
        base.OnRoomLeave();
        if (wasMissing) SetPortalsActive(false);
    }
    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        if (wasMissing || open) SetPortalsActive(true);
        else SetPortalsActive(false);
    }

    private void OnEnable() {
        Stairs.OnStairsMissing += DetatchFromStairs;
        Gravity.onGravityMissing += DetatchFromStairs;
    }
    private void OnDisable() {
        Stairs.OnStairsMissing -= DetatchFromStairs;
        Gravity.onGravityMissing -= DetatchFromStairs;
    }

    private void DetatchFromStairs() {
        if (!inSpace) return;
        Interactable = false;
        paintingButton.Interactable = false;
        paintingButton.OutlineEnabled = false;
        rigidBody.isKinematic = false;
    }

    private void SetPortalsActive(bool val) {
        if (hiddenRoom == null) return;
        Debug.Log("set portals active: " + val);
        
        hiddenRoom.SetActive(val);
        foreach (Portal portal in portals)
            portal.gameObject.SetActive(val);
    }

    protected override void OnFocus() {
        // Debug.Log("focus!" + OutlineEnabled);
    }

    public override void Interact()
    {
        OnInteract?.Invoke();
        if (animating) return;
        animating = true;
        open = true;

        if (hiddenRoom != null) {
            SteamAchievementHandler.Instance?.SetAchievement(SteamAchievement.EarlyBirdCatchesTheApple);
            animator.SetBool("open", open);
            SetPortalsActive(true);
            StartCoroutine(WaitBeforeAnimationFinish());
        }

        base.Interact();
    }
    private IEnumerator WaitBeforeAnimationFinish() {
        yield return new WaitForSeconds(2f);
        animating = false;
    }

    public override void OnMissing()
    {
        base.OnMissing();
        SetPortalsActive(true);
        wasMissing = true;
    }
    public override void OnMissingFinish()
    {
        //no base call!
        meshObject.enabled = false;
        coll.enabled = false;
        button.enabled = false;
    }

    public override void OnAppearing()
    { 
        //no base call!
        meshObject.enabled = true;
        coll.enabled = true;
        button.enabled = true;
        paintingButton.Interactable = false;
        paintingButton.OutlineEnabled = false;
        // OutlineEnabled = true;
        base.OnAppearing();
    }

    public override void OnAppearingFinish()
    {
        base.OnAppearingFinish();
        DetatchFromStairs();
        // if (!open) SetPortalsActive(false);

    }

    private void Reset() {
        Word = "painting";
        AlternativeWords = new string[] {"portrait", "illustration", "paintings", "drawing", "illustrations"};
    }


}
