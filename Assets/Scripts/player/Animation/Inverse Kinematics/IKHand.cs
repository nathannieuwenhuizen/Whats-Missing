using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHand: MonoBehaviour, IIKLimb
{
    private RaycastHit currentHandHit;
    private RaycastHit destinationHandHit;
    private bool hasContact = false;

    private Vector3 oldPos;
    
    public bool HasContact {
        get { return hasContact;}
        set { 
            if (hasContact == value) return;

            hasContact = value; 

            if (weightCoroutine != null) StopCoroutine(weightCoroutine);
            weightCoroutine = StartCoroutine(AnimateWeight(value ? 1 : 0));
        }
    }

    private bool isActive = true;
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

    public static Vector3 SHOULDER_OFFSET = new Vector3(0.5f, 4f, 0f);
    public static float HAND_RANGE = 1.5f;

    private Coroutine weightCoroutine;
    private Coroutine handHitCoroutine;

    private float contactDuration = .5f;
    private float handMovingDuration = .3f;

    private float maxDistanceBetweenhits = .5f;
    private float minDistanceBetweenhits = .5f;
    private float distanceBetweenhits = .3f;

    private Transform animatorTransform;
    private Animator animator;
    private Rigidbody rigidBody;

    private AvatarIKGoal IKGoal = AvatarIKGoal.RightHand;
    public void Setup(Transform _transform, Animator _animator, Rigidbody _rigidBody, bool rightHand = true) {
        animatorTransform = _transform;
        animator = _animator;rigidBody = _rigidBody;
        if (rightHand == false) IKGoal = AvatarIKGoal.LeftHand;
        distanceBetweenhits = Random.Range(minDistanceBetweenhits, maxDistanceBetweenhits);
        
    }


    public Vector3 GetRayCastPosition() {
        Vector3 temp = SHOULDER_OFFSET;
        if (IKGoal == AvatarIKGoal.LeftHand) temp.x *= -1;
        return animatorTransform.position + animatorTransform.TransformDirection(temp) + animatorTransform.forward;
    }

    public void UpdateIK() {
        if (animator == null) return;

        RaycastWall();
        Weight = Weight;

        if (Weight != 0) {

            Vector3 delta = transform.InverseTransformPoint( currentHandHit.point + currentHandHit.normal * .2f);
            if (rigidBody.velocity.magnitude > 0.1f) 
            {
                if (IKGoal == AvatarIKGoal.RightHand) delta.x = Mathf.Max(0f, delta.x);
                else delta.x = Mathf.Min(0f, delta.x);
            } 
            // Debug.Log("delta: " + delta);
            animator.SetIKPosition(IKGoal,transform.TransformPoint(delta));
            animator.SetIKRotation(IKGoal,Quaternion.LookRotation(-currentHandHit.normal + new Vector3(0,90,0), animatorTransform.up));
        }
    }


    private void RaycastWall() {

        Ray ray = new Ray(GetRayCastPosition(), animatorTransform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, HAND_RANGE) && rigidBody.velocity.magnitude < 8.5f)
        {    
            if (HasContact == false) currentHandHit = hit;

            float handAngle = Vector3.Angle(transform.forward, -hit.normal);
            if (Vector3.Distance(hit.point, destinationHandHit.point) > distanceBetweenhits && handAngle < 50) {
                if (HasContact == false) {
                    destinationHandHit = currentHandHit = hit;
                } else {
                    destinationHandHit = hit;
                    distanceBetweenhits = Random.Range(minDistanceBetweenhits, maxDistanceBetweenhits);
                    if (handHitCoroutine != null) StopCoroutine(handHitCoroutine);
                    handHitCoroutine = StartCoroutine(AnimateHitPosition(destinationHandHit));
                }
            }
            HasContact = true;
        } else {
            HasContact = false;
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
    private IEnumerator AnimateHitPosition(RaycastHit end) {
        float index = 0;
        RaycastHit start = currentHandHit;
        RaycastHit mid = currentHandHit.LerpUnclamped(end, .5f);
        mid.point += mid.normal * .4f;
        AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
        while (index < handMovingDuration) {
            index += Time.deltaTime;
            currentHandHit = currentHandHit.LerpWithBezier(end, mid, curve.Evaluate(index / handMovingDuration));
            yield return new WaitForEndOfFrame();
        }
        currentHandHit = end;
    }
}
