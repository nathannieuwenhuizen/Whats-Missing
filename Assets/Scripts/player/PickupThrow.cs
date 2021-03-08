using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupThrow : MonoBehaviour
{

    private IPickable holdingObject;
    [SerializeField]
    private float pickupDIstance = 2;

    public void pickup() {

        IPickable pickedObject = focussedObject();
        if (pickedObject != null) {
            pickedObject.rigidBody.isKinematic = true;
            pickedObject.rigidBody.transform.parent = transform;
            holdingObject = pickedObject;
        }
    }


    public void release() {
        if (holdingObject == null) return;

        holdingObject.rigidBody.isKinematic = false;
        holdingObject.rigidBody.transform.parent = null; // maybe not null but previous parent?
        holdingObject = null;
    }

    private IPickable focussedObject() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupDIstance)) {
            if (hit.rigidbody.gameObject.GetComponent<IPickable>() != null) return hit.rigidbody.gameObject.GetComponent<IPickable>();
        }
        return null;
    }

}
