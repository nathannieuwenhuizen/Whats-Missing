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

    public bool InsideArea { get; set; } = false;

    private bool inSpace = false;
    public bool InSpace {get => inSpace; set => inSpace = value; }

    protected override void Start() {
        base.Start();
        Active = false;
    }
    public override void FadeIn()
    {
        base.FadeIn();
        if (isTVTutorial) {
            isTVTutorial = false;
            onMirrorTutorialShow?.Invoke();
        }
    }

    public void OnAreaEnter(Player player)
    {
        if (inSpace) {
            FadeIn();
        }
        InsideArea = true;
    }

    public void OnAreaExit(Player player)
    {
        InsideArea = false;
    }

    public void OnRoomEnter()
    {
        inSpace = true;
    }

    public void OnRoomLeave()
    {
        inSpace = false;
    }
}
