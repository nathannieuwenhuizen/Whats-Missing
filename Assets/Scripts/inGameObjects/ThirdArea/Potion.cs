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
    private ChangeType changeType;

    public ChangeType ChangeType {
        get { return changeType;}
    }
    private float forcePower = 500f;

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


    public override void Release()
    {
        base.Release();
        AudioHandler.Instance?.PlaySound(SFXFiles.potion_throw);
        Vector3 delta =  Camera.main.transform.forward;
        rb.AddForce(delta * forcePower * rb.mass);
        thrown = true;
    }


    private void OnCollisionEnter(Collision other) {
        if (!thrown || broken) return;
        broken = true;
        IChangable changable = getPotentialIChangable(other);
        if (changable != null) {
            OnChanging?.Invoke(this, changable);
        }
        Break();
    }

    private IChangable getPotentialIChangable(Collision other)
    {        
        IChangable changable = other.transform.GetComponentInChildren<IChangable>();
        if (changable == null) 
            changable = other.transform.GetComponent<IChangable>();
        if (changable == null) 
            changable = other.transform.parent.GetComponent<IChangable>();
        return changable;
    }

    public void Break() {
        AudioHandler.Instance?.Play3DSound(SFXFiles.potion_break, burstParticle.transform);

        burstParticle.transform.parent = smokeParticle.transform.parent = transform.parent;

        burstParticle.Emit(50);
        smokeParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        OnBreak?.Invoke(this);
        Destroy(burstParticle, 5f);
        Destroy(smokeParticle, 5f);
        Destroy(gameObject);

    }
}
