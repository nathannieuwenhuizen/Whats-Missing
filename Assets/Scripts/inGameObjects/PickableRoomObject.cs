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
    protected bool grabbed = false;

    protected Rigidbody rb;     
    private Transform oldParent;
    private string grabbedLayer = "Test";
    private string idleLayer;
    protected override void Awake()
    {
        base.Awake();
        idleLayer = LayerMask.LayerToName(gameObject.layer);
        rb = GetComponent<Rigidbody>();
        RigidBodyInfo.Save(rb);
        grabSound = SFXFiles.player_grab;

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

    protected string grabSound = SFXFiles.player_grab;

    [Header("pickable info")]
    ///<summary>
    /// If the objects will burn in flames if it comes in contact with fire.
    ///</summary>
    [SerializeField]
    private bool flamable = false;

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
        if (grabbed) Release();
        DeactivateRigidBody();
        base.OnMissing();
    }

    public override float CurrentScale { get { return base.CurrentScale; } 
        set {
            base.CurrentScale = value;
            rb.mass = value;
        }  
    }
    protected float holdingDistance = 3f;
    public float HoldingDistance => holdingDistance;

    protected HoldOrientation holdOrientation = HoldOrientation.none;
    public HoldOrientation HoldOrientation => holdOrientation;

    private Vector3 holdingOffset = new Vector3(0,0,0);
    public Vector3 HoldingOffset { get => holdingOffset; set => holdingOffset = value; }
    public Vector3 HoldingPosition { get {
            return RigidBody.transform.position;
    }}

    public Vector3 HoldingLocalOffset { get {
        if (GetComponent<MeshRenderer>() != null) {
            Debug.Log("offset = " + (HoldingPosition - GetComponent<MeshRenderer>().bounds.center));
            return HoldingPosition - GetComponent<MeshRenderer>().bounds.center;
        } else {
            return Vector3.zero;
        }
    }}

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

    public virtual void Grab(Rigidbody connectedRigidBody)
    {
        // oldParent = transform.parent;
        // transform.SetParent(connectedRigidBody.transform.parent);        
        grabbed = true;
        gameObject.layer = LayerMask.NameToLayer(grabbedLayer);
        AudioHandler.Instance?.PlaySound(grabSound); // is this called twice?

        OutlineEnabled = false;
        RigidBodyInfo.Save(rb);
        rb.drag = 100f;
        rb.mass = 0.1f;
        rb.angularDrag = 10f;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        gameObject.tag = Tags.Picked;
    }
    public void UpdateGrabPhysics() {

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
        if (!grabbed) return;

        // transform.SetParent(oldParent);
        gameObject.layer = LayerMask.NameToLayer(idleLayer);
        grabbed = false;
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

    public bool TooHeavy(Hands hands)  {
        if (rb == null) return false;
        bool result = hands.MassThreshhold < rb.mass;
        rb.constraints = result ? RigidbodyConstraints.FreezeRotation : rb.constraints; 
        return result;
    } 

    private void OnTriggerEnter(Collider other) {
        //if the object enters a fire spread and is flamable
        if (other.GetComponent<FireSpread>() != null && flamable)  StartCoroutine(Burn());
    }

    ///<summary>
    /// Object burns to crisps and eventually gets destroyed
    ///</summary>
    public IEnumerator Burn() {
        // OnBlur();
        Outline.enabled = false;
        Interactable = false;
        flamable = false;

        // yield return new WaitForEndOfFrame();
        foreach (Material mat in getMaterials()) {
            if (mat.HasProperty("EdgeColor")) {
                mat.SetColor("EdgeColor", new Color(.7f,.04f,0) * 8f); //set color to hdr orange
                StartCoroutine(mat.AnimatingDissolveMaterial(0,1, AnimationCurve.EaseInOut(0,0,1,1), 3f));
            }
        }
        ShowDisovleParticles(true);
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);

    } 

    public virtual bool CanBeReleased() => true;
    

    #endregion
}
