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
    [SerializeField]
    private IInteractable currentInteractable;

    private float maxThrowForce = .7f;
    private float throwForceMultiplier = 10f;

    public void Grab() {
        //check stuff to pickup
        IPickable pickedObject = focussedObject<IPickable>();
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

    private void OnEnable() {
        InputManager.OnClickDown += Grab;
        InputManager.OnClickUp += Release;
    }

    private void OnDisable() {
        InputManager.OnClickDown -= Grab;
        InputManager.OnClickUp -= Release;
    }

    //Releases the holding object
    public void Release() {
        if (holdingObject == null) return;

        if (Time.timeScale == 0) {
            StartCoroutine(UpdatePhysics());
        }
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
            Debug.Log("wait time");
            yield return new WaitForSecondsRealtime(.1f);
        }
    }

    //TODO: needs a way better fix than this!
    private IEnumerator UpdatePhysics() {
        Time.timeScale = 1f;
        holdingObject.RigidBody.isKinematic = true;
        Physics.Simulate(1f);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        // holdingObject.RigidBody.isKinematic = false;

        Time.timeScale = 0;
        
    }

    private void Update() {
        UpdateFocusedObject();
    }
    private void UpdateFocusedObject() {
        IInteractable interactableObj = focussedObject<IInteractable>();
        if (interactableObj != default(IInteractable)) {
            if (interactableObj != currentInteractable) {
                if (currentInteractable != default(IInteractable))
                    currentInteractable.Focused = false;
                interactableObj.Focused = true;
                currentInteractable = interactableObj;
            }
        } else if (currentInteractable != null) {
            currentInteractable.Focused = false;
            currentInteractable = default(IInteractable);
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
