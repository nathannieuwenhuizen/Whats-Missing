using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class AreaTextMeshFader : TextMeshFader
{
    public delegate void OnTVTutorial();
    public static OnTVTutorial onTVTurialShow;

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
            onTVTurialShow?.Invoke();
        }
    }
}
