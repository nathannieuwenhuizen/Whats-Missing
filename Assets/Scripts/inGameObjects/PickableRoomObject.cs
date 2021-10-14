using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///<summary>
/// Holds the values of the rigidbody and saves it when the rigidbody gets deleted for later use.
///</summary>
[System.Serializable]
public class RigidBodyInfo {
    public float Drag = 0f;
    public float AngularDrag = 0.05f;
    public float Mass = 1;
    public RigidbodyConstraints RigidbodyConstraints = RigidbodyConstraints.None;
    public Vector3 Holdvelocity = Vector3.zero;
    public bool UseGravity = true;
    public string tag = "";
    public void Save(Rigidbody rb) {
        Drag = rb.drag;
        AngularDrag = rb.angularDrag;
        Mass = rb.mass;
        RigidbodyConstraints = rb.constraints;
        Holdvelocity = rb.velocity;
        tag = rb.gameObject.tag;
        UseGravity = rb.useGravity;
    }
    public void Load(Rigidbody rb) {
        rb.drag = Drag;
        rb.angularDrag = AngularDrag;
        rb.mass = Mass;
        rb.constraints = RigidbodyConstraints;
        rb.velocity = Holdvelocity;
        rb.gameObject.tag = tag;
    }
}

///<summary>
/// A physical object inside of the room that can be picked and has a rigidbody.
/// The rigidbody isn't always available because it gets deleted when grabbed.
///</summary>
public class PickableRoomObject : InteractabelObject, IPickable
{
    private SpringJoint joint; 
    protected Rigidbody rb; 
    
    
    public int gameObjectID;


    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        RigidBodyInfo.Save(rb);
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
    [SerializeField]
    private RigidBodyInfo rigidBodyInfo = new RigidBodyInfo();
    public RigidBodyInfo RigidBodyInfo { get => rigidBodyInfo; set => rigidBodyInfo = value; }

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

    public void Grab(Rigidbody connectedRigidBody)
    {
        //create spring
        joint = gameObject.AddComponent<SpringJoint>();
        joint.connectedBody = connectedRigidBody;
        // joint.minDistance = 2f;
        // joint.maxDistance = 2f;
        joint.anchor = new Vector3(.5f,.5f,.5f);
        joint.spring = 500;
        joint.connectedAnchor = connectedRigidBody.transform.InverseTransformDirection(transform.position- connectedRigidBody.transform.position);
        

        if (Room.TimeScale == 0) {
            ActivateRigidBody();
        }
        RigidBodyInfo.Save(rb);
        rb.drag = 100f;
        rb.mass = 0.1f;
        rb.angularDrag = 10f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        gameObject.tag = Tags.Picked;
    }

    ///<summary>
    /// Deletes the rigidbody while saving its values inside this class.
    ///</summary>
    public void DeactivateRigidBody(bool saveInfo = true) {
        if (rb == null) return;

        if (saveInfo) RigidBodyInfo.Save(rb);
        Destroy(rb);
    }
    ///<summary>
    /// Makes a new rigidbody and sets its data to that of the values stored inside this class.
    ///</summary>
    public void ActivateRigidBody() {
        if (rb == null) {
            if (GetComponent<Rigidbody>() != null) {
                rb = GetComponent<Rigidbody>();
            } else 
                rb = gameObject.AddComponent<Rigidbody>();
        }
        RigidBodyInfo.Load(rb);
    }
    ///<summary>
    /// Releases the object activating the body only when the timescale of the room isn't 0.
    ///</summary>
    public void Release()
    {

        joint.connectedBody = null;
        joint.breakForce = 0;

        if (Room.TimeScale == 0) {
            Destroy(joint);
            DeactivateRigidBody(false);
        } else {
            ActivateRigidBody();
            rb.isKinematic = false;
        }
    }
}
