using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskObject {

    public Transform transform;
    public Vector3 oldPos;
    public Quaternion oldRotation;
    public void SaveOldPos() {
        oldPos = transform.position;
        oldRotation = transform.transform.rotation;
    }
}

///<summary>
/// A parent which has smaller objects on top of it so that when it reappears again, the child objects go into their original position.
///</summary>
public class ParentRoomObject : RoomObject
{
    [SerializeField]
    private Transform[] objectsOnTable;

    private List<DeskObject> deskObjects = new List<DeskObject>();

    protected override void Awake() {
        base.Awake();
        deskObjects = new List<DeskObject>();
        for(int i = 0 ; i < objectsOnTable.Length; i++) {
            DeskObject newDeskObject = new DeskObject() {transform = objectsOnTable[i] };
            deskObjects.Add(newDeskObject);
        }
    }

    public override void OnMissing()
    {
        SaveObjectPositions();
        base.OnMissing();
    }

    public override IEnumerator AnimateAppearing()
    {
        AnimateObjectsBackIntoPos();
        return base.AnimateAppearing();
    }


    private void SaveObjectPositions() {
        foreach(DeskObject deskObj in deskObjects) {
            deskObj.SaveOldPos();
        } 
    }
    private void AnimateObjectsBackIntoPos() {
        foreach(DeskObject deskObj in deskObjects) {
            StartCoroutine(AnimateBackToOldPos(deskObj, Random.Range(0, animationDuration * .25f), animationDuration * .75f));
        } 
    }

    public IEnumerator AnimateBackToOldPos(DeskObject deskObj, float delay, float duration) {
        Rigidbody rb = deskObj.transform.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
        }
        yield return new WaitForSeconds(delay);
        Vector3 begin = transform.position;
        Vector3 end = deskObj.oldPos;
        Vector3 mid = begin + (end - begin) * .5f;
        mid.y += 5f;
        StartCoroutine(deskObj.transform.AnimatingPosBezierCurve(end, mid, AnimationCurve.EaseInOut(0,0,1,1), duration));
        yield return StartCoroutine(deskObj.transform.AnimatingRotation(deskObj.oldRotation, AnimationCurve.EaseInOut(0,0,1,1), duration));
        if (rb != null) {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

}
