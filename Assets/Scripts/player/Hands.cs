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

    private Rigidbody rigidbody;
    private Vector3 localPos;

    private float maxThrowForce = .7f;
    private float throwForceMultiplier = 10f;
    
    private bool isEnabled = true;

    public void OnClick() {
        if (!isEnabled) return;
        
        //check objects to interact with.
        IInteractable interactableObj = FocussedObject<IInteractable>();
        Debug.Log(interactableObj != default(IInteractable));
        if (interactableObj != default(IInteractable)) {
            if (interactableObj.Gameobject.GetComponent<IPickable>() != default(IPickable)) {
                Grab(interactableObj.Gameobject.GetComponent<IPickable>());
            } else {
                interactableObj.Interact();
            }
        }
    }
    private void Grab(IPickable obj) {
        // obj.RigidBody.isKinematic = true;
        // holidngObjectParent = obj.gameObject.transform.parent;
        // obj.gameObject.transform.parent = transform;
        holdingObject = obj;
        oldPos = holdingObject.gameObject.transform.position;
        obj.Grab(rigidbody);
        StartCoroutine(UpdateVelocity());
        StartCoroutine(UpdatePhysics()); 
    }

    private void OnEnable() {
        InputManager.OnClickDown += OnClick;
        InputManager.OnClickUp += Release;
        PauseScreen.OnPause += DisableClick;
        PauseScreen.OnResume += EnableClick;
    }

    private void OnDisable() {
        InputManager.OnClickDown -= OnClick;
        InputManager.OnClickUp -= Release;
        PauseScreen.OnPause -= DisableClick;
        PauseScreen.OnResume -= EnableClick;
    }

    private void EnableClick() {
        isEnabled = true;
    }
    private void DisableClick() {
        isEnabled = false;
    }


    private void Awake() {
        localPos = transform.localPosition;
        rigidbody = GetComponent<Rigidbody>();
    }
    

    //Releases the holding object
    public void Release() {
        if (holdingObject == null) return;

        if (Time.timeScale == 0) {
        }
        if (velocity.magnitude > maxThrowForce) {
            velocity = velocity.normalized * maxThrowForce;
        }            
        holdingObject.RigidBodyInfo.Holdvelocity = velocity * throwForceMultiplier;
        holdingObject.Release();
        // holdingObject.gameObject.transform.parent = holidngObjectParent;
        holdingObject = null;
    }

    //updates the holding object velocity;
    private IEnumerator UpdateVelocity() {
        while (holdingObject != null) {
            velocity = holdingObject.gameObject.transform.position - oldPos;
            oldPos = holdingObject.gameObject.transform.position;
            yield return new WaitForSecondsRealtime(.1f);
        }
    }

    //TODO: needs a way better fix than this!
    private IEnumerator UpdatePhysics() {
        while (holdingObject != null) {

            yield return new WaitForEndOfFrame();
        }   
    }

    private void Update() {
        UpdateFocusedObject();
    }
    private void UpdateFocusedObject() {
        IInteractable interactableObj = FocussedObject<IInteractable>();
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


    //raycast froward from the camera to any object that has the component T with it.
    private T FocussedObject<T>() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupDistance)) {
            if (hit.collider.gameObject.GetComponent<T>() != null) return hit.collider.gameObject.GetComponent<T>();
        }
        return default(T);
    }

}
