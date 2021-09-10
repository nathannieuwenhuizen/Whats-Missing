using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//cant require rigdibody because then it cant be deleted when it gets disappeared.
public class PickableRoomObject : InteractabelObject, IPickable
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

    protected override void OnFocus()
    {
        //todo: check if mass isnt too high with what the player can handle.
        base.OnFocus();
    }

    protected override void OnBlur()
    {
        base.OnBlur();
    }

    public float Mass { get { return mass; } set {mass = value; } }


    private float mass = 0;

    public override void Interact()
    {
        base.Interact();
        // throw new System.NotImplementedException();
    }
}
