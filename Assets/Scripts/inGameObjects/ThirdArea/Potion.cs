using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
/// A potion that when hitting a changable object, introduces a change ot that object it crashes into.
///</summary>
public class Potion : PickableRoomObject
{
    public delegate void  PotionCrashEvent(Potion potion, IChangable changable);
    public static PotionCrashEvent OnCrash;
    
    [SerializeField]
    private ChangeType changeType;

    public ChangeType ChangeType {
        get { return changeType;}
    }
    private float forcePower = 300f;

    private bool thrown = false;

    private void Reset() {
        Word = "potion";
        AlternativeWords = new string[] { "potions", "brew" };
    }

    public override void Release()
    {
        base.Release();
        Vector3 delta =  Camera.main.transform.forward;
        rb.AddForce(delta * forcePower);
        thrown = true;
    }


    private void OnCollisionEnter(Collision other) {
        if (!thrown) return;
        IChangable changable = other.transform.GetComponentInChildren<IChangable>();
        if (changable != null) {
            OnCrash?.Invoke(this, changable);
        }

        Destroy(gameObject);
    }
}
