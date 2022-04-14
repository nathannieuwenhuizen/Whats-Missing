using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKLeg : IKLimb
{

    private float GroundOffset = .35f;
    private Transform feetTransform;


    public override Vector3 GetRayCastPosition()
    {
         return  animator.GetIKPosition(IKGoal) - currentHit.normal * GroundOffset;
    }

    public Vector3 GetFeetPos() {
        return feetTransform.position;
    }

    public void Setup(Transform _transform, Animator _animator, Rigidbody _rigidBody, Transform _feetTransform, bool rightFoot = true) {
        base.Setup(_transform, _animator, _rigidBody);
        contactDuration = .3f;
        feetTransform = _feetTransform;

        if (rightFoot == false) IKGoal = AvatarIKGoal.LeftFoot;
        else  IKGoal = AvatarIKGoal.RightFoot;
        
    }

    private void RayCastToGround() {
        Ray ray = new Ray(GetRayCastPosition() + animatorTransform.up * GroundOffset, -animatorTransform.up * FPMovement.FOOT_RANGE * 1);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, FPMovement.FOOT_RANGE) && rigidBody.velocity.magnitude < 8.5f && rigidBody.velocity.magnitude < .5f)
        {    
            currentHit = hit;
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

    public override void UpdateIK() {
        if (animator == null) return;


        RayCastToGround();
        if (Weight != 0) {
            Weight = Weight;
            animator.SetIKPositionWeight(IKGoal, Weight);
            animator.SetIKRotationWeight(IKGoal,Mathf.Sqrt(Weight));

            animator.SetIKPosition(IKGoal,currentHit.point + currentHit.normal * GroundOffset);
            animator.SetIKRotation(IKGoal,Quaternion.LookRotation(transform.forward, currentHit.normal));
            animator.SetIKPosition("", Vector3.zero);
        }
    }

    private void OnDrawGizmos() {
        // Gizmos.color = HasContact ? Color.green : Color.red;
        // Gizmos.DrawSphere(GetRayCastPosition(), .1f);

        Gizmos.color = HasContact ? Color.green : Color.red;
        Gizmos.DrawLine(GetFeetPos(), GetFeetPos() - transform.up * FPMovement.FOOT_RANGE * 1f);
    }

}
