using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : Property
{

    [SerializeField]
    private Room room;
    [SerializeField]
    private Material shockWaveMaterial;

    public static OnPropertyToggle onGravityMissing;

    public override IEnumerator AnimateMissing()
    {
        yield return AnimatGravityToggle((rb) => {
            rb.useGravity = false;
            rb.AddForce(Extensions.RandomVector(5f));
            rb.angularVelocity = Extensions.RandomVector(5f);
        });
        yield return base.AnimateMissing();
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
    }


    private IEnumerator AnimatGravityToggle(Action<Rigidbody> callback) {
        float totalTime = 1f;
        List<Rigidbody> allRigidbodies = SortedByDistanceRigidBodies();
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
            yield return new WaitForSeconds(totalTime / ((float)allRigidbodies.Count / steps));
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
    
    public List<Rigidbody> SortedByDistanceRigidBodies() {
        List<Rigidbody> allRigidbodies = room.GetAllObjectsInRoom<Rigidbody>();
        if (currentChange.mirror != null)
            allRigidbodies.Sort(delegate(Rigidbody a, Rigidbody b) {
                return Vector3.Distance(a.transform.position, currentChange.mirror.transform.position) >
                    Vector3.Distance(b.transform.position, currentChange.mirror.transform.position) ? 1 : -1;
            });
        return allRigidbodies;
    }



    private void Reset() {
        Word = "gravity";
    }

}
