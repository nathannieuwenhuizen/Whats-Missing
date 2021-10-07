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

    public override void onMissingFinish()
    {
        onGravityMissing?.Invoke();
        base.onMissingFinish();
        foreach(Rigidbody rb in room.GetAllObjectsInRoom<Rigidbody>()) {
            rb.useGravity = false;
        }
    }

    public override IEnumerator AnimateMissing()
    {
        float totalTime = .5f;
        List<Rigidbody> allRigidbodies = SortedByDistanceRigidBodies();
        foreach(Rigidbody rb in allRigidbodies) {
            rb.useGravity = false;
            rb.AddForce(Extensions.RandomVector(5f));
            rb.angularVelocity = Extensions.RandomVector(5f);
            yield return new WaitForSeconds(totalTime / (float)allRigidbodies.Count);
        }
        yield return base.AnimateMissing();
    }

    public override IEnumerator AnimateAppearing()
    {
        float totalTime = .3f;
        List<Rigidbody> allRigidbodies = SortedByDistanceRigidBodies();
        foreach(Rigidbody rb in allRigidbodies) {
            rb.useGravity = true;
            yield return new WaitForSeconds(totalTime / (float)allRigidbodies.Count);
        }
        yield return base.AnimateAppearing();
    }

    public List<Rigidbody> SortedByDistanceRigidBodies() {
        List<Rigidbody> allRigidbodies = room.GetAllObjectsInRoom<Rigidbody>();
        allRigidbodies.Sort(delegate(Rigidbody a, Rigidbody b) {
            return Vector3.Distance(a.transform.position, currentChange.television.transform.position) >
                   Vector3.Distance(b.transform.position, currentChange.television.transform.position) ? 1 : -1;
        });
        return allRigidbodies;
    }


    public override void onAppearingFinish()
    {
        base.onAppearingFinish();
        foreach(Rigidbody rb in room.GetAllObjectsInRoom<Rigidbody>()) {
            rb.useGravity = true;
        }
    }

    private void Reset() {
        Word = "gravity";
    }

}
