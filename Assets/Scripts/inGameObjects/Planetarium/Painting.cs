using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painting : InteractabelObject
{

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


    private void Reset() {
        Word = "painting";
        AlternativeWords = new string[] {"portret", "illustration", "paintings", "drawing", "illustrations"};
    }

    protected void Awake() {
        animator = GetComponent<Animator>();
        // SetPortalsActive(false);
    }
    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        SetPortalsActive(false);
    }


    private void Start() {
        
    }  

    private void SetPortalsActive(bool val) {
        hiddenRoom.SetActive(val);
        foreach (Portal portal in portals)
            portal.gameObject.SetActive(val);
    }

    public override void Interact()
    {
        
        open = !open;
        animator.SetBool("open", open);
        SetPortalsActive(true);

        base.Interact();
    }

    public override void OnMissing()
    {
        base.OnMissing();
        SetPortalsActive(true);
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
        base.OnAppearing();
    }

    public override void OnAppearingFinish()
    {
        base.OnAppearingFinish();
        if (!open) SetPortalsActive(false);

    }


}