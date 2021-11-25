using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class Chair : RoomObject
{
    private Rigidbody rb;


    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision other) {
        if (rb == null) return;
        // AudioHandler.Instance.Player3DSound(SFXFiles.chair_hit, transform, .05f * Mathf.Min(1, rb.velocity.magnitude / 5f),1f, false, true, 30);
    }
}
