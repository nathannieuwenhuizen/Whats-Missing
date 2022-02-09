using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hands : MonoBehaviour
{
    private IPickable holdingObject;
    private Transform holidngObjectParent;
    private Vector3 oldPos = new Vector3();
    private Vector3 velocity = new Vector3();

    public delegate void FocusedAction();
    public static event FocusedAction OnFocus; 
    public static event FocusedAction OnUnfocus; 

    [SerializeField]
    private float pickupDistance = 3;
    private float shrinkPickupDistance;

    private bool shrinked = false;

    public float PickupDistance {
        get {
            return shrinked ? shrinkPickupDistance : pickupDistance;
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
        IInteractable interactableObj = FocussedObject<IInteractable>();
        if (interactableObj != default(IInteractable)) {
            if (interactableObj.Gameobject.GetComponent<IPickable>() != default(IPickable)) {
                Grab(interactableObj.Gameobject.GetComponent<IPickable>());
            } else if (interactableObj.Interactable){
                interactableObj.Interact();
            }
        }
    }
    private void Grab(IPickable obj) {
        holdingObject = obj;
        oldPos = holdingObject.gameObject.transform.position;
        obj.Grab(rigidbody);
        AudioHandler.Instance?.PlaySound(SFXFiles.player_grab, .5f);
        StartCoroutine(UpdateVelocity());
        StartCoroutine(UpdatePhysics()); 
    }

    private void OnEnable() {
        InputManager.OnClickDown += OnClick;
        InputManager.OnClickUp += Release;
        PauseScreen.OnPause += DisableClick;
        PauseScreen.OnResume += EnableClick;
        Player.OnPlayerShrink += PlayerHasShrunk;
        Player.OnPlayerUnShrink += PlayerHasUnShrunk;
    }

    private void OnDisable() {
        InputManager.OnClickDown -= OnClick;
        InputManager.OnClickUp -= Release;
        PauseScreen.OnPause -= DisableClick;
        PauseScreen.OnResume -= EnableClick;
        Player.OnPlayerShrink -= PlayerHasShrunk;
        Player.OnPlayerUnShrink -= PlayerHasUnShrunk;
    }

    private void PlayerHasShrunk() {
        shrinked = true;

    }
    private void PlayerHasUnShrunk() {
        shrinked = false;
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
        if (holdingObject == null) return;

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

    private IEnumerator UpdatePhysics() {
        while (holdingObject != null) {
            for(int i=0 ; i < 1f; i++) {
                var speed = holdingObject.Touching ? 3f : 10f;
                holdingObject.RigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
                Vector3 offset = transform.TransformDirection(new Vector3(0,0,1)).normalized * (PickupDistance * .5f);
                Vector3 midwayDestination = Vector3.Lerp(holdingObject.RigidBody.transform.position, transform.position + offset, Time.deltaTime * speed);
                holdingObject.RigidBody.MovePosition(midwayDestination);
                yield return new WaitForFixedUpdate();
            }
        }   
    }

    private void Update() {
        if( holdingObject == null) 
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
                OnFocus?.Invoke();
            }
        } else if (currentInteractable != null) {
            OnUnfocus?.Invoke();
            currentInteractable.Focused = false;
            currentInteractable = default(IInteractable);
        }
    }


    //raycast froward from the camera to any object that has the component T with it.
    private T FocussedObject<T>() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, PickupDistance)) {
            if (hit.collider.gameObject.GetComponent<T>() != null) {
                return hit.collider.gameObject.GetComponent<T>();
            } else if (hit.collider.transform.parent != null) {
                if (hit.collider.transform.parent.GetComponent<T>() != null) 
                    return hit.collider.transform.parent.GetComponent<T>();
            }
        }
        return default(T);
    }

}
