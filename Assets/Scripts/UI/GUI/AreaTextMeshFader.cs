using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class AreaTextMeshFader : TextMeshFader
{
    public delegate void OnTVTutorial();
    public static OnTVTutorial onMirrorTutorialShow;

    [SerializeField]
    private bool isTVTutorial = false;
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
}
