using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupThrow : MonoBehaviour
{

    private Rigidbody holdingObject;
    private float pickupDIstance = 2;

    public void pickup() {

        Rigidbody pickedObject = focussedObject();
        if (pickedObject) {
            pickedObject.isKinematic=  true;
            pickedObject.transform.parent = transform;
            holdingObject = pickedObject;
        }
    }


    public void release() {
        if (!holdingObject) return;

        holdingObject.isKinematic = false;
        holdingObject.transform.parent = null; // maybe not null but previous parent?
        holdingObject = null;
    }

    private Rigidbody focussedObject() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupDIstance)) {
            if (hit.rigidbody != null) return hit.rigidbody;
        }
        return null;
    }

}
