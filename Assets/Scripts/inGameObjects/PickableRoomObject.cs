using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//cant require rigdibody because then it cant be deleted when it gets disappeared.
public class PickableRoomObject : InteractabelObject, IPickable
{
    private Rigidbody rb; 
    [SerializeField]   
    private float mass = 1;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        rb.mass = mass;
    }
    public Rigidbody RigidBody { 
        get {
            return rb;
        } 
        set {
            rb = value;
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



    public override void Interact()
    {
        base.Interact();
    }

    public void Grab()
    {
        Destroy(rb);
    }

    public void Release()
    {
        if (rb == null) {
            rb = GetComponent<Rigidbody>();
            if (rb == null) {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.mass = mass;
        }
        rb.isKinematic = false;
    }
}
