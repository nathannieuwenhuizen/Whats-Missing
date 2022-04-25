using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DestroyTrigger : MonoBehaviour, IRoomObject
{

    private Collider coll;
    private Rigidbody rb;

    public bool InSpace { get; set; }

    private void Awake() {
        coll = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    public void OnRoomEnter()
    {
        coll.enabled = true;
    }

    public void OnRoomLeave()
    {
        coll.enabled = false;
    }

    private void OnCollisionEnter(Collision other) {
        RoomObject changable = other.gameObject.GetComponent<RoomObject>();
        if (changable != null) {
            Player player = changable.Transform.GetComponent<Player>();
            if (player != null) {
                player.Die(false);
            } else {
                Destroy(changable.Transform.gameObject);
            }
        }
    }
}
