using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKLeg : MonoBehaviour, IIKLimb
{
    private RaycastHit currentFootHit;

    private float GroundOffset = .35f;
    private float contactDuration = .3f;

    private AvatarIKGoal IKGoal = AvatarIKGoal.RightFoot;

    private Transform animatorTransform;
    private Animator animator;
    private Rigidbody rigidBody;
    private Transform feetTransform;

    [SerializeField]
    private bool hasContact = false;
    private Coroutine weightCoroutine;
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
            animator.SetIKRotationWeight(IKGoal,value);
        }
    }


    public Vector3 GetRayCastPosition()
    {
         return  animator.GetIKPosition(IKGoal) - currentFootHit.normal * GroundOffset;
    }

    public Vector3 GetFeetPos() {
        return feetTransform.position;
    }

    public void Setup(Transform _transform, Animator _animator, Rigidbody _rigidBody, bool rightFoot = true) {
    }
    public void Setup(Transform _transform, Animator _animator, Rigidbody _rigidBody, Transform _feetTransform, bool rightFoot = true) {
        animatorTransform = _transform;
        animator = _animator;
        rigidBody = _rigidBody;
        feetTransform = _feetTransform;

        if (rightFoot == false) IKGoal = AvatarIKGoal.LeftFoot;
        
    }

    public IEnumerator AnimateWeight(float end)
    {
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

    private void RayCastToGround() {
        Ray ray = new Ray(GetRayCastPosition() + animatorTransform.up * GroundOffset, -animatorTransform.up * FPMovement.FOOT_RANGE * 1);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, FPMovement.FOOT_RANGE) && rigidBody.velocity.magnitude < 8.5f && rigidBody.velocity.magnitude < .5f)
        {    
            currentFootHit = hit;
            HasContact = true;
        } else {
            HasContact = false;
        }
    }

    private void OnEnable() {
        Player.OnPlayerShrink += OnPlayerShrink;
        Player.OnPlayerUnShrink += OnPlayerNormal;
    }
    private void OnDisable() {
        Player.OnPlayerShrink -= OnPlayerShrink;
        Player.OnPlayerUnShrink -= OnPlayerNormal;
    }

    private void OnPlayerShrink() {
        GroundOffset = .1f;
    }
    private void OnPlayerNormal() {
        GroundOffset = .35f;
    }

    public void UpdateIK() {
        if (animator == null) return;


        // animator.SetIKPosition(IKGoal,currentFootHit.point + currentFootHit.normal * GroundOffset);
        RayCastToGround();
        // Weight = 1;

        // if (HasContact) Weight = animator.GetFloat(IKGoal == AvatarIKGoal.LeftFoot ? "IKLeftFootWeight" : "IKRightFootWeight");
        if (Weight != 0) {
            Weight = Weight;
            animator.SetIKPosition(IKGoal,currentFootHit.point + currentFootHit.normal * GroundOffset);
            animator.SetIKRotation(IKGoal,Quaternion.LookRotation(transform.forward, currentFootHit.normal));
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = HasContact ? Color.green : Color.red;
        Gizmos.DrawSphere(GetRayCastPosition(), .1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(GetFeetPos(), GetFeetPos() - transform.up * FPMovement.FOOT_RANGE * 1f);
    }

}
