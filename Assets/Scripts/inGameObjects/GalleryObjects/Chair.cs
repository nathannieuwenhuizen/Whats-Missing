using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : PickableRoomObject
{
    protected override void Awake() {
        base.Awake();
    }
    private void OnCollisionEnter(Collision other) {
        if (rb == null) return;
        AudioHandler.Instance.PlaySound(SFXFiles.chair_hit, .2f * Mathf.Min(1, RigidBody.velocity.magnitude / 5f));
    }
}
