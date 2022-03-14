using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class AreaTextMeshFader : TextMeshFader, ITriggerArea, IRoomObject
{
    public delegate void OnTVTutorial();
    public static OnTVTutorial onMirrorTutorialShow;

    [SerializeField]
    private bool isTVTutorial = false;

    private bool hasBeenFaded = false; 

    public bool InsideArea { get; set; } = false;

    public bool InSpace {get; set; } = false;

    protected override void Start() {
        base.Start();
        Active = false;
    }
    public override void FadeIn()
    {
        if (hasBeenFaded) return;
        hasBeenFaded = true;

        base.FadeIn();
        if (isTVTutorial) {
            isTVTutorial = false;
            onMirrorTutorialShow?.Invoke();
        }
    }

    public void OnAreaEnter(Player player)
    {
        if (InSpace) {
            FadeIn();
        }
        InsideArea = true;
    }

    public void OnAreaExit(Player player)
    {
        // if (inSpace) {
        //     FadeOut();
        // }
        InsideArea = false;
    }

    public void OnRoomEnter()
    {
    }

    public void OnRoomLeave()
    {
    }
}
