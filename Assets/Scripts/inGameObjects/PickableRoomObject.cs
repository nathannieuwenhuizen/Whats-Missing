using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableRoomObject : RoomObject, IPickable
{

    private Rigidbody rb;
    void Awake()
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

    private float mass = 0;


    // Update is called once per frame
    void Update()
    {
        
    }
}
