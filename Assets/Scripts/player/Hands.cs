using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hands : MonoBehaviour
{
    private IPickable holdingObject;
    private Transform holidngObjectParent;
    private Vector3 oldPos = new Vector3();
    private Vector3 velocity = new Vector3();

    public delegate void FocusedAction(bool whiteColor);
    public static event FocusedAction OnFocus; 
    public static event FocusedAction OnUnfocus; 
    [SerializeField]
    private Player player;

    private float pickupDistance = 6;
    private float shrinkPickupDistance;

    private float massThreshhold = 1f;
    public float MassThreshhold {
        get { return massThreshhold;}
        set { massThreshhold = value; }
    }


    public float PickupDistance {
        get {
            return pickupDistance / player.NormalScale * player.CurrentScale;
        }
    }

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
        IInteractable interactableObj = FocussedObject();
        if (interactableObj != default(IInteractable)) {
            IPickable pickable = interactableObj.Gameobject.GetComponent<IPickable>();
            if (pickable != default(IPickable) && pickable.TooHeavy(this) == false) {
                Grab(pickable);

            } else if (interactableObj.Interactable){
                interactableObj.Interact();
            }
        }
    }
    private void Grab(IPickable obj) {
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
        shrinkPickupDistance = pickupDistance / 2f;
    }


    //Releases the holding object
    public void Release() {
        // return;
        if (holdingObject == null) return;
        if (holdingObject.CanBeReleased() == false) return;

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
            if (holdingObject.TooHeavy(this)) Release();
            yield return new WaitForSecondsRealtime(.1f);
        }
    }
    public Vector3 test;
    private IEnumerator UpdatePhysics() {
        while (holdingObject != null) {
            holdingObject.UpdateGrabPhysics();
            Transform t = Camera.main.transform;

            var speed = holdingObject.Touching ? 3f : 10f;
            holdingObject.RigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            Vector3 offset = t.TransformDirection(new Vector3(0,0,1)).normalized * (holdingObject.HoldingDistance) + t.TransformDirection(holdingObject.HoldingOffset);
            Vector3 midwayDestination = Vector3.Lerp(holdingObject.HoldingPosition, t.position + offset + holdingObject.HoldingLocalOffset, Time.deltaTime * speed);
            holdingObject.RigidBody.MovePosition(midwayDestination);
            // holdingObject.RigidBody.MovePosition(t.position + offset);

            if (holdingObject.HoldOrientation != HoldOrientation.none) {
                Quaternion currentRotation = holdingObject.RigidBody.rotation;

                holdingObject.RigidBody.transform.LookAt(transform.position, transform.up);
                if (holdingObject.HoldOrientation == HoldOrientation.shard) {
                    holdingObject.RigidBody.transform.Rotate(-90, 90, -90);
                } else {
                    holdingObject.RigidBody.transform.Rotate(0,180,0);
                }
                Quaternion desiredRotation = holdingObject.RigidBody.rotation;

                holdingObject.RigidBody.rotation = currentRotation;
                holdingObject.RigidBody.MoveRotation(Quaternion.SlerpUnclamped(currentRotation, desiredRotation, Time.deltaTime * 10f));

                // holdingObject.RigidBody.rotation.LookAt(transform.position, transform.up);
            }
            yield return new WaitForFixedUpdate();
            
        }   
    }
    private void OnDrawGizmosSelected() {
        // if (holdingObject != null && test != null) {
        //     Gizmos.DrawSphere(test, 1f);
        // }
    }

    private void Update() {
        if(holdingObject == null) 
            UpdateFocusedObject();
    }
    private void UpdateFocusedObject() {
        IInteractable interactableObj = FocussedObject();
        if (interactableObj != default(IInteractable)) {
            if (interactableObj != currentInteractable && interactableObj.Interactable) {
                if (currentInteractable != default(IInteractable))
                    currentInteractable.Focused = false;
                if (interactableObj.Gameobject.GetComponent<IPickable>() != default(IPickable)) {
                    if (interactableObj.Gameobject.GetComponent<IPickable>().TooHeavy(this)){
                        interactableObj.Disabled = true;
                        interactableObj.Focused = true;
                        currentInteractable = interactableObj;
                        OnFocus?.Invoke(false);
                        return;
                    }
                } 
                interactableObj.Disabled = false;
                interactableObj.Focused = true;
                currentInteractable = interactableObj;
                OnFocus?.Invoke(true);
            }
        } else if (currentInteractable != null) {
            OnUnfocus?.Invoke(true);
            currentInteractable.Focused = false;
            currentInteractable = default(IInteractable);
        }
    }


    //raycast froward from the camera to any object that has the component T with it.
    private IInteractable FocussedObject() {
        RaycastHit hit;
        IInteractable result = default(IInteractable);
        if (Physics.Raycast(transform.position, transform.forward, out hit, 20f)) {
            if (hit.collider.gameObject.GetComponent<IInteractable>() != null) {
                result =  hit.collider.gameObject.GetComponent<IInteractable>();
            } else if (hit.collider.transform.parent != null) {
                if (hit.collider.transform.parent.GetComponent<IInteractable>() != null) 
                    result = hit.collider.transform.parent.GetComponent<IInteractable>();
            }
        }
        

        if (result != default(IInteractable)) {
            float distance = Vector3.Distance(transform.position,hit.point);
            if (distance < result.InteractableDistance) return result;
        } 
        return default(IInteractable);
    }

}
