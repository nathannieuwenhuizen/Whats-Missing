using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///<summary>
/// A physical object inside of the room that can be picked and has a rigidbody.
/// The rigidbody isn't always available because it gets deleted when grabbed.
///</summary>

public class PickableRoomObject : InteractabelObject, IPickable
{
    protected Rigidbody rb; 
    [SerializeField]   
    private float mass = 1;
    public float Mass { get { return mass; } set {mass = value; } }

    private bool useGravity;

    public Vector3 HoldVelocity {
        get => holdVelocity;
        set => holdVelocity = value;
    }
    private Vector3 holdVelocity;

    

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
        ActivateRigidBody();
        base.onAppearingFinish();
    }

    public override void onMissing()
    {
        DeactivateRigidBody();
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
        DeactivateRigidBody();
    }

    ///<summary>
    /// Deletes the rigidbody while saving its values inside this class.
    ///</summary>
    public void DeactivateRigidBody() {
        if (rb == null) return;
        useGravity = rb.useGravity;
        holdVelocity = rb.velocity;
        Destroy(rb);
    }
    ///<summary>
    /// Makes a new rigidbody and sets its data to that of the values stored inside this class.
    ///</summary>
    public void ActivateRigidBody() {
        if (rb == null) {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.mass = mass;
        rb.useGravity = useGravity;
        rb.velocity = holdVelocity;
    }
    ///<summary>
    /// Releases the object activating the body only when the timescale of the room isn't 0.
    ///</summary>
    public void Release()
    {
        if (Room.TimeScale == 0) return;
        ActivateRigidBody();
        rb.isKinematic = false;
    }
}
