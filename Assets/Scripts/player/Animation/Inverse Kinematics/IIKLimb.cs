using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Intervace for a IK human limb.
///</summary>
public interface IIKHumanLimb
{
    public bool HasContact { get; set;}
    public bool IsActive { get; set;}
    public float Weight {get; set; }

    public Vector3 GetRayCastPosition();
    public void UpdateIK();
    public IEnumerator AnimateWeight(float end);
    public void Setup(Transform _transform, Animator _animator, Rigidbody _rigidBody, bool rightHand = true);
}


public abstract class IKLimb : MonoBehaviour, IIKHumanLimb
{
    protected RaycastHit currentHit;

    protected bool hasContact = false;
    protected Coroutine weightCoroutine;

    protected Transform animatorTransform;
    protected Animator animator;
    protected Rigidbody rigidBody;
    protected AvatarIKGoal IKGoal;

    protected float contactDuration = .5f;
    protected float hitMovingDuration = .3f;


    public bool HasContact {
        get { return hasContact;}
        set { 
            if (hasContact == value) return;

            hasContact = value; 

            if (weightCoroutine != null) StopCoroutine(weightCoroutine);
            weightCoroutine = StartCoroutine(AnimateWeight(value ? 1 : 0));
        }
    }


    protected bool isActive = true;
    public bool IsActive {
        get => isActive;
        set {
            isActive = value;
            if (value == false) HasContact = false;
        }
    }
    private float weight = 0f;
    public float Weight {
        get { return weight;}
        set { 
            weight = value;
            animator.SetIKPositionWeight(IKGoal, value);
            animator.SetIKRotationWeight(IKGoal,Mathf.Sqrt(value));
        }
    }


    public IEnumerator AnimateWeight(float end) {
        float index = 0;
        float start = Weight;
        AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
        while (index < contactDuration) {
            index += Time.deltaTime;
            Weight = Mathf.Lerp(start,end, curve.Evaluate(index / contactDuration));
            yield return new WaitForEndOfFrame();
        }
        Weight = end;
    }

    public virtual Vector3 GetRayCastPosition()
    {
        return Vector3.zero;
    }

    public virtual void Setup(Transform _transform, Animator _animator, Rigidbody _rigidBody, bool right = true)
    {
        animatorTransform = _transform;
        animator = _animator;
        rigidBody = _rigidBody;

    }
    protected IEnumerator AnimateHitPosition(RaycastHit end) {
        float index = 0;
        RaycastHit start = currentHit;
        RaycastHit mid = currentHit.LerpUnclamped(end, .5f);
        mid.point += mid.normal * .4f;
        AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
        while (index < hitMovingDuration) {
            index += Time.deltaTime;
            currentHit = currentHit.LerpWithBezier(end, mid, curve.Evaluate(index / hitMovingDuration));
            yield return new WaitForEndOfFrame();
        }
        currentHit = end;
    }


    public virtual void UpdateIK()
    {

    }
}