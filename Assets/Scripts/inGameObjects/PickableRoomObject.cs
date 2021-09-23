using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//cant require rigdibody because then it cant be deleted when it gets disappeared.
public class PickableRoomObject : InteractabelObject, IPickable
{
    private Rigidbody rb; 
    [SerializeField]   
    private float mass = 1;
    public float Mass { get { return mass; } set {mass = value; } }

    private bool useGravity;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        rb.mass = mass;
    }
    public Rigidbody RigidBody { 
        get {
            if (rb == null) {
                Debug.LogError("Rigibody of pickable object called while it doesn't exist.");
                return default(Rigidbody);
            }
            return rb;
        } 
        set {
            rb = value;
        }
    }

    public override void onAppearing()
    {
        base.onAppearing();
    }
    public override void onAppearingFinish()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.useGravity = useGravity;
        base.onAppearingFinish();
    }

    public override void onMissing()
    {
        useGravity = rb.useGravity;
        Destroy(rb);
        base.onMissing();
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


    public override void Interact()
    {
        base.Interact();
    }

    public void Grab()
    {
        Debug.Log("grab" + rb.useGravity);
        useGravity = rb.useGravity;
        Destroy(rb);
    }

    public void Release()
    {
        if (rb == null) {
            rb = GetComponent<Rigidbody>();
            if (rb == null) {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.useGravity = useGravity;
            }
            rb.mass = mass;
        }
        rb.isKinematic = false;
    }
}
