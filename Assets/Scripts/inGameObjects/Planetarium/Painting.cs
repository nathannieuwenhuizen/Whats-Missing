using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painting : RoomObject
{

    [SerializeField]
    private MeshRenderer meshObject;
    [SerializeField]
    private MeshRenderer button;
    [SerializeField]
    private Portal[] portals;
    [SerializeField]
    private GameObject hiddenRoom;


    private void Reset() {
        Word = "painting";
        AlternativeWords = new string[] {"portret", "illustration", "paintings", "drawing", "illustrations"};
    }

    protected void Awake() {
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

    public override void OnMissing()
    {
        base.OnMissing();
        SetPortalsActive(true);
    }
    public override void OnMissingFinish()
    {
        //no base call!
        meshObject.enabled = false;
        button.enabled = false;
    }

    public override void OnAppearing()
    { 
        //no base call!
        meshObject.enabled = true;
        button.enabled = true;
        base.OnAppearing();
    }

    public override void OnAppearingFinish()
    {
        base.OnAppearingFinish();
        SetPortalsActive(false);

    }


}
