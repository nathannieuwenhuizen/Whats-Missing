using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupThrow : MonoBehaviour
{

    private IPickable holdingObject;
    private Transform holidngObjectParent;

    [SerializeField]
    private float pickupDIstance = 3;

    public void pickup() {
        //check stuff to pickup
        IPickable pickedObject = focussedObject<IPickable>();
        if (pickedObject != default(IPickable)) {
            pickedObject.RigidBody.isKinematic = true;
            holidngObjectParent = pickedObject.RigidBody.transform.parent;
            pickedObject.RigidBody.transform.parent = transform;
            holdingObject = pickedObject;
            return;
        }

        //check objects to interact with.
        IInteractable interactableObj = focussedObject<IInteractable>();
        if (interactableObj != default(IInteractable)) {
            interactableObj.Interact();
        }
    }


    public void release() {
        if (holdingObject == null) return;

        holdingObject.RigidBody.isKinematic = false;
        holdingObject.RigidBody.transform.parent = holidngObjectParent;
        holdingObject = null;
    }

    private T focussedObject<T>() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupDIstance)) {
            if (hit.collider.gameObject.GetComponent<T>() != null) return hit.collider.gameObject.GetComponent<T>();
        }
        return default(T);
    }

}
