using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupThrow : MonoBehaviour
{

    private IPickable holdingObject;
    [SerializeField]
    private float pickupDIstance = 2;

    public void pickup() {
        //check stuff to pickup
        IPickable pickedObject = focussedObject<IPickable>();
        if (pickedObject != default(IPickable)) {
            pickedObject.RigidBody.isKinematic = true;
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
        holdingObject.RigidBody.transform.parent = null; // maybe not null but previous parent?
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
