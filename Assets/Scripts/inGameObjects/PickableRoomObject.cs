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
    public bool IsKinematic = true;
    public string tag = "";
    public void Save(Rigidbody rb) {
        Drag = rb.drag;
        AngularDrag = rb.angularDrag;
        Mass = rb.mass;
        RigidbodyConstraints = rb.constraints;
        Holdvelocity = rb.velocity;
        tag = rb.gameObject.tag;
        UseGravity = rb.useGravity;
        IsKinematic = rb.isKinematic;
    }
    public void Load(Rigidbody rb) {
        rb.drag = Drag;
        rb.angularDrag = AngularDrag;
        rb.mass = Mass;
        rb.constraints = RigidbodyConstraints;
        rb.velocity = Holdvelocity;
        rb.gameObject.tag = tag;
        rb.isKinematic = IsKinematic;
    }
}

///<summary>
/// A physical object inside of the room that can be picked and has a rigidbody.
/// The rigidbody isn't always available because it gets deleted when grabbed.
///</summary>
public class PickableRoomObject : InteractabelObject, IPickable
{
    protected Rigidbody rb;     
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        RigidBodyInfo.Save(rb);
        // DeactivateRigidBody();
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

    private bool touching = false;
    public bool Touching => touching;

    public override void OnAppearing()
    {
        base.OnAppearing();
    }
    public override void OnAppearingFinish()
    {
        ActivateRigidBody();
        base.OnAppearingFinish();
    }

    public override void OnMissing()
    {
        DeactivateRigidBody();
        base.OnMissing();
    }

    public override float CurrentScale { get { return base.CurrentScale; } 
        set {
            base.CurrentScale = value;
            rb.mass = value;
        }  
    }


    protected override void OnFocus()
    {
        //todo: check if mass isnt too high with what the player can handle.
        base.OnFocus();
    }

    private void FixedUpdate() {
        touching = false;
    }

    private void OnCollisionStay(Collision other) {
        touching = true;
    }

    public void Grab(Rigidbody connectedRigidBody)
    {        
        OutlineEnabled = false;
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
    /// Releases the object activating the body.
    ///</summary>
    public virtual void Release()
    {
        OutlineEnabled = true;
        ActivateRigidBody();
    }

    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        // ActivateRigidBody();
    }
    public override void OnRoomLeave()
    {
        base.OnRoomLeave();
        // DeactivateRigidBody();
    }

    #region flipped
    public override void OnFlipped()
    {
        DeactivateRigidBody();
        base.OnFlipped();
    }
    public override void OnFlippingFinish()
    {
        ActivateRigidBody();
        base.OnFlippingFinish();
    }

    public override void OnFlippingRevert()
    {
        DeactivateRigidBody();
        base.OnFlippingRevert();
    }

    public override void OnFlippingRevertFinish()
    {
        ActivateRigidBody();
        base.OnFlippingRevertFinish();
    }

    public bool TooHeavy(Hands hands) => hands.MassThreshhold < rb.mass;
    
    #endregion
}
