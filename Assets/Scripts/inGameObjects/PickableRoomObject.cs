using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractabelObject))]
[RequireComponent(typeof(Rigidbody))]
public class PickableRoomObject : RoomObject, IPickable
{

    private Rigidbody rb;
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }
    public Rigidbody RigidBody { 
        get {
            return rb;
        } 
    }

    public override IEnumerator Appearing()
    {
        yield return base.Appearing();
        rb = gameObject.AddComponent<Rigidbody>();
    }

    public override IEnumerator Dissappearing()
    {
        Destroy(rb);
        yield return base.Dissappearing();

    }

    public float Mass { get { return mass; } set {mass = value; } }

    public bool Focused { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private float mass = 0;

    public void Interact()
    {
        throw new System.NotImplementedException();
    }
}
