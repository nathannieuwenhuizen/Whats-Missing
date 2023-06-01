using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : Property
{

    [SerializeField]
    private Room room;

    public static OnPropertyToggle onGravityMissing;

    [SerializeField]
    private bool movePlayerUpwardOnMissing = false;

    private List<Rigidbody> allRigidbodies;
    private float  gravityScale = 1f;


    public override IEnumerator AnimateMissing()
    {
        yield return AnimatGravityToggle((rb) => {
            rb.useGravity = false;
            rb.AddForce(Extensions.RandomVector(5f));
            rb.angularVelocity = Extensions.RandomVector(5f);
        });
        yield return base.AnimateMissing();
    }

    public Gravity() {
        animationDuration = 1f;
    }
    public override void OnMissingFinish()
    {
        onGravityMissing?.Invoke();
        base.OnMissingFinish();
        foreach(Rigidbody rb in room.GetAllObjectsInRoom<Rigidbody>()) {
            rb.AddForce(Extensions.RandomVector(1f));
            rb.angularVelocity = Extensions.RandomVector(2f);
            rb.useGravity = false;
        }
        if (room.Animated && movePlayerUpwardOnMissing) room.Player.Movement.RB.velocity = Vector3.up * 8f;
    }


    private IEnumerator AnimatGravityToggle(Action<Rigidbody> callback) {
        allRigidbodies = SortedByDistanceRigidBodies();
        int steps = allRigidbodies.Count > 20 ? Mathf.CeilToInt(allRigidbodies.Count / 20) : 1;
        for (int i = 0; i < allRigidbodies.Count; i += steps)
        {
            for (int j = 0; j < steps; j++)
            {
                if (i + j < allRigidbodies.Count) {
                    Rigidbody rb = allRigidbodies[i + j];
                    callback(rb);
                }
            }
            yield return new WaitForSeconds(animationDuration / ((float)allRigidbodies.Count / steps));
        }
    }

    public override IEnumerator AnimateAppearing()
    {
        yield return AnimatGravityToggle((rb) => {
            rb.useGravity = true;
        });
        yield return base.AnimateAppearing();
    }

    public override void OnAppearingFinish()
    {
        base.OnAppearingFinish();
        foreach(Rigidbody rb in room.GetAllObjectsInRoom<Rigidbody>()) {
            rb.useGravity = true;
        }
    }

    private void FixedUpdate() {
        if (IsShrinked || IsEnlarged) {
            Vector3 gravity = -9.81f * gravityScale * Vector3.up;
            foreach(Rigidbody rb in allRigidbodies) {
                rb.AddForce(gravity, ForceMode.Acceleration);
            }
        }
    }
    public override void OnShrinkingFinish()
    {
        base.OnShrinkingFinish();
        allRigidbodies = room.GetAllObjectsInRoom<Rigidbody>();
        foreach(Rigidbody rb in allRigidbodies) rb.useGravity = false;
        gravityScale = .5f;
    }
    public override void OnShrinkingRevertFinish()
    {
        base.OnShrinkingRevertFinish();
        foreach(Rigidbody rb in allRigidbodies) rb.useGravity = true;
    }

    public override void OnEnlargingFinish()
    {
        base.OnEnlargingFinish();
        allRigidbodies = room.GetAllObjectsInRoom<Rigidbody>();
        foreach(Rigidbody rb in allRigidbodies) rb.useGravity = false;
        gravityScale = 3f;

    }
    public override void OnEnlargeRevertFinish()
    {
        base.OnEnlargeRevertFinish();
        foreach(Rigidbody rb in allRigidbodies) rb.useGravity = true;
    }

    public List<Rigidbody> SortedByDistanceRigidBodies() {
        List<Rigidbody> allRigidbodies = room.GetAllObjectsInRoom<Rigidbody>();
        Debug.Log("current change: " + currentChange);
        if (currentChange != null &&  currentChange.mirror != null)
            allRigidbodies.Sort(delegate(Rigidbody a, Rigidbody b) {
                return Vector3.Distance(a.transform.position, currentChange.mirror.transform.position) >
                    Vector3.Distance(b.transform.position, currentChange.mirror.transform.position) ? 1 : -1;
            });
        return allRigidbodies;
    }

    public override void OnFlipped()
    {
        base.OnFlipped();
        Physics.gravity *= -1;
        FPMovement.GLOBAL_GRAVITY *= -1;
        Vector3 flipped = room.Player.transform.eulerAngles;
        flipped.z += 180f;
        StartCoroutine(room.Player.transform.AnimatingRotation(Quaternion.Euler(flipped), AnimationCurve.EaseInOut(0,0,1,1), animationDuration ));
    }
    public override void OnFlippingRevert()
    {
        Physics.gravity *= -1;
        FPMovement.GLOBAL_GRAVITY *= -1;
        Vector3 flipped = room.Player.transform.eulerAngles;
        flipped.z += 180f;
        StartCoroutine(room.Player.transform.AnimatingRotation(Quaternion.Euler(flipped), AnimationCurve.EaseInOut(0,0,1,1), animationDuration));

        base.OnFlippingRevert();
    }



    private void Reset() {
        Word = "gravity";
    }

}
