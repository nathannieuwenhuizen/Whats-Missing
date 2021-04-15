using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SFXObject))]
public class Chair : PickableRoomObject
{
    private SFXObject sfx;
    [SerializeField]
    private AudioClip hitSound;
    protected override void Awake() {
        base.Awake();
        sfx = GetComponent<SFXObject>();
    }
    private void OnCollisionEnter(Collision other) {
        sfx.Play(hitSound, .2f, false);
    }
}
