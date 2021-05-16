using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    private IPickable holdingObject;
    private Transform holidngObjectParent;
    private Vector3 oldPos = new Vector3();
    private Vector3 velocity = new Vector3();

    [SerializeField]
    private float pickupDistance = 3;

    private float maxThrowForce = .7f;
    private float throwForceMultiplier = 10f;

    public void Grab() {
        //check stuff to pickup
        IPickable pickedObject = focussedObject<IPickable>();
        Debug.Log("grab" + (pickedObject != default(IPickable)));
        if (pickedObject != default(IPickable)) {
            pickedObject.RigidBody.isKinematic = true;
            holidngObjectParent = pickedObject.RigidBody.transform.parent;
            pickedObject.RigidBody.transform.parent = transform;
            holdingObject = pickedObject;
            oldPos = holdingObject.RigidBody.transform.position;
            StartCoroutine(UpdateVelocity());
            return;
        }

        //check objects to interact with.
        IInteractable interactableObj = focussedObject<IInteractable>();
        if (interactableObj != default(IInteractable)) {
            interactableObj.Interact();
        }
    }

    //Releases the holding object
    public void Release() {
        if (holdingObject == null) return;

        holdingObject.RigidBody.isKinematic = false;
        holdingObject.RigidBody.transform.parent = holidngObjectParent;
        if (velocity.magnitude > maxThrowForce) {
            velocity = velocity.normalized * maxThrowForce;
        }
        holdingObject.RigidBody.velocity =  velocity * throwForceMultiplier;
        holdingObject = null;
    }

    //updates the holding object velocity;
    private IEnumerator UpdateVelocity() {
        while (holdingObject != null) {
            velocity = holdingObject.RigidBody.transform.position - oldPos;
            oldPos = holdingObject.RigidBody.transform.position;
            yield return new WaitForSeconds(.1f);
        }
    }

    private T focussedObject<T>() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupDistance)) {
            if (hit.collider.gameObject.GetComponent<T>() != null) return hit.collider.gameObject.GetComponent<T>();
        }
        return default(T);
    }

}
