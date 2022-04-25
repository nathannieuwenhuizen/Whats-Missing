using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHand: IKLimb
{
    private RaycastHit destinationHandHit;
    
    public static Vector3 SHOULDER_OFFSET = new Vector3(0.5f, 4f, 0f);
    public static float HAND_RANGE = 1.5f;

    private Coroutine handHitCoroutine;

    private float scale = 1f;
    private bool grabbing = false;

    private float maxDistanceBetweenhits = .5f;
    private float minDistanceBetweenhits = .5f;
    private float distanceBetweenhits = .3f;

    public override void Setup(Transform _transform, Animator _animator, Rigidbody _rigidBody, bool rightHand = true) {
        base.Setup(_transform, _animator, _rigidBody, rightHand);

        contactDuration = .5f;
        if (rightHand == false) IKGoal = AvatarIKGoal.LeftHand;
        else IKGoal = AvatarIKGoal.RightHand;
        distanceBetweenhits = Random.Range(minDistanceBetweenhits, maxDistanceBetweenhits);
        
    }

    public override Vector3 GetRayCastPosition() {
        Vector3 temp = SHOULDER_OFFSET;
        if (IKGoal == AvatarIKGoal.LeftHand) temp.x *= -1;
        return animatorTransform.position + (animatorTransform.TransformDirection(temp) + animatorTransform.forward) * scale;
    }

    public override void UpdateIK() {
        if (animator == null) return;

        if (!grabbing) RaycastWall();

        Weight = Weight;

        if (Weight != 0) {

            animator.SetIKPositionWeight(IKGoal, Weight);
            animator.SetIKRotationWeight(IKGoal,Mathf.Sqrt(Weight));
            
            Vector3 delta = transform.InverseTransformPoint( currentHit.point + currentHit.normal * .2f);
            if (rigidBody.velocity.magnitude > 0.1f) 
            {
                if (IKGoal == AvatarIKGoal.RightHand) delta.x = Mathf.Max(0f, delta.x);
                else delta.x = Mathf.Min(0f, delta.x);
            } 

            animator.SetIKPosition(IKGoal,transform.TransformPoint(delta));
            animator.SetIKRotation(IKGoal,Quaternion.LookRotation(-currentHit.normal + new Vector3(0,90,0), animatorTransform.up));
        }
    }


    private void RaycastWall() {

        //left hand doesnt touch walls
        if (IKGoal == AvatarIKGoal.LeftHand) {
            HasContact = false;
            return;
        }

        Ray ray = new Ray(GetRayCastPosition(), animatorTransform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, HAND_RANGE) && rigidBody.velocity.magnitude < 8.5f)
        {    
            if (hit.transform.tag == Tags.Picked || hit.collider.isTrigger) {
                HasContact = false;
                return;
            }
            if (HasContact == false) currentHit = hit;

            float handAngle = Vector3.Angle(transform.forward, -hit.normal);
            if (Vector3.Distance(hit.point, destinationHandHit.point) > distanceBetweenhits && handAngle < 50) {
                if (HasContact == false) {
                    destinationHandHit = currentHit = hit;
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

    private void OnEnable() {
        Player.OnPlayerShrink += OnPlayerShrink;
        Player.OnPlayerUnShrink += OnPlayerNormal;
        Door.OnPassingThrough += GrabDoorKnob;
    }
    private void OnDisable() {
        Player.OnPlayerShrink -= OnPlayerShrink;
        Player.OnPlayerUnShrink -= OnPlayerNormal;
        Door.OnPassingThrough -= GrabDoorKnob;
    }

    private void OnPlayerShrink() {
        scale = 0.1f;
    }
    private void OnPlayerNormal() {
        scale = 1;
    }

    public void GrabDoorKnob(Door door) {

        if (door.PlayerIsAtStartSide() && IKGoal == AvatarIKGoal.RightHand ||
        !door.PlayerIsAtStartSide() && IKGoal == AvatarIKGoal.LeftHand) return;
        StartCoroutine(GrabbingDoorKnob(door));
    }
    private IEnumerator GrabbingDoorKnob(Door door) {
        grabbing = true;
        HasContact = true;
        RaycastHit doorKnobPos = currentHit;
        bool atStart = door.PlayerIsAtStartSide();
        Transform knob = door.GetKnob(atStart);
        doorKnobPos.point = knob.position;
        doorKnobPos.normal = (Camera.main.transform.position - knob.position).normalized;
        yield return StartCoroutine(AnimateHitPosition(doorKnobPos));
        while(Door.IN_WALKING_ANIMATION) {
            Weight = 1;
            knob = door.GetKnob(atStart);
            currentHit.point = knob.position;
            currentHit.normal = (Camera.main.transform.position - knob.position).normalized;
            // doorKnobPos.normal = door.Knob.forward;
            yield return new WaitForEndOfFrame();
        }
        grabbing = false;
    }

    private void OnDrawGizmos() {
        Gizmos.color = HasContact ? Color.green : Color.red;
        Gizmos.DrawSphere(GetRayCastPosition(), .1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(GetRayCastPosition(), GetRayCastPosition() + transform.forward * IKHand.HAND_RANGE);
    }

}
