using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
/// A potion that when hitting a changable object, introduces a change ot that object it crashes into.
///</summary>
public class Potion : PickableRoomObject
{
    public delegate void  PotionCrashEvent(Potion potion, IChangable changable);
    public delegate void  PotionEvent(Potion potion);
    public static PotionCrashEvent OnChanging;
    public static PotionEvent OnBreak;

    [SerializeField]
    private ParticleSystem burstParticle;
    [SerializeField]
    private ParticleSystem smokeParticle;

    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private Transform aimHitObject;
    [SerializeField]
    private GameObject potionMeshObject;

    private int linePointCount = 300;
    
    [SerializeField]
    private ChangeType changeType;

    public ChangeType ChangeType {
        get { return changeType;}
    }
    private float forcePower = 1000f;

    private bool thrown = false;
    private bool broken = false;

    private void Reset() {
        Word = "potion";
        AlternativeWords = new string[] { "potions", "brew" };
    }

    protected override void Awake()
    {
        base.Awake();
        grabSound = SFXFiles.potion_grab;
        ClearAimLine();
        StartCoroutine(DeactivateRigidVelocity());
        InteractableDistance = 15f;
        HoldingOffset = new Vector3(-1f, 0f, -1f);

    }

    private void Start() {
        holdOrientation = HoldOrientation.potion;
    }

    public IEnumerator DeactivateRigidVelocity() {
        rb.isKinematic = true;
        yield return new WaitForSeconds(.5f);
        rb.isKinematic = false;
    }

    public override void OnRoomEnter()
    {
        inSpace = true;
        smokeParticle.Play();
        base.OnRoomEnter();
    }

    private void Update() {
        if (!broken) {
            smokeParticle.transform.rotation = Quaternion.Euler(0,0,0);
        }
    }

    public override void OnRoomLeave()
    {
        inSpace = false;
        smokeParticle.Stop();
        base.OnRoomLeave();
    }


    public override void Grab(Rigidbody connectedRigidBody)
    {
        base.Grab(connectedRigidBody);
        StartCoroutine(UpdatingAimLine());
    }

    public override void Release()
    {
        base.Release();
        AudioHandler.Instance?.PlaySound(SFXFiles.potion_throw);
        Vector3 delta =  Camera.main.transform.forward;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(delta * forcePower * rb.mass);
        rb.drag = 0;
        rb.angularDrag = 0;
        rb.angularVelocity = transform.InverseTransformDirection(new Vector3(0,50,0));
        thrown = true;
    }


    private void OnCollisionEnter(Collision other) {
        if (!thrown || broken) return;
        broken = true;
        IChangable changable = getPotentialIChangable(other);
        if (changable != null) {
            if (changable.Transform.GetComponent<RoomObject>() != null) 
                if (changable.Transform.GetComponent<RoomObject>().AffectedByPotions == false) {
                    Break();
                    return;
                }
            OnChanging?.Invoke(this, changable);
        }
        Break();
    }

    private IChangable getPotentialIChangable(Collision other)
    {        
        //check first in children
        IChangable changable = other.transform.GetComponentInChildren<IChangable>();
        if (changable == null) 
            //then itself
            changable = other.transform.GetComponent<IChangable>();
        if (changable == null) 
            //then its parent
            changable = other.transform.parent.GetComponent<IChangable>();
        return changable;
    }

    IEnumerator UpdatingAimLine() {
        lineRenderer.enabled = true;
        aimHitObject.transform.parent = transform.parent;

        while (thrown == false) {
            AimLineAlpha = 1;
            Vector3 delta =  -Camera.main.transform.forward * forcePower * rb.mass;
            UpdateAimLine(delta, rb, transform.position);
            yield return new WaitForEndOfFrame();
        }
        // ClearAimLine();

    }

    ///<summary>
    /// Updates the aimline by performing a formula with gravity on the physics rigidbody
    ///</summary>
    public void UpdateAimLine(Vector3 forceVector, Rigidbody rigidbody, Vector3 startingPoint) {
        List<Vector3> linePoints =new List<Vector3>();

        Vector3 velocity = (forceVector / rigidbody.mass) * Time.fixedDeltaTime;

        // float flightDuration = (2  * -Mathf.Abs( Mathf.Min(100, velocity.y))) / Physics.gravity.y;
        float stepTime =  .1f;
        bool hitted = false;

        Vector3 oldPos = startingPoint;
        for (int i = 0 ; i < linePointCount; i++) {
            float stepTimePassed = stepTime * i;
            Vector3 movementVector = new Vector3(
                velocity.x * stepTimePassed,
                velocity.y * stepTimePassed - .5f * (Physics.gravity.y * 1.0f) * stepTimePassed * stepTimePassed,
                velocity.z * stepTimePassed
            );
            RaycastHit hit;
            if (Physics.SphereCast(startingPoint -movementVector,.5f, ((startingPoint -movementVector) -oldPos).normalized * .1f, out hit, ((startingPoint -movementVector) -oldPos).magnitude)) {
                if (hit.transform.parent != transform) {
                    linePoints.Add(hit.point);
                    hitted = true;
                    aimHitObject.transform.position = hit.point;
                    aimHitObject.transform.rotation = Quaternion.LookRotation(aimHitObject.forward, hit.normal);;
                    break;
                }
            }
            oldPos = -movementVector + startingPoint;
            linePoints.Add(-movementVector + startingPoint);
        }
        linePoints.RemoveAt(0);
        lineRenderer.positionCount = linePoints.Count;
        lineRenderer.SetPositions(linePoints.ToArray());
        aimHitObject.gameObject.SetActive(hitted);

        lineRenderer.materials = new Material[]{ lineRenderer.materials[0]};
    }


    ///<summary>
    /// CLears the aim line setting all the positions to zero
    ///</summary>
    public void ClearAimLine() {
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
        aimHitObject.gameObject.SetActive(false);
    }

    public void Break() {
        AudioHandler.Instance?.Play3DSound(SFXFiles.potion_break, burstParticle.transform);

        burstParticle.transform.parent = smokeParticle.transform.parent = transform.parent;

        burstParticle.Emit(50);
        smokeParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        OnBreak?.Invoke(this);
        Destroy(aimHitObject.gameObject);
        Destroy(burstParticle, 5f);
        Destroy(smokeParticle, 5f);
        Destroy(gameObject, 5f);
        StartCoroutine(Extensions.AnimateCallBack(1f, 0f, AnimationCurve.Linear(0,0,1,1), (val) => {
            AimLineAlpha = val;
        }, 5f));
        Destroy(potionMeshObject);
    }
    public float AimLineAlpha {
        get { return lineRenderer.material.GetFloat("_alpha");}
        set { lineRenderer.material.SetFloat("_alpha", value); }
    }

}
