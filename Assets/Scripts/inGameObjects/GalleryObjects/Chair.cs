using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : PickableRoomObject
{
    private void OnCollisionEnter(Collision other) {
        if (rb == null) return;
        AudioHandler.Instance.Player3DSound(SFXFiles.chair_hit, transform, .2f * Mathf.Min(1, RigidBody.velocity.magnitude / 5f),1f, false, true, 30);
    }
}
